﻿namespace KBS2.CarSystem.Sensors
{
    public abstract class Sensor
    {
        public double Range { get; set; }
        public Direction SensorDirection { get; set; }
        public Car Car { get; set; }
        public SensorController Controller { get; set; }
    }
}