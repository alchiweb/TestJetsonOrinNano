using System.Device.I2c;
using Alchiweb.IoT.Icm20948;
// using Iot.Device.Camera;
// using Iot.Device.Camera.Settings;
using Iot.Device.Common;
using TestOrin.Media;
using Iot.Device.Media;
using Meadow.Hardware;

namespace TestOrin
{
    internal class Program
    {
        private const string CmdList = "list";
        private const string CmdStillLegacy = "still-legacy";
        private const string CmdVideoLegacy = "video-legacy";
        private const string CmdLapseLegacy = "lapse-legacy";
        private const string CmdStillLibcamera = "still-libcamera";
        private const string CmdVideoLibcamera = "video-libcamera";
        private const string CmdLapseLibcamera = "lapse-libcamera";
        static async Task Main(string[] args)
        {
            // var arg = "list";
            // ProcessSettings? processSettings = arg switch
            // {
            //     CmdList => ProcessSettingsFactory.CreateForLibcamerastillAndStderr(),
            //     CmdStillLegacy => ProcessSettingsFactory.CreateForRaspistill(),
            //     CmdVideoLegacy => ProcessSettingsFactory.CreateForRaspivid(),
            //     CmdLapseLegacy => ProcessSettingsFactory.CreateForRaspistill(),
            //     CmdStillLibcamera => ProcessSettingsFactory.CreateForLibcamerastill(),
            //     CmdVideoLibcamera => ProcessSettingsFactory.CreateForLibcameravid(),
            //     CmdLapseLibcamera => ProcessSettingsFactory.CreateForLibcamerastill(),
            //     _ => null,
            // };
            // await ProcessCamera(arg, processSettings);

             var camera = new Camera();

            IEnumerable<VideoPixelFormat> formats = camera.Device.GetSupportedPixelFormats();

            foreach (var format in formats)
            {
                Console.WriteLine($"Pixel Format {format}");
                IEnumerable<Resolution> resolutions = camera.Device.GetPixelFormatResolutions(format);
                if (resolutions is not null)
                {
                    foreach (var res in resolutions)
                    {
                        Console.WriteLine($"   min res: {res.MinWidth} x {res.MinHeight} ");
                        Console.WriteLine($"   max res: {res.MaxWidth} x {res.MaxHeight} ");
                    }
                }
            }

            // //camera.Device.DevicePath = "/dev/video";
            // camera.Device.Capture("/home/orinadmin/Videos/test2.jpg");


            //camera.Device.Settings.PixelFormat = PixelFormat.YUV420;

            // Get image stream, convert pixel format and save to file
            // MemoryStream ms = camera.Device..Capture().Capture();
            // Color[] colors = VideoDevice.Yv12ToRgb(ms, camera.Device.Settings.CaptureSize);
            // Bitmap bitmap = VideoDevice.RgbToBitmap(camera.Device.Settings.CaptureSize, colors);
            // bitmap.Save("/home/pi/yuyv_to_jpg.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

            // camera.StartCapture();
            // await Task.Delay(TimeSpan.FromMilliseconds(100));
            // camera.StopCapture();


            using (var i2cCommunications = new I2cCommunications(7, 0x68))
            {
                var imu = new ImuIcm20948(i2cCommunications);
                bool isCalib = false;

                if (await imu.Initialise())
                {
                    Console.WriteLine("Initialisation finie");
                    if (!isCalib)
                    {
                        await imu.CalibrateGyro();
                        Console.WriteLine("Calibration finie");
                    }
                    for (int i = 0; i < 20; i++)
                    {
                        var data = imu.ImuDataGet();
                        Console.WriteLine($"/------- counter: {i} ----- {data.UpdatePeriod}/");
                        Console.WriteLine($"\n \tAngle：Roll: {data.Angles[0]}     Pitch: {data.Angles[1]}     Yaw: {data.Angles[2]}");
                        Console.WriteLine($"\n Acceleration(g): X: {data.Acc[0]}     Y: {data.Acc[1]}     Z: {data.Acc[2]}");
                        Console.WriteLine($"\n Gyroscope(dps): X: {data.Gyro[0] * Conversion.RAD_TO_DEG}     Y: {data.Gyro[1] * Conversion.RAD_TO_DEG}     Z: {data.Gyro[2] * Conversion.RAD_TO_DEG}");
                        Console.WriteLine($"\n Magnetic(uT): X: {data.Mag[0]}     Y: {data.Mag[1]}     Z: {data.Mag[2]}");
                        Console.WriteLine($"\n Temperature(C): {data.Temp}");
                        await Task.Delay(TimeSpan.FromMilliseconds(500));
                    }
                }
            }


        }
        // static private async Task ProcessCamera(string arg, ProcessSettings? processSettings)
        // {
        //     if (processSettings == null)
        //         return;
        //     var capture = new Capture(processSettings);
        //     if (arg == CmdList)
        //     {
        //         var cams = await capture.List();
        //         Console.WriteLine("List of available cameras:");
        //         foreach (var cam in cams)
        //         {
        //             Console.WriteLine(cam);
        //         }

        //         return;
        //     }

        //     if (arg == CmdStillLegacy || arg == CmdStillLibcamera)
        //     {
        //         var filename = await capture.CaptureStill();
        //         Console.WriteLine($"Captured the picture: {filename}");

        //         return;
        //     }

        //     if (arg == CmdVideoLegacy || arg == CmdVideoLibcamera)
        //     {
        //         var filename = await capture.CaptureVideo();
        //         Console.WriteLine($"Captured the video: {filename}");

        //         return;
        //     }

        //     if (arg == CmdLapseLegacy || arg == CmdLapseLibcamera)
        //     {
        //         await capture.CaptureTimelapse();
        //         Console.WriteLine($"The time-lapse images have been saved to disk");

        //         return;
        //     }

        // }
    }
}
