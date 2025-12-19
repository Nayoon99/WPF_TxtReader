using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxtReader.Model
{
    // 변동사항 감지에서 화면에 반영해주기 위해 BaseViewModel 상속
    internal class FileModel : BaseViewModel
    {
        // 1. 파일 제목
        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        // 2. 파일 내용
        private string _content;
        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        // 3. 파일 크기
        private long _size;
        public long Size
        {
            get => _size;
            set => SetProperty(ref _size, value);
        }

        // 4. 파일 경로
        private string _path;
        public string Path
        {
            get => _path;
            set => SetProperty(ref _path, value);
        }

        // 5. 파일 버전
        private string _version = "0.0.1"; // 기본값 설정
        public string Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }

        public override string ToString()
        {
            return $"{Title} v{Version} ({Size} bytes) : {Content}";
        }
    }
}
