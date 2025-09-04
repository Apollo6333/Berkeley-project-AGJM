namespace Berkeley_project_AGJM
{
    internal class NodeManager
    {
        private static readonly int _randomTimeVariationMs = 10000; // +-5 seg

        private static List<Node> _nodes = [];
        private static Dictionary<int, int> _nodePorts = [];

        public static void StartNodes(int numNodes, int initialPort)
        {
            _nodes.Clear();
            _nodePorts.Clear();
            Random random = new();

            List<int> nodesToAdd = [];

            int lastId = numNodes;
            int coordinatorId = -1;
            for (int i = 1; i <= lastId; i++)
            {
                if (Helpers.IsPortAlreadyInUse(i + initialPort)) // Passa id sendo criado para frente se porta não estiver disponível
                {
                    lastId++;
                    continue;
                }

                if (coordinatorId == -1) coordinatorId = i;
                nodesToAdd.Add(i);
                _nodePorts.Add(i, i + initialPort); // Gera _nodePorts, que é usado para mensagens entre os nós
            }

            foreach (int id in nodesToAdd)
            {
                DateTime time = DateTime.Now;
                if (id != coordinatorId)
                {
                    // Variação inicial do tempo de cada nó
                    time = time.AddMilliseconds((random.NextDouble() * _randomTimeVariationMs) - (_randomTimeVariationMs / 2f));
                }

                _nodes.Add(new(id, id + initialPort, coordinatorId, _nodePorts, time)); // Cria cada nó
            }
        }
    }
}
