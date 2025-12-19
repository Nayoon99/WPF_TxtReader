using Microsoft.Win32;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TxtReader.Model;

namespace TxtReader.ViewModel
{
    internal class MainViewModel : BaseViewModel
    {
        // 1. 파일 목록 (상태)
        public ObservableCollection<FileModel> Files { get; } = new ObservableCollection<FileModel>();

        // 2. 버튼 명령
        public Command SelectFileCommand { get; }

        // 생성자
        public MainViewModel()
        {
            SelectFileCommand = new Command(SelectFile);
        }

        /** 파일 선택 */
        private FileModel _selectedFile;
        public FileModel SelectedFile
        {
            get => _selectedFile;
            set => SetProperty(ref _selectedFile, value);
        }
        
        /** 선택된 파일 미리보기 */
        private void SelectFile()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;

                string content = ReadTextSmart(path);

                FileModel file = new()
                {
                    Title = Path.GetFileName(path),
                    Content = content,
                    Size = new FileInfo(path).Length,
                    Path = path
                };

                Files.Add(file);
                SelectedFile = file;
                SetupFileWatcher(path);
            }
        }

        /** txt 인코딩 종류 구분 */
        private string ReadTextSmart(string path)
        {
            // 1. UTF-8 먼저 시도
            string utf8 = File.ReadAllText(path, Encoding.UTF8);

            // 2. 깨짐 문자 포함 여부 검사
            if (!utf8.Contains("�"))
            {
                return utf8; // UTF-8 정상
            }

            // 3. CP949(ANSI) 로 재시도
            return File.ReadAllText(path, Encoding.GetEncoding(949));
        }


        /** 파일 변경 시 자동갱신 **/
        /*
         *  == FileSystemWatcher 사용 방법 ==
         *  => using System.IO 필요
         *  1. FileSystemWatcher 생성자 호출
         *  2. 감시할 폴더 설정 (디렉토리)
         *  3. 감시할 항목 설정 (파일 생성, 크기, 이름, 마지막 접근 변경 등)
         *  4. 감시할 이벤트 설정 (생성, 변경, 삭제 등)
         *  5. FileSystemWatcher 감시 모니터링 활성화
         *  6. 감시할 폴더 내부 변경 시 event 호출
         *  
        */
        private FileSystemWatcher _watcher;

        private void SetupFileWatcher(string path)
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
            }

            _watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(path),
                Filter = "*.*",
                NotifyFilter =
                    NotifyFilters.LastWrite |
                    NotifyFilters.FileName |
                    NotifyFilters.Size
            };

            _watcher.Changed += OnFileChanged;
            _watcher.Created += OnFileChanged;
            _watcher.Renamed += OnFileRenamed;

            _watcher.EnableRaisingEvents = true;

            Debug.WriteLine("FileSystemWatcher STARTED");
        }




        private async void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (SelectedFile == null || e.FullPath != SelectedFile.Path)
                return;

            AddLog(e.FullPath, "Changed", "파일 내용이 수정됨");

            await Task.Delay(300);

            string newContent = ReadTextWithRetry(e.FullPath);

            Application.Current.Dispatcher.Invoke(() =>
            {
                SelectedFile.Content = newContent;
            });
        }



        private async void OnFileRenamed(object sender, RenamedEventArgs e)
        {

            if (SelectedFile == null || e.FullPath != SelectedFile.Path)
                return;

            AddLog(
                e.FullPath,
                "Renamed",
                $"{Path.GetFileName(e.OldFullPath)} → {Path.GetFileName(e.FullPath)}"
            );

            await Task.Delay(300);

            string newContent = ReadTextSmart(e.FullPath);

            Application.Current.Dispatcher.Invoke(() =>
            {
                SelectedFile.Content = newContent;
            });
        }

        private string ReadTextWithRetry(string path)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    using (var stream = new FileStream(
                        path,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite))
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
                catch (IOException)
                {
                    Thread.Sleep(100); // 파일 잠금 풀릴 때까지 대기
                }
            }

            return string.Empty;
        }


        // ======= 로그 저장 =======
        // 로그 저장
        public ObservableCollection<ChangeLogModel> ChangeLogs { get; } = new ObservableCollection<ChangeLogModel>();

        // 로그 추가 메서드
        private void AddLog(string fileName, string changeType, string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                // 최신 로그가 위에 오도록 설정
                ChangeLogs.Insert(0, new ChangeLogModel
                {
                    Time = DateTime.Now,
                    FileName = Path.GetFileName(fileName),
                    ChangeType = changeType,
                    Message = message
                });
            });
        }

    }
}
