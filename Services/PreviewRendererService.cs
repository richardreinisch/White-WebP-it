
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Windows.Storage.Streams;
using Microsoft.UI.Xaml.Media.Imaging;

using WhiteWebPit.Models;
using WhiteWebPit.Services.Interfaces;

using ImageMagick;

using ByteSizeLib;


namespace WhiteWebPit.Services
{
    internal class PreviewRendererService : IPreviewRendererService
    {

        public PreviewRendererService() { }

        public async Task<BitmapImage?> RenderPreviewImage(MagickImage? magickImage, ImageData previewImageData, uint quality)
        {

            return await RenderImage(magickImage, previewImageData, quality);

        }

        public async Task<BitmapImage?> RenderPreviewImage(byte[]? rawImage, ImageData previewImageData, uint quality)
        {

            var image = rawImage != null ? new MagickImage(rawImage) : null;

            return await RenderImage(image, previewImageData, quality);

        }

        private async Task<BitmapImage?> RenderImage(MagickImage? magickImage, ImageData previewImageData, uint quality)
        {

            BitmapImage? toReturn = null;

            if (magickImage != null)
            {
                try
                {

                    magickImage.Format = MagickFormat.WebP;
                    magickImage.Quality = quality;
                    magickImage.Settings.SetDefine(MagickFormat.WebP, "lossless", false);

                    using (var memoryStream = new MemoryStream())
                    {
                        await magickImage.WriteAsync(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        toReturn = await LoadImageFromStream(memoryStream, previewImageData);
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Could not write image to memory. {ex.Message}");
                }
            }

            return toReturn;

        }


        private async Task<BitmapImage?> LoadImageFromStream(MemoryStream memoryStream, ImageData previewImageData)
        {

            BitmapImage? toReturn = null;

            if (memoryStream != null && memoryStream.Length > 0)
            {

                try
                {

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    previewImageData.ByteSize = memoryStream.Length;

                    Debug.WriteLine($"Memory-Stream Size: {ByteSize.FromBytes(memoryStream.Length)}");

                    var randomAccessStream = new InMemoryRandomAccessStream();

                    using (var outputStream = randomAccessStream.GetOutputStreamAt(0))
                    {
                        await memoryStream.CopyToAsync(outputStream.AsStreamForWrite());
                        await outputStream.FlushAsync();
                    }

                    var bitmapImage = new BitmapImage();

                    // TBD: Stream with size smaller than 17 KB cannot be handled by BitmapImage

                    try
                    {

                        randomAccessStream.Seek(0);
                        await bitmapImage.SetSourceAsync(randomAccessStream);
                        Debug.WriteLine($" W, H {bitmapImage.PixelWidth} {bitmapImage.PixelHeight}");

                    }
                    catch (COMException ex)
                    {
                        Debug.WriteLine($"Failed to load image from stream: {ex.Message}");
                        bitmapImage = null;
                    }

                    return bitmapImage;

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Could not load image from memory-stream. {ex.GetType} - {ex.Message}");
                }

            }
            else
            {
                Debug.WriteLine("Stream not given.");
            }

            return toReturn;

        }

    }

}
