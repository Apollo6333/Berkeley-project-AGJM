namespace Berkeley_project_AGJM
{
    internal class Node
    {
        private readonly int _id;
        private readonly int _port;
        private readonly bool _isCoordinator;
        private DateTime _time;

        public Node(int id, int port, bool isCoordinator, DateTime time)
        {
            _id = id;
            _port = port;
            _isCoordinator = isCoordinator;
            _time = time;
            
            AnnounceStart();
        }

        private void AnnounceStart()
        {
            Helpers.Log(_id, _time, $"Iniciando em <{_port}>" + (_isCoordinator ? ", eu sou o coordenador" : ""), _isCoordinator);
        }
    }
}
