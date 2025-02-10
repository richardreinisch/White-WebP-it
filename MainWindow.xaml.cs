
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using Windows.ApplicationModel.DataTransfer;

using WhiteWebPit.Communication.Interfaces;
using WhiteWebPit.Interfaces;
using WhiteWebPit.Models;
using WhiteWebPit.Services;


namespace WhiteWebPit
{
    internal sealed partial class MainWindow : Window
    {

        public MainViewModel ViewModel { get; }

        public MainWindow(MainViewModel viewModel, IMessenger messenger)
        {

            this.InitializeComponent();

            ViewModel = viewModel;

            messenger.Register<ScrollViewZoomMessage>(HandleScrollViewZoom);
            messenger.Register<StatusBarMessage>(HandleStatusBarMessage);

            ShowBetaWarning();

        }

        private void HandleScrollViewZoom(ScrollViewZoomMessage message)
        {
            Debug.WriteLine("Update Zoom");
            int? imageWidth = message.ImageWidth;
            var scrollWidth = scrollViewImagePreview.ActualWidth;

            if (imageWidth.HasValue)
            {
                float zoomFactor = (float)(scrollWidth / imageWidth);
                scrollViewImagePreview.ZoomTo(zoomFactor, null);
            }
            else
            {
                scrollViewImagePreview.ZoomTo((float)message.ZoomFactor, null);
            }

        }

        private void HandleStatusBarMessage(StatusBarMessage message)
        {
            Debug.WriteLine("Update StatusBar");
            StatusBarMessage(message.Message, message.Severity, message.IsOpen);
        }

        private async void SliderQuality_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            if (ViewModel != null && sliderQuality != null)
            {
                await ViewModel.SliderQuality((uint)sliderQuality.Value);
            }
        }

        private void SliderZoomFactor_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (ViewModel != null && sliderZoomFactor != null)
            {
                ViewModel.SliderZoom(e.NewValue);
            }
        }

        private void ScrollViewImagePreview_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            scrollViewImagePreview.BorderThickness = new Thickness(5);
            scrollViewImagePreview.BorderBrush = (SolidColorBrush)Application.Current.Resources["FocusStrokeColorOuterBrush"];
        }

        private void ScrollViewImagePreview_DragLeave(object sender, DragEventArgs e)
        {
            ResetDragAndDropIndication();
        }

        private async void ScrollViewImagePreview_Drop(object sender, DragEventArgs e)
        {

            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();

                await ViewModel.DropFiles(items);

                ResetDragAndDropIndication();

            }

        }

        private async void BtnSaveAs_Click(object sender, RoutedEventArgs e)
        {

            if (ViewModel != null)
            {

                var savePicker = new FileSavePicker();
                savePicker.FileTypeChoices.Add("WebP Image", new[] { FileService.WEBP_FILE_EXTENSION });
                savePicker.SuggestedFileName = ViewModel.GetSuggestedFilename() + FileService.WEBP_FILE_EXTENSION;

                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

                var fileToSave = await savePicker.PickSaveFileAsync();

                if (fileToSave != null)
                {
                    string savedFilePath = await ViewModel.SaveFileAs(fileToSave);

                    if (!savedFilePath.Contains("undefined"))
                    {
                        StatusBarMessage($"Saved as {savedFilePath}", InfoBarSeverity.Success, true);
                    }
                    else
                    {
                        StatusBarMessage("Could not save file.", InfoBarSeverity.Error, true);
                    }
                }
                else
                {
                    StatusBarMessage("No file selected.", InfoBarSeverity.Error, true);
                }

            }

        }

        private void ResetDragAndDropIndication()
        {
            scrollViewImagePreview.BorderThickness = new Thickness(2);
            scrollViewImagePreview.BorderBrush = null;
        }

        private void ShowBetaWarning()
        {
            statusBar.Message = "Beta Phase: Use at your own risk.";
            statusBar.Severity = InfoBarSeverity.Warning;
            statusBar.IsOpen = true;
        }

        private void StatusBarMessage(string message, InfoBarSeverity severity, bool isOpen)
        {
            statusBar.Message = message;
            statusBar.Severity = severity;
            statusBar.IsOpen = isOpen;
        }

    }

}
