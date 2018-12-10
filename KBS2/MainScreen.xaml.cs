using KBS2.CitySystem;
using KBS2.Console;
using KBS2.GPS;
using System.Windows;
using System.Xml;
using System.IO;
using KBS2.Util;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using KBS2.Util.Loop;
using CommandSystem;
using CommandSystem.PropertyManagement;
using KBS2.Visual.Controls;
using System;
using KBS2.CarSystem;
using KBS2.Visual;

namespace KBS2
{
    /// <summary>
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainScreen : Window
    {
        /*
         * A note on loops:
         * There's 3 different loops, a CommandLoop, a WPFLoop and an AILoop.
         * Make sure you understand what they are before subscribing to one!
         *
         * The CommandLoop is only used for the Console, and should NEVER be
         * used for anything else! Just pretend like it doesn't exist and
         * don't use it.
         *
         * The WPFLoop for anything related to the visuals. It's important
         * for a smooth interaface that this loop runs fast, so DON'T do
         * anything complicated or time-intensive on there.
         *
         * The AILoop is for any AI logic. This is where things like the
         * customers, car AI, car sensors, etc are supposed to run. Still
         * needs to run fast, but can more easily deal with irregular
         * refresh rates.
         */
        public static readonly TickLoop CommandLoop = new MainLoop("Command");
        public static readonly TickLoop WPFLoop = new MainLoop("Main");
        public static readonly TickLoop AILoop = new ThreadLoop("AI");

        private readonly ConsoleWindow consoleWindow;

        public CityRenderHandler CityRenderHandler { get; private set; }
        public CustomerRenderHandler CustomerRenderHandler { get; private set; }
        public CarRenderHandler CarRenderHandler { get; private set; }
        public SimulationControlHandler SimulationControlHandler { get; private set; }
        public PropertyDisplayHandler PropertyDisplayHandler { get; private set; }

        public int Ticks { get; set; }
        public double SecondsRunning { get; set; }

        public MainScreen()
        {
            consoleWindow = new ConsoleWindow();
            CommandLoop.Start();
            
            Initialized += (sender, args) => Initialize();
            InitializeComponent();
        }

        private void Initialize()
        {
            GPSSystem.Setup();

            CityRenderHandler = new CityRenderHandler(this, CanvasMain);
            CustomerRenderHandler = new CustomerRenderHandler(CanvasMain, this);
            CarRenderHandler = new CarRenderHandler(CanvasMain, this);
            SimulationControlHandler = new SimulationControlHandler(this);
            PropertyDisplayHandler = new PropertyDisplayHandler(this);

            WPFLoop.Subscribe(Update);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            consoleWindow.AllowClose = true;
            consoleWindow.Close();
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            SimulationControlHandler.SelectButtonClick();
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            SimulationControlHandler.LoadButtonClick();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            SimulationControlHandler.StartButtonClick();
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            SimulationControlHandler.PauseButtonClick();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            SimulationControlHandler.ResetButtonClick();
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnLoadResult_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnShow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnShowLatest_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSaveSim_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {

        }

        // Method for saving the new values the user has filled in in the Settings tab.
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            PropertyDisplayHandler.SaveProperties();
            PropertyDisplayHandler.UpdatePriceLabel(LabelSimulationPriceFormula);
        }

        private void BtnDefault_Click(object sender, RoutedEventArgs e)
        {
            PropertyDisplayHandler.ResetDefaults();
        }

        public void UpdateTimer()
        {
            LabelSimulationTime.Content = SecondsRunning;
        }

        public double GetSeconds()
        {
            return Math.Round(Ticks / 10d, 2);
        }

        public void Update()
        {
            Ticks++;
            SecondsRunning = GetSeconds();
            UpdateTimer();
        }

        private void BtnConsole_Click(object sender, RoutedEventArgs e)
        {
            if (consoleWindow.IsVisible)
            {
                consoleWindow.Hide();
            }
            else
            {
                consoleWindow.Show();
            }
        }
    }
}
