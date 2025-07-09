using System;
using System.Threading.Tasks;
using Alchiweb.IoT.Icm20948;
using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Sensors.Motion;
using Meadow.Hardware;
using Silk.NET.Input;

namespace MeadowOrin
{
    public class MeadowApp : App<JetsonNano>
    {
        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            return base.Initialize();
        }

        public override async Task Run()
        {
            Resolver.Log.Info("Run...");

            Resolver.Log.Info("Hello, Jetson Nano!");

            var bus = Device.CreateI2cBus(7);
            var imu = new ImuIcm20948(new I2cCommunications(bus, 0x68));
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

            Resolver.Log.Info("Hello, Jetson Nano!");
            return;
        }
    }
}