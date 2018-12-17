using KBS2.GPS;
using System.Windows;
using System.ComponentModel;
using KBS2.Util.Loop;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using KBS2.CitySystem;
using KBS2.Visual;
using KBS2.Database;

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
        public static readonly TickLoop CommandLoop = new MainLoop("CMD", 60);
        public static readonly TickLoop WPFLoop = new MainLoop("WPF");
        public static readonly TickLoop AILoop = new ThreadLoop("AI");

        private readonly ConsoleWindow consoleWindow;
        private readonly ModelDesigner.ModelDesigner modelDesigner;
        private readonly Stopwatch Stopwatch = new Stopwatch();

        public CityRenderHandler CityRenderHandler { get; private set; }
        public CustomerRenderHandler CustomerRenderHandler { get; private set; }
        public CarRenderHandler CarRenderHandler { get; private set; }
        public SimulationControlHandler SimulationControlHandler { get; private set; }
        public PropertyDisplayHandler PropertyDisplayHandler { get; private set; }
        public ZoomHandler ZoomHandler { get; private set; }

        public int Ticks { get; set; }
        public double SecondsRunning { get; set; }

        public float Zoom { get; set; } = 1.0F;

        public MainScreen()
        {
            consoleWindow = new ConsoleWindow();
            modelDesigner = new ModelDesigner.ModelDesigner();
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
            ZoomHandler = new ZoomHandler(this);

            WPFLoop.Subscribe(Update);

            using (var context = new myDatabase("killakid")) {
                var City = new KBS2.Database.City { CityName = "BedenkWatLeuks" };
                context.Cities.Add(City);
                context.SaveChanges();
            } 
            CommandLoop.Subscribe(CmdUpdate);

            PreviewMouseWheel += ZoomHandler.Scroll;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            consoleWindow.AllowClose = true;
            consoleWindow.Close();
            modelDesigner.AllowClose = true;
            modelDesigner.Close();
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
            Stopwatch.Start();
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            SimulationControlHandler.PauseButtonClick();
            Stopwatch.Stop();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            SimulationControlHandler.ResetButtonClick();
            LabelSimulationTime.Content = "00:00:00";
            Stopwatch.Reset();
            
            CommandLoop.Register();
            WPFLoop.Register();
            AILoop.Register();
            GPSSystem.Setup();
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

        /// <summary>
        /// Method for the save button with saving the new values the user has filled in in the Settings tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            PropertyDisplayHandler.SaveProperties();
            PropertyDisplayHandler.UpdatePriceLabel(LabelSimulationPriceFormula);
        }

        private void BtnDefault_Click(object sender, RoutedEventArgs e)
        {
            PropertyDisplayHandler.ResetDefaults();
        }

        /// <summary>
        /// This method updates the timer of the simulation
        /// </summary>
        public void UpdateTimer()
        {
            var time = Stopwatch.ElapsedMilliseconds;
            var temp = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(time);
            LabelSimulationTime.Content = $"{temp:mm:ss:FF}";
        }

        /// <summary>
        /// This method is calculating seconds from ticks
        /// </summary>
        /// <returns>seconds the simulation is running</returns>
        public double CalculateSeconds()
        {
            return Ticks / 30.0d;
        }

        public void Update()
        {
            Ticks++;
            SecondsRunning = CalculateSeconds();
            UpdateTimer();
        }

        public void CmdUpdate()
        {
            if (City.Instance == null) return;

            var maxWidth = 0.0;
            var maxHeight = 0.0;
            foreach (var road in City.Instance.Roads)
            {
                if (road.Start.X * Zoom > maxWidth) maxWidth = road.Start.X * Zoom;
                if (road.End.X * Zoom > maxWidth) maxWidth = road.End.X * Zoom;
                if (road.Start.Y * Zoom > maxHeight) maxHeight = road.Start.Y * Zoom;
                if (road.End.Y * Zoom > maxHeight) maxHeight = road.End.Y * Zoom;
            }

            CanvasMain.Width = Math.Max(maxWidth + 400, Width - 50);
            CanvasMain.Height = Math.Max(maxHeight + 50, Height - 50);
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

        private void BtnCarModelMaker_Click(object sender, RoutedEventArgs e)
        {
            if (consoleWindow.IsVisible)
            {
                modelDesigner.Hide();
            }
            else
            {
                modelDesigner.Show();
            }
        }

        private void BtnCityMaker_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Zoom_Changed(object sender, SelectionChangedEventArgs e)
        {
            ZoomHandler?.ZoomBoxChanged();
        }
    }
}
