# 🌡️ .NET IoT Raspberry Pi Enviro pHAT

Welcome to a comprehensive environmental sensing journey with your Raspberry Pi! This project brings the versatile [Pimoroni Enviro pHAT](https://shop.pimoroni.com/products/enviro-phat) to life using C# and .NET, transforming your Pi into an environmental monitoring station.

## 🚀 What Does It Do?

With this .NET IoT implementation, you can:

- 🌡️ Monitor temperature and pressure with the BMP280 sensor
- 🎨 Detect colors and light levels with the TCS3472 sensor
- 🧭 Track motion and orientation with the LSM303D accelerometer/magnetometer
- ⚡ Measure analog inputs with the ADS1115 ADC
- 💡 Control the onboard LED for status indication

Perfect for environmental monitoring, motion detection, and IoT experimentation!

## 🔌 Hardware Requirements

- [Raspberry Pi](https://www.raspberrypi.org/) (any model with 40-pin GPIO)
- [Pimoroni Enviro pHAT](https://shop.pimoroni.com/products/enviro-phat)
- I²C enabled on your Raspberry Pi

## 🖥️ Software Requirements

- [.NET SDK](https://learn.microsoft.com/en-us/dotnet/iot/deployment) installed on your Raspberry Pi
- I²C interface enabled (`sudo raspi-config`)
- Git (for cloning the repository)

## 🏃‍♂️ How to Get Started

1. **Clone this repository:**
   ```bash
   git clone https://github.com/yourusername/dotnet-iot-raspberrypi-enviro.git
   cd dotnet-iot-raspberrypi-enviro
   ```

2. **Build and run:**
   ```bash
   dotnet run --project Enviro
   ```

## 📊 Features

- Real-time sensor data reading and display
- Automated sensor polling every 10 seconds
- LED status indication
- Error handling and graceful shutdown
- Clean, documented C# code following best practices

## 🔧 Implementation Details

The project implements drivers for:
- BMP280 temperature/pressure sensor
- TCS3472 color/light sensor
- LSM303D accelerometer/magnetometer
- ADS1115 analog-to-digital converter

Each sensor is properly initialized and configured for optimal performance.

## 🤝 Contributing

Contributions are welcome! Whether you want to add new features, improve documentation, or fix bugs, feel free to:

1. Fork the repository
2. Create a feature branch
3. Submit a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
Dependencies or assets may be licensed differently.

---

Happy monitoring! 📊✨