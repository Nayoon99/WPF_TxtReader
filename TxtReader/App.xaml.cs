using System.Configuration;
using System.Data;
using System.Text;
using System.Windows;

namespace TxtReader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // CP949 / ANSI 인코딩 사용 가능하게 등록
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
    }

}
