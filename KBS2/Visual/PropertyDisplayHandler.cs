using System.Linq;
using System.Windows.Controls;
using CommandSystem;
using CommandSystem.PropertyManagement;
using KBS2.GPS;

namespace KBS2.Visual
{
    public class PropertyDisplayHandler
    {
        private StackPanel PropertyPanel { get; }

        private int ticks;

        public PropertyDisplayHandler(MainScreen screen)
        {
            PropertyPanel = screen.StackPanelSettings;

            MainScreen.WPFLoop.Subscribe(Update);
            screen.Loaded += (sender, args) => UpdateProperties();
        }

        public void UpdateProperties()
        {
            PropertyPanel.Children.Clear();

            var properties = PropertyHandler.GetProperties();
            foreach (var property in properties)
            {
                var propertyName = property.Key;
                var propertyValue = property.Value.Value.ToString();

                PropertyPanel.Children.Add(new PropertySettings(propertyName, propertyValue));
            }
        }

        public void ResetDefaults()
        {
            PropertyHandler.ModifyProperty("main.tickRate", 30);
            PropertyHandler.ModifyProperty("command.tickRate", 30);
            PropertyHandler.ModifyProperty("startingPrice", 1.50);
            PropertyHandler.ModifyProperty("pricePerKilometer", 1.00);
            PropertyHandler.ModifyProperty("customerSpawnRate", 0.2f);
            PropertyHandler.ModifyProperty("availableCars", 10);
            PropertyHandler.ModifyProperty("customerCount", 10);
            PropertyHandler.ModifyProperty("globalSpeedLimit", -1);
            PropertyHandler.ModifyProperty("avgGroupSize", 10);

            UpdateProperties();
        }

        public void SaveProperties()
        {
            foreach (var child in PropertyPanel.Children)
            {
                var propertyControl = (PropertySettings)child;
                var name = propertyControl.LabelPropertyName.Content.ToString();
                var property = PropertyHandler.GetProperties().First(p => p.Key == name);

                if (propertyControl.TBCurrentValue.Text != property.Value.ToString())
                {
                    var value = propertyControl.TBCurrentValue.Text;
                    CommandHandler.HandleInput($"set { name } { value }");
                    propertyControl.CurrentValue = propertyControl.TBCurrentValue.Text;
                }
            }
        }

        public void UpdatePriceLabel(Label priceLabel)
        {
            var startingPrice = GPSSystem.StartingPrice.Value;
            var pricePerKilometer = GPSSystem.PricePerKilometer.Value;

            priceLabel.Content = $"€{startingPrice:0.00} + ({pricePerKilometer:0.00} * km)";
        }

        private void Update()
        {
            ticks++;
            if (ticks == 100)
            {
                ticks = 0;
                UpdateProperties();
            }
        }
    }
}