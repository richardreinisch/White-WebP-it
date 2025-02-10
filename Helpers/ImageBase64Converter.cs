
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;


namespace WhiteWebPit.Helpers
{
    internal class ImageBase64Converter
    {
        public async Task<BitmapImage?> From(byte[] imageData)
        {

            BitmapImage? toReturn = null;

            try
            {

                using (InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream())
                {
                    using (DataWriter writer = new DataWriter(memoryStream.GetOutputStreamAt(0)))
                    {
                        writer.WriteBytes((byte[])imageData);
                        writer.StoreAsync().GetResults();
                    }

                    var bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(memoryStream);

                    return bitmapImage;

                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return toReturn;

        }

    }

}
