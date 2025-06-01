using System;
using System.Device.I2c;
using System.Numerics;

namespace Enviro;

/// <summary>
/// Represents a LSM303D 3D accelerometer and magnetometer sensor.
/// This sensor provides acceleration measurements in 3 axes and magnetic field measurements in 3 axes.
/// </summary>
public class Lsm303d : IDisposable
{
    #region Public member

    /// <summary>
    /// Gets the raw magnetometer readings in Gauss
    /// </summary>
    public Vector3 Magnetometer => _magnetometer;

    /// <summary>
    /// Gets the raw accelerometer readings in g (9.81 m/s²)
    /// </summary>
    public Vector3 Accelerometer => _accelerometer;

    /// <summary>
    /// Gets the tilt-compensated magnetometer readings
    /// </summary>
    public Vector3 TiltCompensatedMagnetometer => _tiltComp;

    /// <summary>
    /// Gets the raw heading in radians
    /// </summary>
    public float Heading => _heading;

    /// <summary>
    /// Gets the raw heading in degrees
    /// </summary>
    public float HeadingDegrees => _headingDegrees;

    /// <summary>
    /// Gets the tilt-compensated heading in radians
    /// </summary>
    public float TiltHeading => _tiltHeading;

    /// <summary>
    /// Gets the tilt-compensated heading in degrees
    /// </summary>
    public float TiltHeadingDegrees => _tiltHeadingDegrees;

    #endregion

    #region Private register definitions

    /// <summary>
    /// Temperature sensor output - low byte
    /// </summary>
    private const byte TEMP_OUT_L = 0x05;

    /// <summary>
    /// Temperature sensor output - high byte
    /// </summary>
    private const byte TEMP_OUT_H = 0x06;

    /// <summary>
    /// Status register for magnetometer
    /// </summary>
    private const byte STATUS_REG_M = 0x07;

    /// <summary>
    /// Magnetometer X-axis output - low byte
    /// </summary>
    private const byte OUT_X_L_M = 0x08;

    /// <summary>
    /// Magnetometer X-axis output - high byte
    /// </summary>
    private const byte OUT_X_H_M = 0x09;

    /// <summary>
    /// Magnetometer Y-axis output - low byte
    /// </summary>
    private const byte OUT_Y_L_M = 0x0A;

    /// <summary>
    /// Magnetometer Y-axis output - high byte</summary>
    private const byte OUT_Y_H_M = 0x0B;

    /// <summary>Magnetometer Z-axis output - low byte
    /// </summary>
    private const byte OUT_Z_L_M = 0x0C;

    /// <summary>
    /// Magnetometer Z-axis output - high byte
    /// </summary>
    private const byte OUT_Z_H_M = 0x0D;

    /// <summary>
    /// Device identification register
    /// </summary>
    private const byte WHO_AM_I = 0x0F;

    /// <summary>
    /// Control register 1 - Accelerometer settings
    /// </summary>
    private const byte CTRL_REG1 = 0x20;

    /// <summary>
    /// Control register 2 - Accelerometer scale
    /// </summary>
    private const byte CTRL_REG2 = 0x21;

    /// <summary>
    /// Control register 3 - Interrupt control
    /// </summary>
    private const byte CTRL_REG3 = 0x22;

    /// <summary>
    /// Control register 4 - Interrupt control
    /// </summary>
    private const byte CTRL_REG4 = 0x23;

    /// <summary>
    /// Control register 5 - Magnetometer settings
    /// </summary>
    private const byte CTRL_REG5 = 0x24;

    /// <summary>
    /// Control register 6 - Magnetometer scale
    /// </summary>
    private const byte CTRL_REG6 = 0x25;

    /// <summary>
    /// Control register 7 - Mode settings
    /// </summary>
    private const byte CTRL_REG7 = 0x26;

    /// <summary>
    /// Status register for accelerometer
    /// </summary>
    private const byte STATUS_REG_A = 0x27;

    /// <summary>
    /// Accelerometer X-axis output - low byte
    /// </summary>
    private const byte OUT_X_L_A = 0x28;

    /// <summary>
    /// Accelerometer X-axis output - high byte
    /// </summary>
    private const byte OUT_X_H_A = 0x29;

    /// <summary>
    /// Accelerometer Y-axis output - low byte
    /// </summary>
    private const byte OUT_Y_L_A = 0x2A;

    /// <summary>
    /// Accelerometer Y-axis output - high byte
    /// </summary>
    private const byte OUT_Y_H_A = 0x2B;

    /// <summary>
    /// Accelerometer Z-axis output - low byte
    /// </summary>
    private const byte OUT_Z_L_A = 0x2C;

    /// <summary>
    /// Accelerometer Z-axis output - high byte
    /// </summary>
    private const byte OUT_Z_H_A = 0x2D;

    #endregion

    #region Private scale definitions

    /// <summary>
    /// Magnetometer full-scale setting: +/- 2 Gauss
    /// </summary>
    private const byte MAG_SCALE_2 = 0x00;

    /// <summary>
    /// Magnetometer full-scale setting: +/- 4 Gauss
    /// </summary>
    private const byte MAG_SCALE_4 = 0x20;

    /// <summary>
    /// Magnetometer full-scale setting: +/- 8 Gauss
    /// </summary>
    private const byte MAG_SCALE_8 = 0x40;

    /// <summary>
    /// Magnetometer full-scale setting: +/- 12 Gauss
    /// </summary>
    private const byte MAG_SCALE_12 = 0x60;

    /// <summary>
    /// Accelerometer scale factor (+/- 2g)
    /// </summary>
    private const float ACCEL_SCALE = 2.0f;

    #endregion

    #region Private fields

    /// <summary>
    /// I2C communication interface
    /// </summary>
    private readonly I2cDevice _i2cDevice;

    /// <summary>
    /// Last read magnetometer values
    /// </summary>
    private Vector3 _magnetometer;

    /// <summary>
    /// Last read accelerometer values
    /// </summary>
    private Vector3 _accelerometer;

    /// <summary>
    /// Tilt-compensated magnetometer values
    /// </summary>
    private Vector3 _tiltComp;

    /// <summary>
    /// Raw heading in radians
    /// </summary>
    private float _heading;

    /// <summary>
    /// Raw heading in degrees
    /// </summary>
    private float _headingDegrees;

    /// <summary>
    /// Tilt-compensated heading in radians
    /// </summary>
    private float _tiltHeading;

    /// <summary>
    /// Tilt-compensated heading in degrees
    /// </summary>
    private float _tiltHeadingDegrees;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the LSM303D sensor.
    /// </summary>
    /// <param name="i2cDevice">The I2C device used for communication with the sensor.</param>
    /// <exception cref="ArgumentNullException">Thrown when i2cDevice is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when sensor initialization fails.</exception>
    public Lsm303d(I2cDevice i2cDevice)
    {
        _i2cDevice = i2cDevice ?? throw new ArgumentNullException(nameof(i2cDevice));

        Span<byte> readBuffer = stackalloc byte[1];

        _i2cDevice.WriteByte(WHO_AM_I);
        _i2cDevice.Read(readBuffer);
        if (readBuffer[0] != 0x49)
            throw new InvalidOperationException("No LSM303D detected!");

        // Initialize the sensor with the same settings as the Python implementation
        Span<byte> writeBuffer = stackalloc byte[2];

        // CTRL_REG1: ODR=50hz, all accel axes on
        writeBuffer[0] = CTRL_REG1;
        writeBuffer[1] = 0x57;
        _i2cDevice.Write(writeBuffer);

        // CTRL_REG2: set full scale +/- 2g
        writeBuffer[0] = CTRL_REG2;
        writeBuffer[1] = (byte)((3 << 6) | (0 << 3));
        _i2cDevice.Write(writeBuffer);

        // CTRL_REG3: no interrupt
        writeBuffer[0] = CTRL_REG3;
        writeBuffer[1] = 0x00;
        _i2cDevice.Write(writeBuffer);

        // CTRL_REG4: no interrupt
        writeBuffer[0] = CTRL_REG4;
        writeBuffer[1] = 0x00;
        _i2cDevice.Write(writeBuffer);

        // CTRL_REG5: mag 50Hz output rate + enable temp sensor
        writeBuffer[0] = CTRL_REG5;
        writeBuffer[1] = (byte)(0x80 | (4 << 2));
        _i2cDevice.Write(writeBuffer);

        // CTRL_REG6: Magnetic Scale +/- 2 Gauss
        writeBuffer[0] = CTRL_REG6;
        writeBuffer[1] = MAG_SCALE_2;
        _i2cDevice.Write(writeBuffer);

        // CTRL_REG7: continuous conversion mode
        writeBuffer[0] = CTRL_REG7;
        writeBuffer[1] = 0x00;
        _i2cDevice.Write(writeBuffer);
    }

    #endregion

    #region Public sensor reading methods

    /// <summary>
    /// Reads the temperature from the sensor.
    /// </summary>
    /// <returns>Temperature in degrees Celsius (uncalibrated).</returns>
    public float ReadTemperature()
    {
        Span<byte> buffer = stackalloc byte[2];

        _i2cDevice.WriteByte(TEMP_OUT_L);
        _i2cDevice.Read(buffer);

        return ((short)((buffer[1] << 8) | buffer[0])) / 8.0f;
    }

    /// <summary>
    /// Reads the magnetometer values from the sensor.
    /// </summary>
    /// <returns>Vector3 containing magnetic field strength in gauss for X, Y, and Z axes.</returns>
    public Vector3 ReadMagnetometer()
    {
        Span<byte> data = stackalloc byte[6];
        Span<byte> writeBuffer = stackalloc byte[1] { OUT_X_L_M | 0x80 }; // Set MSB to enable auto-increment

        // Write the register address with auto-increment bit set
        _i2cDevice.Write(writeBuffer);

        // Read all 6 bytes (X, Y, Z - low and high bytes)
        _i2cDevice.Read(data);

        // Combine low and high bytes for each axis
        short xRaw = (short)((data[1] << 8) | data[0]);
        short yRaw = (short)((data[3] << 8) | data[2]);
        short zRaw = (short)((data[5] << 8) | data[4]);

        // Values are already in gauss with the ±2 gauss scale we set in initialization
        _magnetometer = new Vector3(xRaw, yRaw, zRaw);

        return _magnetometer;
    }

    /// <summary>
    /// Reads the accelerometer values from the sensor.
    /// </summary>
    /// <returns>Vector3 containing acceleration in g's for X, Y, and Z axes.</returns>
    public Vector3 ReadAccelerometer()
    {
        Span<byte> data = stackalloc byte[6];
        Span<byte> writeBuffer = stackalloc byte[1] { OUT_X_L_A | 0x80 }; // Set MSB to enable auto-increment

        // Write the register address with auto-increment bit set
        _i2cDevice.Write(writeBuffer);

        // Read all 6 bytes (X, Y, Z - low and high bytes)
        _i2cDevice.Read(data);

        const float conversionFactor = ACCEL_SCALE / (float)(1 << 15);

        // Combine low and high bytes for each axis
        short xRaw = (short)((data[1] << 8) | data[0]);
        short yRaw = (short)((data[3] << 8) | data[2]);
        short zRaw = (short)((data[5] << 8) | data[4]);

        _accelerometer = new Vector3(
            xRaw * conversionFactor,
            yRaw * conversionFactor,
            zRaw * conversionFactor
        );

        return _accelerometer;
    }

    /// <summary>
    /// Calculates the raw heading based on magnetometer readings.
    /// </summary>
    /// <returns>Heading in degrees (0-360°).</returns>
    public float GetRawHeading()
    {
        _heading = (float)Math.Atan2(_magnetometer.X, _magnetometer.Y);

        if (_heading < 0)
            _heading += 2 * MathF.PI;
        if (_heading > 2 * MathF.PI)
            _heading -= 2 * MathF.PI;

        _headingDegrees = _heading * 180f / MathF.PI;
        return _headingDegrees;
    }

    /// <summary>
    /// Calculates tilt-compensated heading using both accelerometer and magnetometer data.
    /// </summary>
    /// <returns>Tilt-compensated heading in degrees (0-360°), or null if calculation fails.</returns>
    public float? GetTiltCompensatedHeading()
    {
        Update();

        try
        {
            var truncatedAccel = new Vector3(
                Math.Sign(_accelerometer.X) * Math.Min(Math.Abs(_accelerometer.X), 1.0f),
                Math.Sign(_accelerometer.Y) * Math.Min(Math.Abs(_accelerometer.Y), 1.0f),
                Math.Sign(_accelerometer.Z) * Math.Min(Math.Abs(_accelerometer.Z), 1.0f)
            );

            float pitch = (float)Math.Asin(-truncatedAccel.X);
            float roll = Math.Abs(Math.Cos(pitch)) >= Math.Abs(truncatedAccel.Y)
                ? (float)Math.Asin(truncatedAccel.Y / Math.Cos(pitch))
                : 0;

            _tiltComp = new Vector3(
                _magnetometer.X * (float)Math.Cos(pitch) + _magnetometer.Z * (float)Math.Sin(pitch),
                _magnetometer.X * (float)Math.Sin(roll) * (float)Math.Sin(pitch) +
                _magnetometer.Y * (float)Math.Cos(roll) -
                _magnetometer.Z * (float)Math.Sin(roll) * (float)Math.Cos(pitch),
                _magnetometer.X * (float)Math.Cos(roll) * (float)Math.Sin(pitch) +
                _magnetometer.Y * (float)Math.Sin(roll) +
                _magnetometer.Z * (float)Math.Cos(roll) * (float)Math.Cos(pitch)
            );

            _tiltHeading = (float)Math.Atan2(_tiltComp.Y, _tiltComp.X);

            if (_tiltHeading < 0)
                _tiltHeading += 2 * MathF.PI;
            if (_tiltHeading > 2 * MathF.PI)
                _tiltHeading -= 2 * MathF.PI;

            _tiltHeadingDegrees = _tiltHeading * 180f / MathF.PI;
            return _tiltHeadingDegrees;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Checks if new magnetometer data is available.
    /// </summary>
    /// <returns>True if new data is available, false otherwise.</returns>
    public bool IsMagnetometerReady()
    {
        Span<byte> buffer = stackalloc byte[1];
        _i2cDevice.WriteByte(STATUS_REG_M);
        _i2cDevice.Read(buffer);
        return (buffer[0] & 0x03) > 0;
    }

    /// <summary>
    /// Updates both accelerometer and magnetometer readings.
    /// Includes a delay to allow the sensor to stabilize between readings.
    /// </summary>
    public void Update()
    {
        ReadAccelerometer();
        ReadMagnetometer();

        // Allow some time for the sensor to stabilize
        Thread.Sleep(300);
    }

    #endregion

    #region Dispose Pattern

    /// <summary>
    /// Releases resources used by the sensor.
    /// </summary>
    public void Dispose()
    {
        _i2cDevice?.Dispose();
    }

    #endregion
}