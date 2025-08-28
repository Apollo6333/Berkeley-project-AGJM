namespace Berkeley_project_AGJM
{
    internal class Node
    {
        private int _id;
        private int _port;
        private bool _isCoordinator;
        private DateTime _time;

        public Node(int id, int port, bool isCoordinator, DateTime time)
        {
            _id = id;
            _port = port;
            _isCoordinator = isCoordinator;
            _time = time;
        }
    }
}
