using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmHelpers;

namespace TxtReader.Model
{
    internal class ChangeLogModel : BaseViewModel
    {
        public DateTime Time { get; set; }     // 변경 시간
        public string FileName { get; set; }   // 파일 이름
        public string ChangeType { get; set; } // 변경 유형 (생성, 수정, 삭제)
        public string Message { get; set; }    // 변경 내용

        public override string ToString()
        {
            return $"[{Time:HH:mm:ss}] {FileName} - {ChangeType}";
        }
    }
}
