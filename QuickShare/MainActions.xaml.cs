﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace QuickShare
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainActions : Page
    {
        public MainActions()
        {
            this.InitializeComponent();

            ClipboardContentPreviewContainer.Opacity = 0;
            ClipboardButton.Opacity = 0;
            sendPictureButton.Opacity = 0;
            SelectFileButton.Opacity = 0;
            DragDropText.Opacity = 0;

            if (Common.DeviceInfo.FormFactorType == Common.DeviceInfo.DeviceFormFactorType.Phone)
                DragDropText.Visibility = Visibility.Collapsed;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            buttonsShowStoryboard.Begin();
            await InitClipboardAsync();

            try
            {
                await InitPicturePicker();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to initialize picture picker. Will try again after a second. Exception: " + ex.ToString());
                try
                {
                    await Task.Delay(1000);
                    await InitPicturePicker();
                }
                catch (Exception ex2)
                {
                    Debug.WriteLine("Failed to initialize picture picker again. :( Exception: " + ex2.ToString());
                }
            }
        }

        private async Task InitPicturePicker()
        {
            StorageFolder f = KnownFolders.PicturesLibrary;
            List<StorageFile> files = (await f.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.OrderByDate, 0, 3)).ToList();
            List<BitmapImage> bitmaps = new List<BitmapImage>();

            foreach (var file in files)
            {
                var thumbnailStream = await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.SingleItem);
                if (thumbnailStream != null)
                {
                    var image = new BitmapImage();
                    await image.SetSourceAsync(thumbnailStream);
                    bitmaps.Add(image);
                }
            }

            if (bitmaps.Count >= 1)
                img1.Source = bitmaps[0];
            if (bitmaps.Count >= 2)
                img2.Source = bitmaps[1];
            if (bitmaps.Count >= 3)
                img3.Source = bitmaps[2];
            imageShowStoryboard.Begin();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            GoodbyeClipboard();

            base.OnNavigatingFrom(e);
        }

        private void ClipboardButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SendDataTemporaryStorage.Text = clipboardTextContent;

            Frame.Navigate(typeof(MainSend), "text");
        }

        private async void SelectFile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            picker.FileTypeFilter.Add("*");

            var files = await picker.PickMultipleFilesAsync();

            SendDataTemporaryStorage.Files.Clear();
            SendDataTemporaryStorage.Files.AddRange(files);

            Frame.Navigate(typeof(MainSend), "file");
        }

        private void sendPictureButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(PicturePicker));
        }

        private void ClipboardLaunchUrlButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SendDataTemporaryStorage.LaunchUri = new Uri(clipboardTextContent);

            Frame.Navigate(typeof(MainSend), "launchUri");
        }
    }
}
