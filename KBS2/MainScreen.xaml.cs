using KBS2.GPS;
using System.Windows;
using System.ComponentModel;
using KBS2.Util.Loop;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using KBS2.CitySystem;
using KBS2.Visual;
using KBS2.Database;
using KBS2.Util;
using System.IO;
using System.Security;
using System.Windows.Forms;
using CommandSystem.Exceptions;
using MessageBox = System.Windows.MessageBox;

namespace KBS2
{
    /// <summary>
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainScreen : Window
    {
        /*
         * A note on loops:
         * There's 5 different loops, a CommandLoop, a WPFLoop, a DrawingLoop,
         * a ZoomLoop and an AILoop.
         * Make sure you understand what they are before subscribing to one!
         *
         * The CommandLoop is mostly used for the Console and some other
         * miscellaneous tasks like handling data that came back from the
         * database.
         *
         * The WPFLoop for a lot of things related to the visuals. It's
         * importantfor a smooth interaface that this loop runs fast, so
         * DON'T doanything complicated or time-intensive on there.
         *
         * The DrawingLoop is for drawing the entities (roads, cars, etc)
         * on the canvas. The DrawingLoop gets it's data from the ZoomLoop.
         *
         * The ZoomLoop handles the calculations for the position and scale
         * of the entities and sends that data over to the DrawingLoop for
         * rendering.
         *
         * The AILoop is for any AI logic. This is where things like the
         * customers, car AI, car sensors, etc are supposed to run. Still
         * needs to run fast, but can more easily deal with irregular
         * refresh rates.
         */
        public static readonly TickLoop CommandLoop = new MainLoop("CMD");
        public static readonly TickLoop WPFLoop = new MainLoop("WPF");
        public static readonly TickLoop DrawingLoop = new MainLoop("DRW", 60);
        public static readonly TickLoop ZoomLoop = new ThreadLoop("ZOOM", 60);
        public static readonly TickLoop AILoop = new ThreadLoop("AI");

        private readonly ConsoleWindow consoleWindow;
        private readonly ModelDesigner.ModelDesigner modelDesigner;
        private readonly CityDesigner.CityDesignerWindow cityDesigner;
        #if DEBUG
        private readonly AlgorithmDebuggerWindow algorithmDebuggerWindow;
        #endif
        private readonly Stopwatch Stopwatch = new Stopwatch();

        public CityRenderHandler CityRenderHandler { get; private set; }
        public CustomerRenderHandler CustomerRenderHandler { get; private set; }
        public CarRenderHandler CarRenderHandler { get; private set; }
        public SimulationControlHandler SimulationControlHandler { get; private set; }
        public PropertyDisplayHandler PropertyDisplayHandler { get; private set; }
        public ZoomHandler ZoomHandler { get; private set; }

        public int Ticks { get; set; }

        public float Zoom { get; set; } = 1.0F;

        public MainScreen()
        {
            consoleWindow = new ConsoleWindow();
            modelDesigner = new ModelDesigner.ModelDesigner();
            cityDesigner = new CityDesigner.CityDesignerWindow();
            #if DEBUG
            algorithmDebuggerWindow = AlgorithmDebuggerWindow.Instance;
            algorithmDebuggerWindow.Show();
            #endif
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

            DrawingLoop.Start();
            ZoomLoop.Start();

            WPFLoop.Subscribe(Update);
            
            CommandLoop.Subscribe(CmdUpdate);

            PreviewMouseWheel += ZoomHandler.Scroll;
            ResultImport.ResultImported += ResultImportComplete;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            consoleWindow.AllowClose = true;
            consoleWindow.Close();
            modelDesigner.AllowClose = true;
            modelDesigner.Close();
            cityDesigner.AllowClose = true;
            cityDesigner.Close();
            #if DEBUG
            algorithmDebuggerWindow.Close();
            #endif
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
            try
            {
                ResultImport.SetPath();
                TBResult.Text = Path.GetFileNameWithoutExtension(ResultImport.Path);
            }
            catch(Exception b)
            {
                MessageBox.Show(this, b.Message, "Import error", MessageBoxButton.OK);
            }
        }

        private void BtnLoadResult_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResultImport.ImportResult(this);
            }
            catch(Exception b)
            {
                MessageBox.Show(this, b.Message, "Import error", MessageBoxButton.OK);
            }
        }

        private void BtnShow_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TBSimID.Text, out var id))
            {
                DatabaseHelper.QueueDatabaseRequest(
                    database => (from sim in database.Simulations
                                 where sim.ID == id
                                 select sim).ToList(),
                    data =>
                    {
                        if (data.Count > 0)
                        {
                            var simulation = data.First();
                            SimulationControlHandler.Results.Instance = simulation.CityInstance;
                            SimulationControlHandler.Results.Update();
                            MessageBox.Show($"Loaded data for simulation {id}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show($"Unknown simulation ID {id}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            TBSimID.Text = "";
                        }
                    },
                    CommandLoop
                );
            }
        }

        private void BtnShowLatest_Click(object sender, RoutedEventArgs e)
        {
            DatabaseHelper.QueueDatabaseRequest(
                database => (from sim in database.Simulations
                             select sim).ToList(),
                data =>
                {
                    if (data.Count > 0)
                    {
                        var id = data.Max(sim => sim.ID);
                        var simulation = data.First(sim => sim.ID == id);
                        SimulationControlHandler.Results.Instance = simulation.CityInstance;
                        SimulationControlHandler.Results.Update();
                        TBSimID.Text = id.ToString();
                        MessageBox.Show($"Loaded data for simulation {id}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show($"No simulations found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        TBSimID.Text = "";
                    }
                },
                CommandLoop
            );
        }

        private void ResultImportComplete(object source, EventArgs args)
        {
            var id = ((ImportEventArgs) args).SimID;
            DatabaseHelper.QueueDatabaseRequest(
                database => (from sim in database.Simulations
                    select sim).ToList(),
                data =>
                {
                    if (data.Count > 0)
                    {
                        var simulation = data.First(sim => sim.ID == id);
                        SimulationControlHandler.Results.Instance = simulation.CityInstance;
                        SimulationControlHandler.Results.Update();
                        TBSimID.Text = id.ToString();
                        MessageBox.Show($"Loaded data for simulation {id}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show($"No simulations found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        TBSimID.Text = "";
                    }
                },
                CommandLoop
            );
        }

        private void BtnSaveSim_Click(object sender, RoutedEventArgs e)
        {

            int cityInstanceId = 0;

            try
            {
                cityInstanceId = SimulationControlHandler.Results.Instance.ID;
            }
            catch(Exception)
            {
                MessageBox.Show(this, "Please give some information to save.", "Export", MessageBoxButton.OK);
                return;
            }
            DatabaseHelper.QueueDatabaseRequest(
                database => (from sim in database.Simulations
                             where sim.CityInstance.ID == cityInstanceId
                             select sim).ToList(),
                data => 
                {
                   try
                   {
                        ResultExport.ExportResult(data.First().ID, "killakid", this);
                   }
                   catch(Exception b)
                   {
                        MessageBox.Show(this, b.Message, "Export error", MessageBoxButton.OK);
                   }
                }, 
                CommandLoop
            );
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            var popupWindow = new SaveFileDialog()
            {
                Title = "Save Log",
                Filter = "TXT file | *.txt"
            };
            popupWindow.ShowDialog();

            if (string.IsNullOrWhiteSpace(popupWindow.FileName))
            {
                return;
            }

            // Trying to write to a file
            try
            {
                using (var sw = File.CreateText(popupWindow.FileName))
                {
                    foreach (var line in App.Console.GetOutputHistory())
                    {
                        sw.WriteLine(line);
                    }
                }
            }
            catch (SystemException se)
                when (se.GetType() == typeof(UnauthorizedAccessException) ||
                      se.GetType() == typeof(SecurityException))
            {
                MessageBox.Show(this, $"Accessn't file \"{popupWindow.FileName}\"", "Save error", MessageBoxButton.OK);
            }
            catch (ArgumentException)
            {
                MessageBox.Show(this, "Can't write to a system device", "Save error", MessageBoxButton.OK);
            }
            catch (IOException ioe)
                when (ioe.GetType() == typeof(PathTooLongException) ||
                      ioe.GetType() == typeof(DirectoryNotFoundException))
            {
                MessageBox.Show(this, $"Invalid path \"{popupWindow.FileName}\"", "Save error", MessageBoxButton.OK);
            }
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

        public void Update()
        {
            Ticks++;
            UpdateTimer();
        }

        public void CmdUpdate()
        {
            if (CitySystem.City.Instance == null) return;

            var maxWidth = 0.0;
            var maxHeight = 0.0;
            foreach (var road in CitySystem.City.Instance.Roads)
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
            if (cityDesigner.IsVisible)
            {
                cityDesigner.Hide();
            }
            else
            {
                cityDesigner.Show();
            }
        }

        private void Zoom_Changed(object sender, SelectionChangedEventArgs e)
        {
            ZoomHandler?.ZoomBoxChanged();
        }

        private void BtnRefresh_OnClick(object sender, RoutedEventArgs e)
        {
            SimulationControlHandler.Results.Update();
        }
    }
}
