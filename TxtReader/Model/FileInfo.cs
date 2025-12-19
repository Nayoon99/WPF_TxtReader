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
        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _content;
        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        private long _size;
        public long Size
        {
            get => _size;
            set => SetProperty(ref _size, value);
        }

        private string _path;
        public string Path
        {
            get => _path;
            set => SetProperty(ref _path, value);
        }

        public override string ToString()
        {
            return $"{Title} ({Size} bytes) : {Content}";
        }
    }
}
