using System.Windows.Controls;
using KBS2.Console;

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

            var properties = CommandHandler.GetProperties();
            foreach (var property in properties)
            {
                var propertyName = property.Key;
                var propertyValue = property.Value.Value.ToString();

                PropertyPanel.Children.Add(new PropertySettings(propertyName, propertyValue));
            }
        }

        public void ResetDefaults()
        {
            CommandHandler.ModifyProperty("main.tickRate", 30);
            CommandHandler.ModifyProperty("command.tickRate", 30);
            CommandHandler.ModifyProperty("startingPrice", 1.50);
            CommandHandler.ModifyProperty("pricePerKilometer", 1.00);
            CommandHandler.ModifyProperty("customerSpawnRate", 0.2f);
            CommandHandler.ModifyProperty("availableCars", 10);
            CommandHandler.ModifyProperty("customerCount", 10);
            CommandHandler.ModifyProperty("globalSpeedLimit", -1);
            CommandHandler.ModifyProperty("avgGroupSize", 10);

            UpdateProperties();
        }

        private void Update()
        {
            ticks++;
            if (ticks == 50)
            {
                ticks = 0;
                UpdateProperties();
            }
        }
    }
}