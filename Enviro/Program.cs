using System.Device.I2c;
using System.Device.Gpio;
using Iot.Device.Bmxx80;
using Iot.Device.Tcs3472x;
using Iot.Device.Ads1115;

try
{
    // Initialize GPIO for LED control
    const int LedPin = 4;  // BCM pin 4
    using var gpioController = new GpioController();
    gpioController.OpenPin(LedPin, PinMode.Output);
    gpioController.Write(LedPin, PinValue.Low); // Start with LED off
    
    // Initialize BMP280 temperature/pressure sensor
    using I2cDevice i2cBmp280 = I2cDevice.Create(new I2cConnectionSettings(1, 0x77));
    using var bmp280 = new Bmp280(i2cBmp280)
    {
        TemperatureSampling = Sampling.UltraHighResolution,
        PressureSampling = Sampling.UltraHighResolution
    };

    // Initialize TCS3472 color sensor
    using I2cDevice i2cTcs3472 = I2cDevice.Create(new I2cConnectionSettings(1, 0x29));
    using var colorSensor = new Tcs3472x(i2cTcs3472);

    // Initialize ADS1115 ADC
    using I2cDevice i2cAds1015 = I2cDevice.Create(new I2cConnectionSettings(1, 0x49));
    using var ads1015 = new Ads1115(i2cAds1015)
    {
        DataRate = DataRate.SPS128
    };

    Thread.Sleep(100); // Initial measurement delay

    while (true)
    {
        try
        {
            // Turn on LED to indicate sensor reading
            // and to enable to check readings with LEDs on and off.
            gpioController.Toggle(LedPin);
            
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
            var tcs3472Reading = colorSensor.GetColor();
            Console.WriteLine($"TCS34732 Sensor Readings:");
            Console.WriteLine($"    R: {tcs3472Reading.R:X2}, G: {tcs3472Reading.G:X2}, B: {tcs3472Reading.B:X2}");
            Console.WriteLine($"    Brightness: {tcs3472Reading.GetBrightness():F2}%");
            Console.WriteLine("");

            // Read all ADC channels
            Console.WriteLine($"ADS1115 Readings:");
            for (int i = 0; i < 4; i++)
            {
                // AIN_X_GND where X is 0-3
                var mux = (InputMultiplexer)(i + 4);
                var voltage = ads1015.ReadVoltage(mux);
                Console.WriteLine($"    Channel #{i}: {voltage.Volts:F3}V");
            }
            Console.WriteLine("");
        }
        catch (Exception e)
        {
            // Log error and turn off LED
            Console.WriteLine($"Error reading sensors: {e.Message}");
            gpioController.Write(LedPin, PinValue.Low);
        }

        Thread.Sleep(10_000); // 10 seconds delay
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to initialize sensors: {ex.Message}");
    Environment.Exit(1);
}
