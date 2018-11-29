using KBS2.CarSystem;

namespace KBS2.ModelDesigner
{
    public partial class ModelDesigner
    {
        public ModelDesigner()
        {
            InitializeComponent();
            
            var design = ImportModel(CarModel.Get("TestModel"));
            
            DesignDisplay.Source = design.Brush;
        }


        public CarDesign ImportModel(CarModel model)
        {
            var newModel = new CarDesign(this);
            foreach (var sensor in model.Sensors)
            {
                newModel.AddSensor(sensor);
            }

            return newModel;
        }
    }
}
