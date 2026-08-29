using System;
using System.Threading.Tasks;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class SafePlcAdapter : IDisposable
    {
        private readonly IpConnectionClient _client;

        public event EventHandler<string> StatusChanged;
        public event EventHandler<IpDataMessageModel> DataReceived;
        public event EventHandler<Exception> ConnectionError;

        public SafePlcAdapter()
            : this(new IpConnectionClient())
        {
        }

        public SafePlcAdapter(IpConnectionClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            _client = client;
            _client.StatusChanged += Client_StatusChanged;
            _client.DataReceived += Client_DataReceived;
            _client.ConnectionError += Client_ConnectionError;
        }

        public bool IsConnected
        {
            get { return _client.IsConnected; }
        }

        public bool WritesEnabled { get; private set; }

        public Task ConnectAsync(string host, int port)
        {
            return _client.ConnectAsync(host, port);
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }

        public void SetWritesEnabled(bool enabled, string reason)
        {
            if (enabled && string.IsNullOrWhiteSpace(reason))
            {
                throw new InvalidOperationException("A confirmation reason is required before enabling PLC writes.");
            }

            WritesEnabled = enabled;
            RaiseStatus(enabled ? "PLC writes enabled: " + reason : "PLC writes disabled.");
        }

        public Task SendTextAsync(string text)
        {
            if (!WritesEnabled)
            {
                throw new InvalidOperationException("PLC writes are disabled. Confirm protocol, addresses, scaling, and write permissions before enabling writes.");
            }

            return _client.SendTextAsync(text);
        }

        public void Dispose()
        {
            _client.StatusChanged -= Client_StatusChanged;
            _client.DataReceived -= Client_DataReceived;
            _client.ConnectionError -= Client_ConnectionError;
            _client.Dispose();
        }

        private void Client_StatusChanged(object sender, string status)
        {
            RaiseStatus(status);
        }

        private void Client_DataReceived(object sender, IpDataMessageModel message)
        {
            var handler = DataReceived;
            if (handler != null)
            {
                handler(this, message);
            }
        }

        private void Client_ConnectionError(object sender, Exception exception)
        {
            var handler = ConnectionError;
            if (handler != null)
            {
                handler(this, exception);
            }
        }

        private void RaiseStatus(string status)
        {
            var handler = StatusChanged;
            if (handler != null)
            {
                handler(this, status);
            }
        }
    }
}
