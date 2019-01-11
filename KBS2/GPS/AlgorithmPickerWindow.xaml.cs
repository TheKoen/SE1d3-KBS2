using System;
using System.Windows;

namespace KBS2.GPS
{
    public partial class AlgorithmPickerWindow
    {
        public Tuple<string, IAlgorithm> SelectedAlgorithm { get; private set; }
        
        public bool Success { get; private set; }
        
        public AlgorithmPickerWindow()
        {
            InitializeComponent();

            ListBoxModels.ItemsSource = GPSSystem.AlgorithmList;

            Success = false;
        }

        private void ButtonOK_OnClick(object sender, RoutedEventArgs e)
        {
            if (ListBoxModels.SelectedItem == null)
            {
                MessageBox.Show("Please select an algorithm");
                return;
            }

            SelectedAlgorithm = (Tuple<string, IAlgorithm>) ListBoxModels.SelectedItem;

            Success = true;
            Close();
        }

        private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
