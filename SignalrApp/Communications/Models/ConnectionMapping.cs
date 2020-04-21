using System.Collections.Generic;

namespace SignalrApp.Communications.Models
{
    public class ConnectionMapping
    {
        private readonly Dictionary<string, string> _connections = new Dictionary<string, string>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(string key, string connectionId)
        {
            lock (_connections)
            {
                if (!_connections.TryGetValue(key, out string connection))
                {
                    _connections.Add(key, connectionId);
                }
            }
        }

        public string GetConnections(string key)
        {
            if (_connections.TryGetValue(key, out string connectionId))
            {
                return connectionId;
            }
            return string.Empty;
        }

        public void Remove(string key, string connectionId)
        {
            lock (_connections)
            {
                if (!_connections.TryGetValue(key, out string connection))
                {
                    return;
                }
                _connections.Remove(key);
            }
        }

    }
}