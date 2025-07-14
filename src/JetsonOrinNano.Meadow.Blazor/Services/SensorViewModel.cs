using Meadow;
using Meadow.Foundation.Sensors.Motion;
using Meadow.Foundation.Spatial;
using Meadow.Peripherals.Leds;
using Meadow.Units;

namespace JetsonOrinNano.Meadow.Blazor.Services
{
    public class SensorViewModel : IDisposable
    {
        private readonly Icm20948 _icm20948;

        public string AngleValue { get; private set; } = "-";
        public string AccelerationValue { get; private set; } = "-";
        public string GyroscopeValue { get; private set; } = "-";
        public string MagneticValue { get; private set; } = "-";
        public string TemperatureValue { get; private set; } = "-";

        public event Action? StateChanged;

        public SensorViewModel()
        {
            _icm20948 = Resolver.Services.Get<Icm20948>() ?? throw new Exception("BME680 not found");
            _icm20948.Updated += Icm20948Updated;
            _icm20948.StartUpdating(TimeSpan.FromSeconds(2));
        }

        private void Icm20948Updated(object? sender, IChangeResult<(
        Acceleration3D? Acceleration3D, AngularVelocity3D? AngularVelocity3D,
        MagneticField3D? MagneticField3D, Quaternion? QuaternionOrientation,
        EulerAngles? EulerOrientation, Temperature? Temperature)> e)
        {
            AngleValue = $"Roll: {e.New.EulerOrientation.Value.Roll}     Pitch: {e.New.EulerOrientation.Value.Pitch}     Yaw: {e.New.EulerOrientation.Value.Heading}";
            AccelerationValue = $"X: {e.New.Acceleration3D.Value.X}     Y: {e.New.Acceleration3D.Value.Y}     Z: {e.New.Acceleration3D.Value.Z}";
            MagneticValue = $"X: {e.New.MagneticField3D.Value.X}     Y: {e.New.MagneticField3D.Value.Y}     Z: {e.New.MagneticField3D.Value.Z}";
            TemperatureValue = $"{e.New.Temperature}°C";
            StateChanged?.Invoke();
        }

        public void Dispose()
        {
            _icm20948?.StopUpdating();
        }
    }
}