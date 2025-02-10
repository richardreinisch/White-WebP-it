
using Microsoft.UI.Xaml.Controls;


namespace WhiteWebPit.Interfaces
{
    internal class StatusBarMessage
    {
        public string Message { get; set; } = "not yet set";

        public InfoBarSeverity Severity { get; set; } = 0;

        public bool IsOpen { get; set; } = false;

    }

}
