namespace Berkeley_project_AGJM
{
    internal class NodeManager
    {
        private static readonly int _randomTimeVariation;

        private static List<Node>? _nodes;

        public static void StartNodes(int numNodes, int initialPort)
        {
            _nodes = [];
            Random random = new();

            for (int i = 1; i <= numNodes; i++)
            {
                _nodes.Add(new Node(i, i + initialPort, i == 1, DateTime.Now));
                Thread.Sleep(random.Next() * 50);
            }
        }
    }
}
