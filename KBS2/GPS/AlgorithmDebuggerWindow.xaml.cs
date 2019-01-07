using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using KBS2.GPS.NodeNetwork;

namespace KBS2.GPS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal partial class AlgorithmDebuggerWindow
    {
        private static readonly Lazy<AlgorithmDebuggerWindow> Lazy =
            new Lazy<AlgorithmDebuggerWindow>(() => new AlgorithmDebuggerWindow());
        public static AlgorithmDebuggerWindow Instance => Lazy.Value;

        private readonly Dictionary<string, INetworkData> _networks =
            new Dictionary<string, INetworkData>();
        
        private AlgorithmDebuggerWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Adds a new <see cref="NodeNetworkCopy"/> to all windows so it can be reviewed
        /// </summary>
        public void AddNetworkResult(string name, NodeNetworkCopy network, Node from, Node to, Tuple<Node, Node> end)
        {
            _networks.Add(name, new NetworkData(network, from, to, end.Item1, end.Item2));
            Instance.Dispatcher.Invoke(() =>
            {
                Instance.ListBoxResults.Items.Add(name);
                Instance.ListBoxResults.Items.Add(new ListBoxItem{ Content = "", IsEnabled = false });
            });
        }

        /// <summary>
        /// Adds a step of the algorithm to all windows so it can be reviewed
        /// </summary>
        public void AddNetworkResult(string name, NodeNetworkCopy network, Node from, Node to)
        {
            _networks.Add(name, new AlgoStepData(network, from, to));
            Instance.Dispatcher.Invoke(() => { Instance.ListBoxResults.Items.Add(name); });
        }

        private void ListBoxResults_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxResults.SelectedValue is ListBoxItem) return;
            var item = (string) ListBoxResults.SelectedValue;
            if (item == string.Empty)
            {
                CanvasResult.Children.Clear();
                return;
            }
            
            var network = _networks[item];
            switch (network)
            {
                case NetworkData data:
                    UpdateView(data);
                    break;
                case AlgoStepData data:
                    UpdateView(data);
                    break;
            }
                
        }

        /// <summary>
        /// Updates the current view using a <see cref="NetworkData"/> object
        /// </summary>
        /// <param name="networkData">Object to use for updating</param>
        private void UpdateView(NetworkData networkData)
        {
            CanvasResult.Children.Clear();

            foreach (var link in networkData.Network.Links)
            {
                var line = new Line
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 6.0,
                    X1 = link.NodeA.PositionX, X2 = link.NodeB.PositionX,
                    Y1 = link.NodeA.PositionY, Y2 = link.NodeB.PositionY
                };
                CanvasResult.Children.Add(line);
            }

            var fromToLine = new Line
            {
                Stroke = Brushes.Goldenrod,
                StrokeThickness = 10.0,
                Opacity = 0.5,
                X1 = networkData.From.PositionX,    X2 = networkData.To.PositionX,
                Y1 = networkData.From.PositionY,    Y2 = networkData.To.PositionY
            };
            CanvasResult.Children.Add(fromToLine);

            var toEllipse = new Ellipse
            {
                Fill = Brushes.Goldenrod,
                Width = 28,
                Height = 28,
                Margin = new Thickness(networkData.To.PositionX - 14, networkData.To.PositionY - 14, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            CanvasResult.Children.Add(toEllipse);
            
            foreach (var node in networkData.Network.Nodes)
            {
                var ellipse = new Ellipse
                {
                    Stroke = Brushes.Blue,
                    StrokeThickness = 2.0,
                    Fill = Brushes.CornflowerBlue,
                    Width = 20,
                    Height = 20,
                    Margin = new Thickness(node.PositionX - 10, node.PositionY - 10, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };
                if (node.Equals(networkData.From))
                {
                    ellipse.Stroke = Brushes.Green;
                    ellipse.StrokeThickness = 4.0;
                }
                else if (node.Equals(networkData.End1) || node.Equals(networkData.End2))
                {
                    ellipse.Stroke = Brushes.DarkRed;
                    ellipse.StrokeThickness = 4.0;
                }
                CanvasResult.Children.Add(ellipse);
                
                var value = new TextBlock
                {
                    Text = $"{node.Value:F0}",
                    Margin = new Thickness(node.PositionX - 25, node.PositionY - 25, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };
                CanvasResult.Children.Add(value);
            }
        }
        
        /// <summary>
        /// Updates the current view using an <see cref="AlgoStepData"/> object
        /// </summary>
        /// <param name="networkData">Object to use for updating</param>
        private void UpdateView(AlgoStepData networkData)
        {
            CanvasResult.Children.Clear();

            foreach (var link in networkData.Network.Links)
            {
                var line = new Line
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 6.0,
                    X1 = link.NodeA.PositionX, X2 = link.NodeB.PositionX,
                    Y1 = link.NodeA.PositionY, Y2 = link.NodeB.PositionY
                };
                CanvasResult.Children.Add(line);
            }

            var fromToLine = new Line
            {
                Stroke = Brushes.MediumPurple,
                StrokeThickness = 10.0,
                Opacity = 0.5,
                X1 = networkData.From.PositionX,    X2 = networkData.To.PositionX,
                Y1 = networkData.From.PositionY,    Y2 = networkData.To.PositionY
            };
            CanvasResult.Children.Add(fromToLine);

            var toEllipse = new Ellipse
            {
                Fill = Brushes.MediumPurple,
                Width = 28,
                Height = 28,
                Margin = new Thickness(networkData.To.PositionX - 14, networkData.To.PositionY - 14, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            CanvasResult.Children.Add(toEllipse);
            
            foreach (var node in networkData.Network.Nodes)
            {
                var ellipse = new Ellipse
                {
                    Stroke = Brushes.Blue,
                    StrokeThickness = 2.0,
                    Fill = Brushes.CornflowerBlue,
                    Width = 20,
                    Height = 20,
                    Margin = new Thickness(node.PositionX - 10, node.PositionY - 10, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };
                CanvasResult.Children.Add(ellipse);
                
                var value = new TextBlock
                {
                    Text = $"{node.Value:F0}",
                    Margin = new Thickness(node.PositionX - 25, node.PositionY - 25, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };
                CanvasResult.Children.Add(value);
            }
        }



        private interface INetworkData
        {
            NodeNetworkCopy Network { get; }
        }

        private struct NetworkData : INetworkData
        {
            public NodeNetworkCopy Network { get; }
            public Node From { get; }
            public Node To { get; }
            public Node End1 { get; }
            public Node End2 { get; }

            public NetworkData(NodeNetworkCopy network, Node from, Node to, Node end1, Node end2)
            {
                Network = network;
                From = from;
                To = to;
                End1 = end1;
                End2 = end2;
            }
        }

        private struct AlgoStepData : INetworkData
        {
            public NodeNetworkCopy Network { get; }
            public Node From { get; }
            public Node To { get; }

            public AlgoStepData(NodeNetworkCopy network, Node from, Node to)
            {
                Network = network;
                From = from;
                To = to;
            }
        }
    }
}