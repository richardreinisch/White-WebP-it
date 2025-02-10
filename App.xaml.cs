
using Microsoft.UI.Xaml;

using WhiteWebPit.Interfaces;
using WhiteWebPit.Models;
using WhiteWebPit.Services;
using WhiteWebPit.Services.Interfaces;


namespace WhiteWebPit
{
    public partial class App : Application
    {

        private Window? window;

        private readonly IModeService modeService;
        private readonly IPreviewRendererService previewRendererService;
        private readonly IFileService fileService;

        public App()
        {
            this.InitializeComponent();

            modeService = new ModeService();
            previewRendererService = new PreviewRendererService();
            fileService = new FileService();

        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {

            var messenger = new Messenger();
            var mainViewModel = new MainViewModel(modeService, previewRendererService, fileService, messenger);

            window = new MainWindow(mainViewModel, messenger);
            window.Activate();

        }

    }

}
