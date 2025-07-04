// ====================================================================================================
// Produced by the Free Edition of C++ to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-cplus-to-csharp.html
// ====================================================================================================

////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2021 Mateusz Malinowski
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
////////////////////////////////////////////////////////////////////////////////

namespace TestOrin.Icm20948
{
	/**
     * A structure to hold all data from IMU.
     */
	public class ImuData
	{
		/** The period at which IMU data is being pulled from the sensor. */
		public float UpdatePeriod { get; set; } = 0.0f;
        /** The temperature in Celsius degrees. */
        public float Temp { get; set; } = 0.0f;
        /** Raw angular rate from gyroscope in rad/s. */
        public float[] Gyro { get; } = [0.0f, 0.0f, 0.0f];
		/** Raw data from accelerometer in G's. */
		public float[] Acc { get; } = [0.0f, 0.0f, 0.0f];
        /** Raw data from compass in uT. */
        public float[] Mag { get; } = [0.0f, 0.0f, 0.0f];
        /** Quaternion representation of IMU orientation. */
        public float[] Quat { get; } = [1.0f, 0.0f, 0.0f, 0.0f];
        /** Euler angles representation of IMU orientation in degrees. */
        public float[] Angles { get; } = [0.0f, 0.0f, 0.0f];

	}
}