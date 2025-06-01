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

## ❤️ More IoT projects of mine
I like to tinker around with Raspberry Pis, I created a couple of educational apps and scripts regarding the Pi and sensors - mostly from Pimoroni.

### .NET on Raspberry Pi 
- [dotnet-iot-raspberrypi-blinkt](https://github.com/tscholze/dotnet-iot-raspberrypi-blinkt)  
  A C# .NET implementation for controlling the Pimoroni Blinkt! LED board on a Raspberry Pi.
- [dotnet-iot-raspberrypi-enviro](https://github.com/tscholze/dotnet-iot-raspberrypi-enviro) A C# controller for the Pimoroini Enviro HAT with BMP, TCS and more sensors

### Windows 10 IoT Core apps
- [dotnet-iot-homebear-blinkt](https://github.com/tscholze/dotnet-iot-homebear-blinkt)  
  Windows 10 IoT Core UWP app that works great with the Pimoroni Blinkt! LED Raspberry Pi HAT.
- [dotnet-iot-homebear-tilt](https://github.com/tscholze/dotnet-iot-homebear-tilt)  
  Windows 10 IoT Core UWP app that works great with the Pimoroni Pan and Tilt HAT (PIC16F1503)
- [dotnet-iot-homebear-rainbow](https://github.com/tscholze/dotnet-iot-homebear-rainbow)  
  Windows 10 IoT Core UWP app that works great with the Pimoroni RainbowHAT
- [dotnet-iot-windowscommunity-sample-app](https://github.com/tscholze/dotnet-iot-windowscommunity-sample-app)  
  An UWP Windows 10 IoT Core sample app for windowscommunity.de

### Android Things apps
- [java-android-things-firebase-pager](https://github.com/tscholze/java-android-things-firebase-pager)
- An Android Things app that displays a Firebase Cloud Messaging notification on a alphanumeric segment control (Rainbow HAT)
- [java-android-things-tobot](https://github.com/tscholze/java-android-things-tobot)
- An Android Things an Google Assistant app to controll a Pimoroni STS vehicle by web and voice

## Python scripts
- [python-enviro-gdocs-logger](https://github.com/tscholze/python-enviro-gdocs-logger)
- Logs values like room temperature and more to a Google Docs Sheet with graphs
- [python-enviro-excel-online-logger](https://github.com/tscholze/python-enviro-excel-online-logger)
- Logs values like room temperature and more to a M365 Excel Sheet with graphs
- [python-enviro-azure-logger](https://github.com/tscholze/python-enviro-azure-logger)
- Logs values like room temperature and more to an Azure IoT Hub instance

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