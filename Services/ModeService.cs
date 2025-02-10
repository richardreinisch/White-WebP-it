
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.UI.Xaml.Media.Imaging;

using WhiteWebPit.Helpers;
using WhiteWebPit.Models;
using WhiteWebPit.Services.Interfaces;


namespace WhiteWebPit.Services
{

    internal class ModeService : IModeService
    {

        private static int MIN_BUNNY_CLICK = 3;

        private int countBunnyClick = 0;
        public bool TestModeActive { get; set; } = false;
        public double ZoomFactor { get; set; } = 1.0f;
        public bool PreviewModeActive { get; set; } = false;


        public ModeService() { }

        public void ClickedBunny()
        {
            countBunnyClick++;

            if (countBunnyClick >= MIN_BUNNY_CLICK)
            {
                Debug.WriteLine("Bunny clicked.");
                TestModeActive = true;
            }
        }

        public void ResetBunnyClick()
        {
            countBunnyClick = 0;
        }

        public async Task LoadTestImage(ImageData imageData)
        {

            try
            {
                imageData.ImagePath = "ms-appx:///Resources/Images/test.b64";
                var uri = new Uri(imageData.ImagePath);
                var storageFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
                var data = await FileIO.ReadTextAsync(storageFile);
                imageData.ImageArray = Convert.FromBase64String(data);
                imageData.Image = await new ImageBase64Converter().From(imageData.ImageArray);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not load test image.", ex);
            }

        }

        public void LoadPreviewFallbackImage(ImageData? imageData)
        {

            if (imageData != null)
            {

                try
                {
                    imageData.ImagePath = "ms-appx:///Resources/Images/preview-fallback.png";
                    var uri = new Uri(imageData.ImagePath);
                    imageData.Image = new BitmapImage(uri);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Could not load preview fallback image.", ex);
                }

            }

        }

    }

}
