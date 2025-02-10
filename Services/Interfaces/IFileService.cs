
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;


namespace WhiteWebPit.Services.Interfaces
{
    internal interface IFileService
    {
        Task<string?> ConvertAndStoreAsWebP(StorageFile? baseFile, uint quality);
        Task<string?> ConvertAndStoreAsWebP(string? basePathAndFilename, StorageFile storageFile, uint quality);
        StorageFile? GetFirstStorageFile(IReadOnlyList<IStorageItem> storageFiles);
        Task<BitmapImage?> LoadImage(StorageFile storageFile);
    }

}