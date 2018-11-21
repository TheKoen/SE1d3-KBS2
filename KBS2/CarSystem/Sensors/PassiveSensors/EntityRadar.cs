using System.Collections.Generic;
using KBS2.CitySystem;
using KBS2.Util;

namespace KBS2.CarSystem.Sensors.PassiveSensors
{
    public class EntityRadar : PassiveSensor
    {
        /// <summary>
        ///     EntityRadar radar checks for entities in range
        /// </summary>
        /// <param name="range">range of the radar</param>
        public EntityRadar(Car car, double range) : base(car, Direction.Global)
        {
            Range = range;
            Controller = new EntityRadarController(this);
        }

        public List<IEntity> EntitiesInRange { get; set; } = new List<IEntity>();
    }

    /// <summary>
    ///     Controller for the EntityRadar
    /// </summary>
    internal class EntityRadarController : SensorController
    {
        public EntityRadarController(EntityRadar radar)
        {
            Radar = radar;
        }

        public EntityRadar Radar { get; set; }

        /// <summary>
        ///     Checks for entities in range of the radar
        /// </summary>
        public override void Update()
        {
            Radar.EntitiesInRange = City.Instance.Cars
                .FindAll(car =>
                    MathUtil.Distance(car.Location, Radar.Car.Location) < Radar.Range && !car.Equals(Radar.Car))
                .ConvertAll(car => (IEntity) car);
        }
    }
}