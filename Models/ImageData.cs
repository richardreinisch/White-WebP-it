
using System;
using Windows.Storage;
using Microsoft.UI.Xaml.Media.Imaging;

using ImageMagick;


namespace WhiteWebPit.Models
{
    public class ImageData
    {

        private readonly object imageDataLock = new();

        private BitmapImage? image;
        private StorageFile? imageFile;
        private MagickImage? imageMagick;
        private byte[]? imageArray;
        private String? imagePath = null;
        private String? filename = null;
        private long byteSize = 0;


        public BitmapImage? Image
        {
            get
            {
                lock (imageDataLock)
                {
                    return image;
                }
            }

            set
            {
                lock (imageDataLock)
                {
                    image = value;
                }
            }
        }

        public StorageFile? ImageFile
        {
            get
            {
                lock (imageDataLock)
                {
                    return imageFile;
                }
            }

            set
            {
                lock (imageDataLock)
                {
                    imageFile = value;
                }
            }
        }

        public MagickImage? ImageMagick
        {
            get
            {
                lock (imageDataLock)
                {
                    return imageMagick;
                }
            }

            set
            {
                lock (imageDataLock)
                {
                    imageMagick = value;
                }
            }
        }

        public byte[]? ImageArray
        {
            get
            {
                lock (imageDataLock)
                {
                    return imageArray;
                }
            }

            set
            {
                lock (imageDataLock)
                {
                    imageArray = value;
                }
            }
        }

        public string? ImagePath
        {
            get
            {
                lock (imageDataLock)
                {
                    return imagePath;
                }
            }

            set
            {
                lock (imageDataLock)
                {
                    imagePath = value;
                }
            }
        }

        public string? Filename
        {
            get
            {
                lock (imageDataLock)
                {
                    return filename;
                }
            }

            set
            {
                lock (imageDataLock)
                {
                    filename = value;
                }
            }
        }

        public long ByteSize
        {
            get
            {
                lock (imageDataLock)
                {
                    return byteSize;
                }
            }

            set
            {
                lock (imageDataLock)
                {
                    byteSize = value;
                }
            }
        }

    }

}
