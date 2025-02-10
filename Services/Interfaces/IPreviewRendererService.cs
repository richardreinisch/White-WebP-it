
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;

using WhiteWebPit.Models;

using ImageMagick;


namespace WhiteWebPit.Services.Interfaces
{

    internal interface IPreviewRendererService
    {
        Task<BitmapImage?> RenderPreviewImage(byte[]? rawImage, ImageData previewImageData, uint quality);

        Task<BitmapImage?> RenderPreviewImage(MagickImage? magickImage, ImageData previewImageData, uint quality);

    }

}