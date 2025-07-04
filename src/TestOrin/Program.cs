using System.Device.I2c;
using TestOrin.Icm20948;

namespace MyApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using (var bus = I2cBus.Create(7)) // 1 for the Raspberry Pi 3B
            {
                using (var device = bus.CreateDevice(0x68)) // Address of Bright Pi
                using (var imu = new Icm20948(device))
                {
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
                    bus.RemoveDevice(0x68);
                }
            }
        }
    }
}
