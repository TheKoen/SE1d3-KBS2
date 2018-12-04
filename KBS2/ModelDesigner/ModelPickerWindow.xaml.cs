using System.Windows;
using KBS2.CarSystem;

namespace KBS2.ModelDesigner
{
    public partial class ModelPickerWindow
    {
        public CarModel SelectedModel { get; private set; }
        
        public bool Success { get; private set; }

        public ModelPickerWindow()
        {
            InitializeComponent();

            ListBoxModels.ItemsSource = CarModel.GetAll();

            Success = false;
        }

        private void ButtonOK_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListBoxModels.SelectedItem == null)
            {
                MessageBox.Show("Please select a model");
                return;
            }

            SelectedModel = (CarModel) ListBoxModels.SelectedItem;

            Success = true;
            Close();
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}