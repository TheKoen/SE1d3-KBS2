using System.Collections.Generic;
using Algorithms.NodeNetwork;

namespace AlgorithmDebugger
{
    public class DebuggerHandler
    {
        private readonly List<NodeNetworkCopy> generatedNetworks = new List<NodeNetworkCopy>();
        private AlgorithmDebuggerWindow DebuggerWindow { get; }

        public DebuggerHandler()
        {
            DebuggerWindow = new AlgorithmDebuggerWindow(ref generatedNetworks);
        }

        public void AddNodeNetworkCopy(NodeNetworkCopy network) =>
            generatedNetworks.Add(network);
    }
}