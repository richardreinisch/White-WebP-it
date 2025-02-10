
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Microsoft.UI.Xaml.Media.Imaging;

using WhiteWebPit.Services.Interfaces;

using ImageMagick;


namespace WhiteWebPit.Services
{
    internal class FileService : IFileService
    {

        public static string WEBP_FILE_EXTENSION = ".webp";

        public FileService() { }

        public static bool IsValidFileType(StorageFile storageFile)
        {
            return storageFile.FileType == ".jpg" || storageFile.FileType == ".jpeg" || storageFile.FileType == ".png" || storageFile.FileType == WEBP_FILE_EXTENSION;
        }

        public async Task<string?> ConvertAndStoreAsWebP(string? basePathAndFilename, StorageFile storageFile, uint quality)
        {

            string? toReturn = null;

            if (basePathAndFilename != null && basePathAndFilename.Length > 0 && storageFile != null)
            {
                try
                {
                    toReturn = await StoreWebP(basePathAndFilename, storageFile.Path, quality);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    throw new Exception("Could not save Image.");
                }
            }

            return toReturn;

        }

        public async Task<string?> ConvertAndStoreAsWebP(StorageFile? baseFile, uint quality)
        {

            string? toReturn = null;

            if (baseFile != null)
            {
                try
                {
                    toReturn = await StoreWebP(baseFile.Path, null, quality);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            return toReturn;

        }

        private async Task<string?> StoreWebP(string baseImagePathAndFilename, string? destinationPathAndFilename, uint quality)
        {

            string? toReturn = null;

            try
            {

                Debug.WriteLine($"Base File: {baseImagePathAndFilename}");

                using var image = new MagickImage(baseImagePathAndFilename);

                image.Format = MagickFormat.WebP;
                image.Quality = quality;
                image.Settings.SetDefine(MagickFormat.WebP, "lossless", false);

                if (destinationPathAndFilename == null)
                { 

                    var filenameNoExtension = Path.GetFileNameWithoutExtension(baseImagePathAndFilename);
                    var destinationPath = Path.GetDirectoryName(baseImagePathAndFilename);

                    if (filenameNoExtension != null && destinationPath != null)
                    {
                        toReturn = Path.Combine(destinationPath, filenameNoExtension + WEBP_FILE_EXTENSION);
                    } else
                    {
                        throw new Exception("Path not ok.");
                    }

                    if (File.Exists(toReturn))
                    {
                        throw new Exception("Already exists.");
                    }

                }
                else
                {
                    toReturn = destinationPathAndFilename;
                }

                Debug.WriteLine($"Destination File: {toReturn}");

                await image.WriteAsync(toReturn);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw new Exception("MagickImage could not be saved.");
            }

            return toReturn;

        }

        public async Task<BitmapImage?> LoadImage(StorageFile storageFile)
        {

            BitmapImage? toReturn = null;

            try
            {
                toReturn = new BitmapImage();
                await toReturn.SetSourceAsync(await LoadImageStream(storageFile));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return toReturn;

        }

        private async Task<IRandomAccessStream> LoadImageStream(StorageFile storageFile)
        {
            return await storageFile.OpenAsync(FileAccessMode.Read);
        }

        public StorageFile? GetFirstStorageFile(IReadOnlyList<IStorageItem> storageFiles)
        {

            StorageFile? toReturn = null;

            if (storageFiles.Count > 0)
            {
                toReturn = storageFiles[0] as StorageFile;
            }

            return toReturn;

        }

    }

}
