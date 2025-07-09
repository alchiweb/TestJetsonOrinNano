using Iot.Device.FtCommon;
using Iot.Device.Graphics;
using Iot.Device.Media;
using System.Drawing;
// using Iot.Device.Graphics.SkiaSharpAdapter;
// using SkiaSharp;

namespace TestOrin.Media;

public class Camera
{
    const string FILENAME = "/home/orinadmin/Videos/test5.jpg";
    public VideoConnectionSettings Settings { get; set; }
    public VideoDevice Device { get; set; }
    CancellationTokenSource _tokenSource = new CancellationTokenSource();

    public event VideoDevice.NewImageBufferReadyEvent NewImageReady
    {
        add { Device.NewImageBufferReady += value; }
        remove { Device.NewImageBufferReady -= value; }
    }


    public Camera()
    {

        // SkiaSharpAdapter.Register();
        Settings = new VideoConnectionSettings(
            busId: 0,
            captureSize: (1280 , 720 ),
            pixelFormat: VideoPixelFormat.SRGGB10
        );
        Device = VideoDevice.Create(Settings);
        Device.ImageBufferPoolingEnabled = true;
        //var bytes = Device.Capture();
        //var ms = new MemoryStream(bytes);  //);.AsMemory().Slice(0, bytes.Length).ToArray());
        //Color[] colors = VideoDevice.Nv12ToRgb(ms, Settings.CaptureSize);
        //var bitmap = VideoDevice.RgbToBitmap(Settings.CaptureSize, colors);
        ////Device.Settings.PixelFormat = PixelFormat.YUV420;

        //// Get image stream, convert pixel format and save to file
        //if (File.Exists(FILENAME))
        //    File.Delete(FILENAME);
        //bitmap.SaveToFile(FILENAME, ImageFileType.Jpg);



        //BitmapImage bitmap = BitmapImage.CreateFromStream(ms);
        // var bitmap = BitmapImage.CreateBitmap((int)Settings.CaptureSize.Width, (int)Settings.CaptureSize.Height, PixelFormat.Format32bppXrgb);
        // var skBitmap = SKBitmap.Decode((bytes);
        // SKImage image = SKImage.FromBitmap(skBitmap);
        // SKData encodedData = image.Encode(SKEncodedImageFormat.Png, 100);
        // var bitmapImageStream = File.Open(FILENAME,
        //                               FileMode.Create,
        //                               FileAccess.Write,
        //                               FileShare.None);
        // encodedData.SaveTo(bitmapImageStream);
        // bitmapImageStream.Flush(true);
        // bitmapImageStream.Dispose();

        // ImageSource imgSrc;
        // imgSrc = ImageSource.FromFile(imagePath);


        //         {
        //             //if (cancellationToken.IsCancellationRequested) return;
        //             if (File.Exists(FILENAME))
        //                 File.Delete(FILENAME);
        //             using FileStream stream = new FileStream(FILENAME, FileMode.CreateNew);
        // //            bitmap.SaveToStream(stream, ImageFileType.Jpg);
        //         }
        //     using var image = SKImage.FromEncodedData(bytes);
        //     PixelFormat pf;
        //     if (image.ColorType == SKColorType.Rgb888x)
        //     {
        //         pf = PixelFormat.Format32bppXrgb;
        //     }
        //     else if (image.ColorType == SKColorType.Bgra8888)
        //     {
        //         pf = PixelFormat.Format32bppArgb;
        //     }
        //     else
        //     {
        //         throw new NotSupportedException($"The stream contains an image with color type {image.ColorType}, which is currently not supported");
        //     }

        // MemoryStream ms2 = new();
        //image.SaveToStream(ms2, ImageFileType.Jpg);
        //bitmap.SaveToStream("/home/orinadmin/Videos/test3.jpg", ImageFileType.Jpg);
    }

    public void StartCapture()
    {
        if (!Device.IsOpen)
        {
            Device.StartCaptureContinuous();
        }

        if (!Device.IsCapturing)
        {
            new Thread(() =>
                {
                    Device.CaptureContinuous(_tokenSource.Token);
                }
            ).Start();
        }
    }

    public void StopCapture()
    {
        if (Device.IsCapturing)
        {
            _tokenSource.Cancel();
            _tokenSource = new CancellationTokenSource();
            Device.StopCaptureContinuous();
        }
    }
        public static Bitmap Rgb10ToBitmap((uint Width, uint Height) size, Color[] colors, System.Drawing.Imaging.PixelFormat format = System.Drawing.Imaging.PixelFormat.Format24bppRgb)
        {
            int width = (int)size.Width, height = (int)size.Height;
            // var newColors = new Color[colors.Length];
            // for(int i=0; i< colors.Length;  i++)
            // {
            //     newColors[i] = Color.FromArgb(colors[i].A,colors[i].R,colors[i].G,colors[i].B);
            // }
            Bitmap pic = new Bitmap(width, height, format);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pic.SetPixel(x, y, colors[y * width + x]);
                }
            }

            return pic;
        }
        public static Color[] RG10ToRgb(Stream stream, (uint Width, uint Height) size)
        {
            int r, g, b;
            uint width = size.Width, height = size.Height;
            uint total = width * height;
            int shift = 0;

            byte[] rg10 = new byte[stream.Length];
            byte[] rg10unit = new byte[17];
            // stream.Read(rg10, 0, stream.Length);

            List<Color> colors = new List<Color>();
        for (uint y = 0; y < height; y++)
        {
            for (uint x = 0; x < width; x++)
            {
            stream.Read(rg10unit, 0, 17);
                shift += 17;
                //shift = y / 2 * width + x - x % 2;
                // Console.WriteLine(string.Join(',',rg10unit.Select(b => b.ToString())));
                r = (rg10unit[0]);// + ((rg10unit[1] << 8) ) ;
                g = (rg10unit[2]);// + ((rg10unit[3] << 8)) ;
                b = (rg10unit[4]);// + ((rg10unit[5] << 8));
                // R_upper = (R >> 6) & 0x03;		// Least significant 2 bits
                // R_lower = (R >> 8) & 0xFF;		// Most significant 8 bits

                // G_upper = (G >> 6) & 0x03;		// Least significant 2 bits
                // G_lower = (G >> 8) & 0xFF;		// Most significant 8 bits

                // B_upper = (B >> 6) & 0x03;		// Least significant 2 bits
                // B_lower = (B >> 8) & 0xFF;	
                colors.Add(Color.FromArgb(g, r, b));
            }
            
        }

            return colors.ToArray();
        }
}
