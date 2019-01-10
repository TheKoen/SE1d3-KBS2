using System.Collections.Generic;
using System.Linq;
using KBS2.CitySystem;
using KBS2.Util;

namespace KBS2.CarSystem.Sensors.PassiveSensors
{
    public class EntityRadar : PassiveSensor
    {
        public EntityRadar(Car car, double range) : base(car, Direction.Global)
        {
            Range = range;
            Controller = new EntityRadarController(this);
        }

        public List<IEntity> EntitiesInRange { get; set; } = new List<IEntity>();
    }

    internal class EntityRadarController : SensorController
    {
        public EntityRadar Radar { get; }
        public EntityRadarController(EntityRadar radar) : base(radar)
        {
            Radar = radar;
        }
        

        /// <summary>
        /// Checks for <see cref="IEntity"/>s in range of the <see cref="EntityRadar"/>
        /// </summary>
        public override void Update()
        {
            if (Sensor.Car.CurrentRoad == null)
            {
                return;
            }

            Radar.EntitiesInRange = City.Instance.Controller.GetEntities()
                .FindAll(entity => entity.GetPoints().Any(point =>
                    MathUtil.Distance(point, Radar.Car.Location) < Radar.Range && !entity.Equals(Radar.Car))
                );
        }
    }
}