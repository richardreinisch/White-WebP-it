
using System.Threading.Tasks;

using WhiteWebPit.Models;


namespace WhiteWebPit.Services.Interfaces
{
    internal interface IModeService
    {

        bool PreviewModeActive { get; set; }

        bool TestModeActive { get; set; }

        double ZoomFactor { get; set; }

        void ClickedBunny();

        void ResetBunnyClick();

        void LoadPreviewFallbackImage(ImageData? imageData);

        Task LoadTestImage(ImageData? imageData);

    }

}