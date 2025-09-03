using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Berkeley_project_AGJM
{
    internal class Node
    {
        public enum MessageType
        {
            GIVE_TIME, // Coordenador pede tempo aos outros nós
            RECEIVE_TIME, // Nós enviam tempo ao coordenador
            OFFSET_TIME // Coordenador manda offset de tempo para outros nós
        }

        private readonly int _id;
        private readonly int _port;
        private readonly bool _isCoordinator;
        private Dictionary<int, int> _nodePorts = []; // Guarda <id, porta>, usado para mandar mensagens

        private DateTime _currentTime;
        private readonly Timer _timer;

        private UdpClient _udpClient;
        private Thread _listenThread;

        private Timer? _berkeleyStartTimer;
        private readonly int _berkeleyStartDelayMs = 5000; // 5 seg

        private static readonly object _nodeLock = new();

        public Node(int id, int port, bool isCoordinator, Dictionary<int, int> nodes, DateTime time)
        {
            _id = id;
            _port = port;
            _isCoordinator = isCoordinator;
            _nodePorts = new(nodes);

            _currentTime = time;
            _timer = new(TickTimer, null, 1, 1);

            _udpClient = new(port);
            _udpClient.Client.ReceiveTimeout = 100;
            _listenThread = new(ListenThread);
            _listenThread.Start();

            AnnounceStart();

            if (isCoordinator) // Se é o coordenador, chamar o algoritmo de Berkeley depois de 5 segundos
            {
                _berkeleyStartTimer = new(BerkeleyStart, null, _berkeleyStartDelayMs, Timeout.Infinite);
            }
        }

        // Thread que escuta e processa mensagens
        private void ListenThread()
        {
            while (true) ListenAndProcessMessages();
        }

        private void TickTimer(object? state)
        {
            _currentTime = _currentTime.AddMilliseconds(1);
        }

        private void AnnounceStart()
        {
            Helpers.Log(_id, _currentTime, $"Iniciando em <{_port}>" + (_isCoordinator ? ", eu sou o coordenador" : ""), _isCoordinator);
        }

        private void ListenAndProcessMessages()
        {
            string message;
            try
            {
                IPEndPoint? remote = null;
                byte[] rawData = _udpClient.Receive(ref remote);
                message = Encoding.UTF8.GetString(rawData);
            }
            catch (SocketException) { return; }

            string[] parts = message.Split('|');

            // Id de quem enviou a mensagem
            string sender = parts[0];

            // Tipo da mensagem
            if (!Enum.TryParse(parts[1], out MessageType type))
            {
                Helpers.Log(_id, _currentTime, $"Nenhuma mensagem com tipo: {parts[1]}", true);
                return;
            }

            // Dado enviado na mensagem (Se existe)
            string data = "";
            if (parts.Length > 2) data = parts[2];

            switch (type)
            {
                case MessageType.GIVE_TIME:
                    GiveTime();
                    break;
                case MessageType.RECEIVE_TIME:
                    ReceiveTime();
                    break;
                case MessageType.OFFSET_TIME:
                    OffsetTime();
                    break;
            }
        }

        private void GiveTime()
        {
            // TO-DO
        }

        private void ReceiveTime()
        {
            // TO-DO
        }

        private void OffsetTime()
        {
            // TO-DO
        }

        private void BerkeleyStart(object? state) // Inicia a algoritmo de Berkeley
        {
            foreach (int id in _nodePorts.Keys)
            {
                if (id == _id) continue;

                // Coordenador pede tempo a todos outros nós
                Helpers.SendMessage(_nodePorts, _nodeLock, _id, id, _currentTime, MessageType.GIVE_TIME);
            }
        }
    }
}
