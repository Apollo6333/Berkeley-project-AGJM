namespace Berkeley_project_AGJM
{
    internal class NodeManager
    {
        private static readonly int _randomTimeVariationMs = 10000; // +-5000 ms

        private static List<Node> _nodes = [];
        private static Dictionary<int, int> _nodePorts = [];

        public static void StartNodes(int numNodes, int initialPort)
        {
            _nodes.Clear();
            _nodePorts.Clear();
            Random random = new();

            List<int> nodesToAdd = [];

            int lastId = numNodes;
            for (int i = 1; i <= lastId; i++)
            {
                if (Helpers.IsPortAlreadyInUse(i + initialPort))
                {
                    lastId++;
                    continue;
                }

                nodesToAdd.Add(i);
                _nodePorts.Add(i, i + initialPort); // Gera _nodePorts
            }

            for (int i = 1; i <= numNodes; i++)
            {
                DateTime time = DateTime.Now;
                if (i == 1)
                {
                    // Variação inicial do tempo de cada nó
                    time = time.AddMilliseconds((random.NextDouble() * _randomTimeVariationMs) - (_randomTimeVariationMs / 2f));
                }

                _nodes.Add(new(i, i + initialPort, i == 1, _nodePorts, time));
            }
        }
    }
}
