# Jetson Orin Nano with C#

Sample apps for the Jetson Orin Nano (but could works with minimal changes for other iot platforms, like for the Raspberry Pi for example).

The sensor used is the IMU sensor (ICM20948) with the dual camera IMX219-83 (https://fr.aliexpress.com/item/1005006368329357.html).

Documentation for C# for IoT: https://dotnet.microsoft.com/en-us/apps/iot

4 sample apps (with shared projects):
- console app with Meadow libraries
- console app with .NET IoT libraries (with Meadow wrapper, in order to reuse the same code)
- Blazor app with Meadow libraries
- Blazor app with .NET IoT libraries (with Meadow wrapper, in order to reuse the same code)
