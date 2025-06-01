using System.Device.I2c;
using System.Device.Gpio;
using Iot.Device.Bmxx80;
using Iot.Device.Tcs3472x;
using Iot.Device.Ads1115;
using Enviro;

// Main program for reading and displaying sensor data from an Enviro pHAT board.
// Reads data from BMP280 (temperature/pressure), TCS3472 (color/light), LSM303D (accelerometer/magnetometer),
// and ADS1115 (analog inputs) sensors, displaying values every 10 seconds and toggling an LED.

try
{
    // Initialize GPIO for LED control
    const int LedPin = 4;  // BCM pin 4
    using var gpioController = new GpioController();
    gpioController.OpenPin(LedPin, PinMode.Output);
    gpioController.Write(LedPin, PinValue.Low);

    // Initialize BMP280 temperature/pressure sensor
    using I2cDevice i2cBmp280 = I2cDevice.Create(new I2cConnectionSettings(1, 0x77));
    using var bmp280 = new Bmp280(i2cBmp280)
    {
        TemperatureSampling = Sampling.UltraHighResolution,
        PressureSampling = Sampling.UltraHighResolution
    };

    // Initialize TCS3472 color sensor
    using I2cDevice i2cTcs3472 = I2cDevice.Create(new I2cConnectionSettings(1, 0x29));
    using var tcs3472x = new Tcs3472x(i2cTcs3472);

    // Initialize ADS1115 ADC
    using I2cDevice i2cAds1015 = I2cDevice.Create(new I2cConnectionSettings(1, 0x49));
    using var ads1015 = new Ads1115(i2cAds1015)
    {
        DataRate = DataRate.SPS128
    };

    // Initialize LSM303D magnetometer/accelerometer
    using I2cDevice i2cLsm303d = I2cDevice.Create(new I2cConnectionSettings(1, 0x1D));
    using var lsm303d = new Lsm303d(i2cLsm303d);

    Thread.Sleep(100); // Initial measurement delay

    var cycle = 0;
    while (true)
    {
        cycle++;
        Console.WriteLine($"Cycle #{cycle}");
        try
        {
            // Timestamp for readings
            Console.WriteLine($"Timestamp: {DateTime.Now.ToString("HH:mm:ss")}");
            Console.WriteLine("");

            // Read BMP280 sensor for temperature and pressure
            var bmp280Reading = bmp280.Read();
            Console.WriteLine($"BMP280 Sensor Readings:");
            Console.WriteLine($"    Temperature: {bmp280Reading.Temperature?.DegreesCelsius:F2}°C");
            Console.WriteLine($"    Pressure: {bmp280Reading.Pressure?.Atmospheres:F2} hPa");
            Console.WriteLine("");

            // Read TCS34732 sensor for color values
            var tcs3472Reading = tcs3472x.GetColor();
            Console.WriteLine($"TCS34732 Sensor Readings:");
            Console.WriteLine($"    R: 0x{tcs3472Reading.R:X2}, G: 0x{tcs3472Reading.G:X2}, 0xB: {tcs3472Reading.B:X2}");
            Console.WriteLine($"    Brightness: {tcs3472Reading.A}");
            Console.WriteLine("");

            // Read LSM303D sensor
            lsm303d.Update();
            var acceleration = lsm303d.Accelerometer;
            var magnetic = lsm303d.Magnetometer;
            Console.WriteLine($"LSM303D Sensor Readings:");
            Console.WriteLine($"    Acceleration: X: {acceleration.X:F2}g, Y: {acceleration.Y:F2}g, Z: {acceleration.Z:F2}g");
            Console.WriteLine($"    Magnetic (gauss): X: {magnetic.X:F2}, Y: {magnetic.Y:F2}, Z: {magnetic.Z:F2}");
            Console.WriteLine($"    Heading Heading: {lsm303d.HeadingDegrees:F2} °");
            Console.WriteLine("");

            // Read all ADC channels
            Console.WriteLine($"ADS1115 Readings:");
            for (int i = 0; i < 4; i++)
            {
                var mux = (InputMultiplexer)(i + 4);
                var voltage = ads1015.ReadVoltage(mux);
                Console.WriteLine($"    Channel #{i}: {voltage.Volts:F3}V");
            }
            Console.WriteLine("");

            // Reading cycle end
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("");
        }
        catch (Exception e)
        {
            // Log error and turn off LED
            Console.WriteLine($"Error reading sensors: {e.Message}");
            gpioController.Write(LedPin, PinValue.Low);
        }

        // Toggle LED state every cycle
        gpioController.Write(LedPin, cycle % 2 == 0 ? PinValue.High : PinValue.Low);

        //  10 seconds delay
        Thread.Sleep(10_000); //
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to initialize sensors: {ex.Message}");
    Environment.Exit(1);
}
