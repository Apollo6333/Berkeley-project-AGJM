using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Berkeley_project_AGJM
{
    internal class Node
    {
        public enum MessageType
        {
            NEED_TIME_OFFSET, // Coordenador pede tempo aos outros nós
            SEND_TIME_OFFSET, // Nós enviam tempo ao coordenador
            FINAL_OFFSET_TIME // Coordenador manda offset de tempo para outros nós
        }

        private readonly int _id;
        private readonly int _port;
        private readonly int _coordinatorId;
        private Dictionary<int, int> _nodePorts = []; // Guarda <id, porta>, usado para mandar mensagens

        private DateTime _currentTime;
        private readonly Timer _timer;
        private static readonly int _timerUpdatePeriod = 20; // 50hz

        private UdpClient _udpClient;
        private Thread _listenThread;

        private Timer? _berkeleyStartTimer;
        private static readonly int _berkeleyStartDelayMs = 5000; // 5 seg

        private Dictionary<int, double> _receivedTimeOffsets = []; // Guarda <id, offset de tempo>

        private static readonly object _nodeLock = new();

        public Node(int id, int port, int coordinator, Dictionary<int, int> nodes, DateTime time)
        {
            _id = id;
            _port = port;
            _coordinatorId = coordinator;
            _nodePorts = new(nodes);

            _currentTime = time;
            _timer = new(TickTimer, null, _timerUpdatePeriod, _timerUpdatePeriod);

            _udpClient = new(port);
            _udpClient.Client.ReceiveTimeout = 100;
            _listenThread = new(ListenThread);
            _listenThread.Start();

            Helpers.Log(_id, _currentTime, $"Iniciando em <{_port}>" + (_coordinatorId == _id ? ", eu sou o coordenador" : ""), _coordinatorId == _id);

            if (coordinator == id) // Se é o coordenador, chamar o algoritmo de Berkeley depois de um delay
            {
                Helpers.Log(_id, _currentTime, $"Algoritmo de Berkeley programado para daqui a {_berkeleyStartDelayMs} ms", _coordinatorId == _id);
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
            _currentTime = _currentTime.AddMilliseconds(_timerUpdatePeriod);
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
            int sender = int.Parse(parts[0]);

            // Tipo da mensagem
            if (!Enum.TryParse(parts[1], out MessageType type))
            {
                Helpers.Log(_id, _currentTime, $"Nenhuma mensagem com tipo: {parts[1]}", _coordinatorId == _id, true);
                return;
            }

            // Dado enviado na mensagem (Se existe)
            string data = "";
            if (parts.Length > 2) data = parts[2];

            switch (type)
            {
                case MessageType.NEED_TIME_OFFSET:
                    DateTime parsedTime = DateTime.ParseExact(data, Helpers.timePattern, CultureInfo.InvariantCulture);
                    NeedTimeOffsetReceived(parsedTime);
                    break;
                case MessageType.SEND_TIME_OFFSET:
                    SendTimeOffsetReceived(sender, double.Parse(data.Replace(",", "."), CultureInfo.InvariantCulture));
                    break;
                case MessageType.FINAL_OFFSET_TIME:
                    FinalOffsetTimeReceived(double.Parse(data.Replace(",", "."), CultureInfo.InvariantCulture));
                    break;
            }
        }

        private void NeedTimeOffsetReceived(DateTime time)
        {
            Helpers.Log(_id, _currentTime, $"Coordenador requisitou diferença do tempo: {time.ToString(Helpers.timePattern)}, mandando...", _coordinatorId == _id);

            // Coordenador pediu tempo, enviando a ele
            Helpers.SendMessage(_nodePorts, _nodeLock, _id, _coordinatorId, _currentTime, MessageType.SEND_TIME_OFFSET, CalculateTimeOffsetMs(time, _currentTime).ToString());
        }

        private void SendTimeOffsetReceived(int senderId, double offset)
        {
            Helpers.Log(_id, _currentTime, $"Offset de tempo recebido de [{senderId}]: {offset}ms", _coordinatorId == _id);

            _receivedTimeOffsets.Add(senderId, offset);

            if (_receivedTimeOffsets.Count >= _nodePorts.Count) // Todos tempos recebidos, calcular média
            {
                BerkeleyFinalize();
            }
        }

        private void FinalOffsetTimeReceived(double ms)
        {
            _currentTime = _currentTime.AddMilliseconds(-ms);

            Helpers.Log(_id, _currentTime, $"Offset de tempo recebido e aplicado: {ms}ms", _coordinatorId == _id);
        }

        private void BerkeleyStart(object? state) // Inicia a algoritmo de Berkeley
        {
            Helpers.Log(_id, _currentTime, $"Iniciando sincronização de tempo com o algoritmo de Berkeley...", _coordinatorId == _id);

            _receivedTimeOffsets.Clear();

            foreach (int id in _nodePorts.Keys)
            {
                // Coordenador pede tempo a todos nós, incluindo a si mesmo
                Helpers.SendMessage(_nodePorts, _nodeLock, _id, id, _currentTime, MessageType.NEED_TIME_OFFSET, _currentTime.ToString(Helpers.timePattern));
            }
        }

        private void BerkeleyFinalize()
        {
            double averageTimeOffset = CalculateAverageTimeOffset();
            Helpers.Log(_id, _currentTime, $"Cálculo de offset de tempo médio terminado: {averageTimeOffset}ms, enviando...", _coordinatorId == _id);

            foreach (KeyValuePair<int, double> kvp in _receivedTimeOffsets)
            {
                // Enviar de volta diferença de tempo da média para todos nós, incluindo a si mesmo
                Helpers.SendMessage(_nodePorts, _nodeLock, _id, kvp.Key, _currentTime, MessageType.FINAL_OFFSET_TIME, (averageTimeOffset - kvp.Value).ToString());
            }
        }

        private double CalculateAverageTimeOffset()
        {
            double totalTimeOffset = 0;
            foreach (double receivedTimeOffset in _receivedTimeOffsets.Values)
            {
                totalTimeOffset += receivedTimeOffset;
            }
            double averageTimeOffset = totalTimeOffset / _receivedTimeOffsets.Count;

            return averageTimeOffset;
        }

        private static double CalculateTimeOffsetMs(DateTime mainTime, DateTime secondaryTime) {
            return mainTime.Subtract(secondaryTime).TotalMilliseconds;
        }
    }
}
