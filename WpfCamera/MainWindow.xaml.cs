using Common;
using Database;
using log4net;
using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace WpfCamera
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly MediaCapture _mediaCapture = new MediaCapture();
		private readonly CaptureElement _captureElement;
		private StorageFolder _captureFolder;
		private bool _initialized = false;
		private bool _isRecording = false;

		public MainWindow()
		{
            LogHelper.Logger = LogManager.GetLogger("MyFileAppender");
            LogHelper.LogInfo($"Start");
            //var t = new LocalDbContext();

            InitializeComponent();

			_captureElement = new CaptureElement
			{
				Stretch = Windows.UI.Xaml.Media.Stretch.Uniform
			};
			_captureElement.Loaded += CaptureElement_Loaded;
			_captureElement.Unloaded += CaptureElement_Unloaded;

			XamlHost.Child = _captureElement;
		}

		private async void CaptureElement_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			await _mediaCapture.StopPreviewAsync();
		}

		private async void CaptureElement_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			if (!_initialized)
			{
				var picturesLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
				// Fall back to the local app storage if the Pictures Library is not available
				_captureFolder = picturesLibrary.SaveFolder ?? ApplicationData.Current.LocalFolder;

				// Get available devices for capturing pictures
				var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

				if(allVideoDevices.Count > 0)
				{
					// try to find back camera
					DeviceInformation desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back);

					// If there is no device mounted on the back panel, return the first device found
					var device = desiredDevice ?? allVideoDevices.FirstOrDefault();

					await _mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings() { VideoDeviceId = device.Id });
					_captureElement.Source = _mediaCapture;

					_initialized = true;
				}
			}

			if (_initialized)
			{
				await _mediaCapture.StartPreviewAsync();
			}
		}

		private async void Video_Click(object sender, RoutedEventArgs e)
		{
			if (!_initialized)
			{
				return;
			}

			if (_isRecording)
			{ // stop recording
				_isRecording = false;
				await _mediaCapture.StopRecordAsync();
			}
			else
			{ // start recording
				var videoFile = await _captureFolder.CreateFileAsync("Video.wmv", CreationCollisionOption.GenerateUniqueName);

				var encodingProfile = MediaEncodingProfile.CreateWmv(VideoEncodingQuality.Auto);

				await _mediaCapture.StartRecordToStorageFileAsync(encodingProfile, videoFile);

				_isRecording = true;
			}
		}

		private async void Photo_Click(object sender, RoutedEventArgs e)
		{
			if (!_initialized)
			{
				return;
			}

			using var stream = new InMemoryRandomAccessStream();

			await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);

			try
			{
				var file = await _captureFolder.CreateFileAsync("Photo.jpg", CreationCollisionOption.GenerateUniqueName);

				var decoder = await BitmapDecoder.CreateAsync(stream);

				using var outputStream = await file.OpenAsync(FileAccessMode.ReadWrite);
				var encoder = await BitmapEncoder.CreateForTranscodingAsync(outputStream, decoder);

				await encoder.FlushAsync();
			}
			catch (Exception)
			{
			}
			//Screen primaryScreen = SystemParameters.PrimaryScreen;
			double screenLeft = 0; // SystemParameters.VirtualScreenLeft;
			double screenTop = 0;// SystemParameters.VirtualScreenTop;
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            using (Bitmap bmp = new Bitmap((int)screenWidth,
                (int)screenHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    String filename = "ScreenCapture-" + DateTime.Now.ToString("ddMMyyyy-hhmmss") + ".png";
                    Opacity = .0;
                    g.CopyFromScreen((int)screenLeft, (int)screenTop, 0, 0, bmp.Size);
                    bmp.Save("C:\\Screenshots\\" + filename);
                    Opacity = 1;
                }
				 
            }
        }

		private async void Audio_Click(object sender, RoutedEventArgs e)
		{
			if (!_initialized)
			{
				return;
			}

			if (_isRecording)
			{ // stop recording
				_isRecording = false;
				await _mediaCapture.StopRecordAsync();
			}
			else
			{ // start recording
				var audioFile = await _captureFolder.CreateFileAsync("Audio.mp3", CreationCollisionOption.GenerateUniqueName);

				var encodingProfile = MediaEncodingProfile.CreateMp3(AudioEncodingQuality.Medium);

				await _mediaCapture.StartRecordToStorageFileAsync(encodingProfile, audioFile);

				_isRecording = true;
			}
		}
	}
}
