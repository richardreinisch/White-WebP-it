
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

using WhiteWebPit.Interfaces;
using WhiteWebPit.Services.Interfaces;
using WhiteWebPit.Communication.Interfaces;
using WhiteWebPit.Services;

using ImageMagick;


namespace WhiteWebPit.Models
{
    internal class MainViewModel : INotifyPropertyChanged
    {

        private bool previewEnable = false;
        private bool autoSaveOnDrop = false;
        private bool progressActive = false;
        private uint quality = 85;

        private BitmapImage? imagePreview;
        private BitmapImage? imagePreviewBackground;

        private ImageData baseImageData;
        private ImageData previewImageData;
        private ImageData previewFallbackImageData;

        private readonly IModeService modeService;
        private readonly IPreviewRendererService previewRendererService;
        private readonly IFileService fileService;

        private readonly IMessenger messenger;


        public MainViewModel(IModeService modeService, IPreviewRendererService previewRendererService, IFileService fileService, IMessenger messenger) 
        {

            this.modeService = modeService;
            this.previewRendererService = previewRendererService;
            this.fileService = fileService;
            this.messenger = messenger;

            this.baseImageData = new();
            this.previewImageData = new();
            this.previewFallbackImageData = new();

            ImagePreviewBackground = new BitmapImage(new Uri("ms-appx:///Resources/Images/drag-drop.png"));

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool PreviewEnable
        {
            get => previewEnable;
            set
            {
                if (previewEnable != value)
                {
                    previewEnable = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool PreviewChecked
        {
            get => modeService.PreviewModeActive;
            set
            {
                if (modeService.PreviewModeActive != value)
                {
                    modeService.PreviewModeActive = value;
                    PreviewClick();
                    OnPropertyChanged();
                }
            }
        }

        public bool ProgressActive
        {
            get => progressActive;
            set
            {
                if (progressActive != value)
                {
                    progressActive = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool AutoSaveOnDrop
        {
            get => autoSaveOnDrop;
            set
            {
                if (autoSaveOnDrop != value)
                {
                    autoSaveOnDrop = value;
                    OnPropertyChanged();
                }
            }
        }

        public BitmapImage? ImagePreview
        {
            get => imagePreview;
            set
            {
                if (imagePreview != value)
                {
                    imagePreview = value;
                    OnPropertyChanged();
                }
            }
        }

        public BitmapImage? ImagePreviewBackground
        {
            get => imagePreviewBackground;
            set
            {
                if (imagePreviewBackground != value)
                {
                    imagePreviewBackground = value;
                    OnPropertyChanged();
                }
            }
        }

        public void ZoomScrollView(double zoomFactor, int? imageWidth)
        {
            messenger.Send(new ScrollViewZoomMessage
            {
                ZoomFactor = zoomFactor,
                ImageWidth = imageWidth
            });
        }

        public void SendStatusBarMessage(string message, InfoBarSeverity severity, bool isOpen)
        {
            messenger.Send(new StatusBarMessage
            {
                Message = message,
                Severity = severity,
                IsOpen = isOpen
            });
        }

        public void SliderZoom(double? value)
        {
            if (value != null && modeService.ZoomFactor != value)
            {
                modeService.ZoomFactor = value ?? 1.0;
                ZoomScrollView(modeService.ZoomFactor, null);
            }
        }

        public async Task SliderQuality(uint newQuality)
        {

            quality = newQuality;

            Debug.WriteLine($"Slider released, Quality: {quality}");

            if (modeService.PreviewModeActive)
            {
                ProgressActive = true;
                await UpdatePreviewImage();
                HandleDisplayMode();
                ProgressActive = false;
                PreviewEnable = true;
            }

        }

        public async Task DropFiles(IReadOnlyList<IStorageItem> items)
        {

            if (fileService.GetFirstStorageFile(items) is StorageFile storageFile)
            {

                if (FileService.IsValidFileType(storageFile))
                {

                    try
                    {

                        var newImage = await fileService.LoadImage(storageFile);

                        if (newImage != null)
                        {
                            baseImageData = new();
                            baseImageData.Image = newImage;
                            ImagePreview = baseImageData.Image;
                            ImagePreviewBackground = null;
                            baseImageData.ImageFile = storageFile;
                            baseImageData.ImageMagick = new MagickImage(storageFile.Path);
                            modeService.TestModeActive = false;
                            PreviewEnable = true;

                            if (AutoSaveOnDrop)
                            {
                                await Save();
                            }
                        }
                        else
                        {
                            ShowError("Could not load image.");
                        }

                    } catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        ShowError("Could not load image.");
                    }

                }
                else
                {
                    ShowError("File type not supported.");
                }

            }
            else
            {
                ShowError("Could not get file.");
            }

        }

        private void ZoomPreview100PercentWidth(BitmapImage? image)
        {

            if (image != null)
            {                
                var imageWidth = image.PixelWidth;

                if (imageWidth > 0)
                {
                    ZoomScrollView(modeService.ZoomFactor, imageWidth);
                }
            }

        }

        private async void PreviewClick()
        {

            if (modeService.PreviewModeActive)
            {
                PreviewEnable = false;
                ProgressActive = true;

                try
                {
                    await UpdatePreviewImage();
                }
                catch (Exception ex)
                {
                    ShowError(ex.ToString());
                }
                finally
                {
                    ProgressActive = false;
                    PreviewEnable = true;
                }
            }

            HandleDisplayMode();

        }

        public async Task Save()
        {
            string savedFilePath = await fileService.ConvertAndStoreAsWebP(baseImageData.ImageFile, quality) ?? "undefined";
            if (!savedFilePath.Contains("undefined")) {
                ShowSucess($"Saved as {savedFilePath}");
            } else
            {
                ShowError("Could not save file.");
            }
        }

        public string GetSuggestedFilename()
        {
            return baseImageData.Filename ?? "";
        }

        public async Task<string> SaveFileAs(StorageFile destinationFile)
        {
            string toReturn = "undefined";
            try
            {
                toReturn = await fileService.ConvertAndStoreAsWebP(baseImageData.ImageFile?.Path, destinationFile, quality) ?? "undefined";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return toReturn;
        }

        private void HandleDisplayMode()
        {
            if (previewFallbackImageData.Image == null)
            {
                modeService.LoadPreviewFallbackImage(previewFallbackImageData);
            }

            if (modeService.PreviewModeActive)
            {
                if (previewImageData.Image != null)
                {
                    Debug.WriteLine("Show converted");
                    SetPreviewImage(previewImageData.Image);
                }
                else if (previewFallbackImageData.Image != null)
                {
                    Debug.WriteLine("Image not prepared, show fallback.");
                    SetPreviewImage(previewFallbackImageData.Image);
                    ZoomPreview100PercentWidth(previewFallbackImageData.Image);
                }
                else
                {
                    Debug.WriteLine("No image for preview given.");
                }

            }
            else
            {
                Debug.WriteLine("Show original");
                SetPreviewImage(baseImageData.Image);
            }

        }

        private void SetPreviewImage(BitmapImage? bitmapImage)
        {
            if (bitmapImage != null)
            {
                ImagePreview = bitmapImage;
            }
            else
            {
                Debug.WriteLine("Could not update image container.");
            }
        }

        private async Task UpdatePreviewImage()
        {
            Debug.WriteLine("Update Preview-Image");

            if (modeService.TestModeActive)
            {
                previewImageData.Image = await previewRendererService.RenderPreviewImage(baseImageData.ImageArray, previewImageData, quality);
            }
            else
            {
                previewImageData.Image = await previewRendererService.RenderPreviewImage(baseImageData.ImageMagick, previewImageData, quality);
            }

        }

        private void ShowSucess(string message)
        {
            SendStatusBarMessage(message, InfoBarSeverity.Success, true);
        }

        private void ShowError(string message)
        {
            SendStatusBarMessage(message, InfoBarSeverity.Error, true);
        }

        public async void IconBunnyPointerPressed()
        {
            modeService.ClickedBunny();

            if (modeService.TestModeActive)
            {
                Debug.WriteLine("Load TestImage");
                baseImageData = new();
                await modeService.LoadTestImage(baseImageData);
                HandleDisplayMode();
                ImagePreviewBackground = null;
                ZoomPreview100PercentWidth(baseImageData.Image);
                ShowSucess("Test Image loaded.");
                PreviewEnable = true;
                modeService.ResetBunnyClick();
            }
        }

    }

}
