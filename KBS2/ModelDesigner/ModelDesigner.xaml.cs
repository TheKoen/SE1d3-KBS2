using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using KBS2.CarSystem;

namespace KBS2.ModelDesigner
{
    public partial class ModelDesigner
    {
        private CarDesign _currentDesign = null;
        private static IFormatter _formatter = new BinaryFormatter();
        
        public ModelDesigner()
        {
            InitializeComponent();

            ButtonExport.Click += (sender, args) => ExportDesign(_currentDesign, "exports/export.txt");
            ButtonImport.Click += (sender, args) =>
            {
                _currentDesign = ImportDesign("exports/export.txt");
                DesignDisplay.Source = _currentDesign?.Brush;
            };
            
            //_currentDesign = DesignFromModel(CarModel.Get("TestModel"));
            
            DesignDisplay.Source = _currentDesign?.Brush;
        }


        private static void ExportDesign(CarDesign design, string filename)
        {
            var stream = new FileStream(filename, FileMode.Create);
                
            _formatter.Serialize(stream, design);
            stream.Close();
        }

        private static CarDesign ImportDesign(string filename)
        {
            var stream = new FileStream(filename, FileMode.Open);
            return (CarDesign) _formatter.Deserialize(stream);
        }
        
        private static CarDesign DesignFromModel(CarModel model)
        {
            var newDesign = new CarDesign();
            foreach (var sensor in model.Sensors)
            {
                newDesign.AddSensor(sensor);
            }

            return newDesign;
        }
    }
}
