namespace Berkeley_project_AGJM
{
    internal class NodeManager
    {
        private static readonly int _randomTimeVariationMs = 5000; // +-2500 ms

        private static List<Node>? _nodes;

        public static void StartNodes(int numNodes, int initialPort)
        {
            _nodes = [];
            Random random = new();

            for (int i = 1; i <= numNodes; i++)
            {
                DateTime time = DateTime.Now;
                if (i != 1)
                {
                    time = time.AddMilliseconds((random.NextDouble() * _randomTimeVariationMs) - (_randomTimeVariationMs / 2f));
                }

                _nodes.Add(new Node(i, i + initialPort, i == 1, time));
            }
        }
    }
}
