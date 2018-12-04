using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KBS2.CityDesigner
{
    /// <summary>
    /// Interaction logic for CityDesignerWindow.xaml
    /// </summary>
    public partial class CityDesignerWindow : Window
    {
        public CityDesignerWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // check 

            // save window
            var popupWindow = new SaveFileDialog();
            popupWindow.Title = "Save City";
            popupWindow.Filter = "XML file | *.xml";
            popupWindow.ShowDialog();
                       

        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            //load window
            var popupWindow = new OpenFileDialog();
            popupWindow.Title = "Load City";
            popupWindow.Filter = "XML file | *.xml";
            if(popupWindow.ShowDialog() == true)
            {
                //set the File name in textBox
                FileName.Text = System.IO.Path.GetFileNameWithoutExtension(popupWindow.FileName);
            }
        }

        private void MouseOnCanvasEventHandler(object sender, MouseEventArgs e)
        {
            PopupCanvas.IsOpen = true;

            PopupCanvas.HorizontalOffset = Mouse.GetPosition(this).X;
            PopupCanvas.VerticalOffset = Mouse.GetPosition(this).Y;
            Canvas.ToolTip = Mouse.GetPosition(this).ToString();
            
        }
    }
}
