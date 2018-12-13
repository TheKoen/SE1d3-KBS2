using System.Collections.Generic;
using Algorithms.NodeNetwork;

namespace AlgorithmDebugger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal partial class AlgorithmDebuggerWindow
    {
        private List<NodeNetworkCopy> _generatedNetworks;
        
        public AlgorithmDebuggerWindow(ref List<NodeNetworkCopy> networks)
        {
            InitializeComponent();

            _generatedNetworks = networks;
        }
    }
}