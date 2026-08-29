using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Services
{
    public class IpConnectionClient : IDisposable
    {
        private readonly Encoding _encoding = Encoding.UTF8;
        private TcpClient _client;
        private NetworkStream _stream;
        private CancellationTokenSource _readCancellation;

        public event EventHandler<string> StatusChanged;
        public event EventHandler<IpDataMessageModel> DataReceived;
        public event EventHandler<Exception> ConnectionError;

        public bool IsConnected
        {
            get { return _client != null && _client.Connected; }
        }

        public async Task ConnectAsync(string host, int port)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentException("IP address or host name is required.", "host");
            }

            Disconnect();

            _client = new TcpClient();
            RaiseStatus("Connecting to " + host + ":" + port + "...");
            await _client.ConnectAsync(host.Trim(), port).ConfigureAwait(false);
            _stream = _client.GetStream();
            _readCancellation = new CancellationTokenSource();
            RaiseStatus("Connected to " + host.Trim() + ":" + port);
            BeginReadLoop(_readCancellation.Token);
        }

        public async Task SendTextAsync(string text)
        {
            if (!IsConnected || _stream == null)
            {
                throw new InvalidOperationException("No active IP connection.");
            }

            if (text == null)
            {
                text = string.Empty;
            }

            var bytes = _encoding.GetBytes(text);
            await _stream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
            await _stream.FlushAsync().ConfigureAwait(false);
        }

        public void Disconnect()
        {
            if (_readCancellation != null)
            {
                _readCancellation.Cancel();
                _readCancellation.Dispose();
                _readCancellation = null;
            }

            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }

            if (_client != null)
            {
                _client.Close();
                _client = null;
            }

            RaiseStatus("Disconnected");
        }

        public void Dispose()
        {
            Disconnect();
        }

        private async void BeginReadLoop(CancellationToken token)
        {
            var buffer = new byte[4096];

            try
            {
                while (!token.IsCancellationRequested && _stream != null)
                {
                    var count = await _stream.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(false);
                    if (count <= 0)
                    {
                        RaiseStatus("Remote device closed the connection.");
                        Disconnect();
                        return;
                    }

                    var bytes = new byte[count];
                    Buffer.BlockCopy(buffer, 0, bytes, 0, count);
                    RaiseDataReceived(new IpDataMessageModel
                    {
                        Timestamp = DateTime.Now,
                        Direction = "RX",
                        Text = _encoding.GetString(bytes),
                        Hex = ToHex(bytes),
                        ByteCount = count
                    });
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception ex)
            {
                RaiseError(ex);
                Disconnect();
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

        private void RaiseDataReceived(IpDataMessageModel message)
        {
            var handler = DataReceived;
            if (handler != null)
            {
                handler(this, message);
            }
        }

        private void RaiseError(Exception ex)
        {
            var handler = ConnectionError;
            if (handler != null)
            {
                handler(this, ex);
            }
        }

        private static string ToHex(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder(bytes.Length * 3);
            for (var i = 0; i < bytes.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(' ');
                }

                builder.Append(bytes[i].ToString("X2"));
            }

            return builder.ToString();
        }
    }
}
