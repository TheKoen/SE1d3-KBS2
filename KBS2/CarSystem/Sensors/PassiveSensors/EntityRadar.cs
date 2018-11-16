using KBS2.CitySystem;
using KBS2.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBS2.CarSystem.Sensors.ActiveSensors
{
    public class EntityRadar : PassiveSensor
    {
        public List<IEntity> EntitiesInRange { get; set; } = new List<IEntity>();

        /// <summary>
        /// EntityRadar radar checks for entities in range
        /// </summary>
        /// <param name="range">range of the radar</param>
        public EntityRadar(double range)
        {
            Range = range;
            SensorDirection = Direction.Global;
            Controller = new EntityRadarController(this);
        }
    }

    /// <summary>
    /// Controller for the EntityRadar
    /// </summary>
    class EntityRadarController : SensorController
    {
        public EntityRadar Radar { get; set; }

        public EntityRadarController(EntityRadar radar)
        {
            Radar = radar;
        }

        /// <summary>
        /// Checks for entities in range of the radar
        /// </summary>
        public override void Update()
        {
            Radar.EntitiesInRange = City.Instance().Cars
                .FindAll(car => (VectorUtil.Distance(car.Location, Radar.Car.Location) < Radar.Range) && !car.Equals(Radar.Car))
                .ConvertAll(car => (IEntity)car);
        }
    }
}
