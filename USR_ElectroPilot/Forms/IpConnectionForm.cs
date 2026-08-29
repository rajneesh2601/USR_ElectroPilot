using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;
using USR_ElectroPilot.Services;

namespace USR_ElectroPilot.Forms
{
    public class IpConnectionForm : Form
    {
        private readonly IpConnectionClient _client = new IpConnectionClient();
        private readonly BindingSource _logSource = new BindingSource();
        private readonly List<IpDataMessageModel> _messages = new List<IpDataMessageModel>();
        private TextBox _txtHost;
        private NumericUpDown _numPort;
        private CheckBox _chkAppendNewLine;
        private Button _btnConnect;
        private Button _btnDisconnect;
        private TextBox _txtSend;
        private Button _btnSend;
        private Button _btnClear;
        private Label _lblStatus;
        private DataGridView _grid;

        public IpConnectionForm()
        {
            Text = "IP Connection Test";
            Width = 980;
            Height = 620;
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(780, 480);

            BuildUi();
            WireEvents();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UiHelper.ApplyDarkTheme(this);
            _logSource.DataSource = _messages;
            UpdateConnectionButtons(false);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _client.Dispose();
            base.OnFormClosing(e);
        }

        private void BuildUi()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(12)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 96));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 94));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

            var connectionPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 8,
                RowCount = 2
            };
            connectionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            connectionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            connectionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48));
            connectionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            connectionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            connectionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            connectionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            connectionPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            connectionPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            connectionPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

            _txtHost = new TextBox { Dock = DockStyle.Fill, Text = "127.0.0.1" };
            _numPort = new NumericUpDown { Dock = DockStyle.Fill, Minimum = 1, Maximum = 65535, Value = 502 };
            _btnConnect = new Button { Dock = DockStyle.Fill, Text = "Connect" };
            _btnDisconnect = new Button { Dock = DockStyle.Fill, Text = "Disconnect" };
            _btnClear = new Button { Dock = DockStyle.Fill, Text = "Clear Log" };
            _chkAppendNewLine = new CheckBox { Dock = DockStyle.Fill, Text = "Append CR/LF", Checked = true };
            _lblStatus = new Label { Dock = DockStyle.Fill, Text = "Disconnected", TextAlign = ContentAlignment.MiddleLeft };

            connectionPanel.Controls.Add(new Label { Text = "IP/Host", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            connectionPanel.Controls.Add(_txtHost, 1, 0);
            connectionPanel.Controls.Add(new Label { Text = "Port", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 2, 0);
            connectionPanel.Controls.Add(_numPort, 3, 0);
            connectionPanel.Controls.Add(_btnConnect, 4, 0);
            connectionPanel.Controls.Add(_btnDisconnect, 5, 0);
            connectionPanel.Controls.Add(_btnClear, 6, 0);
            connectionPanel.Controls.Add(_chkAppendNewLine, 1, 1);
            connectionPanel.Controls.Add(new Label { Text = "Status", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
            connectionPanel.Controls.Add(_lblStatus, 2, 1);
            connectionPanel.SetColumnSpan(_lblStatus, 6);

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Timestamp", HeaderText = "Time", FillWeight = 95 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Direction", HeaderText = "Dir", FillWeight = 40 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ByteCount", HeaderText = "Bytes", FillWeight = 45 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Text", HeaderText = "Text", FillWeight = 180 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Hex", HeaderText = "Hex", FillWeight = 180 });
            _grid.DataSource = _logSource;

            var sendPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2
            };
            sendPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            sendPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            sendPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
            sendPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            sendPanel.Controls.Add(new Label { Dock = DockStyle.Fill, Text = "Send Data", TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            _txtSend = new TextBox { Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Vertical };
            _btnSend = new Button { Dock = DockStyle.Fill, Text = "Send" };
            sendPanel.Controls.Add(_txtSend, 0, 1);
            sendPanel.Controls.Add(_btnSend, 1, 1);

            root.Controls.Add(connectionPanel, 0, 0);
            root.Controls.Add(_grid, 0, 1);
            root.Controls.Add(sendPanel, 0, 2);
            root.Controls.Add(new Label
            {
                Dock = DockStyle.Fill,
                Text = "Raw TCP/IP test screen for future PLC/device integration. Received data is shown as text and hex.",
                TextAlign = ContentAlignment.MiddleLeft
            }, 0, 3);

            Controls.Add(root);
        }

        private void WireEvents()
        {
            _btnConnect.Click += async delegate { await ConnectAsync(); };
            _btnDisconnect.Click += delegate { _client.Disconnect(); UpdateConnectionButtons(false); };
            _btnSend.Click += async delegate { await SendAsync(); };
            _btnClear.Click += delegate { ClearLog(); };
            _client.StatusChanged += delegate(object sender, string status) { RunOnUi(delegate { SetStatus(status); }); };
            _client.DataReceived += delegate(object sender, IpDataMessageModel message) { RunOnUi(delegate { AddMessage(message); }); };
            _client.ConnectionError += delegate(object sender, Exception ex) { RunOnUi(delegate { SetStatus("Error: " + ex.Message); UpdateConnectionButtons(false); }); };
        }

        private async Task ConnectAsync()
        {
            try
            {
                UpdateConnectionButtons(false);
                await _client.ConnectAsync(_txtHost.Text, Convert.ToInt32(_numPort.Value));
                UpdateConnectionButtons(true);
            }
            catch (Exception ex)
            {
                SetStatus("Connect failed: " + ex.Message);
                UpdateConnectionButtons(false);
            }
        }

        private async Task SendAsync()
        {
            try
            {
                var text = _txtSend.Text;
                if (_chkAppendNewLine.Checked)
                {
                    text += "\r\n";
                }

                await _client.SendTextAsync(text);
                AddMessage(new IpDataMessageModel
                {
                    Timestamp = DateTime.Now,
                    Direction = "TX",
                    Text = text,
                    Hex = ToHex(text),
                    ByteCount = System.Text.Encoding.UTF8.GetByteCount(text)
                });
            }
            catch (Exception ex)
            {
                SetStatus("Send failed: " + ex.Message);
                UpdateConnectionButtons(_client.IsConnected);
            }
        }

        private void AddMessage(IpDataMessageModel message)
        {
            _messages.Add(message);
            _logSource.ResetBindings(false);
            if (_grid.Rows.Count > 0)
            {
                _grid.FirstDisplayedScrollingRowIndex = _grid.Rows.Count - 1;
            }
        }

        private void ClearLog()
        {
            _messages.Clear();
            _logSource.ResetBindings(false);
        }

        private void SetStatus(string status)
        {
            _lblStatus.Text = status;
        }

        private void UpdateConnectionButtons(bool connected)
        {
            _btnConnect.Enabled = !connected;
            _btnDisconnect.Enabled = connected;
            _btnSend.Enabled = connected;
            _txtHost.Enabled = !connected;
            _numPort.Enabled = !connected;
        }

        private void RunOnUi(MethodInvoker action)
        {
            if (IsDisposed || action == null)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(action);
                return;
            }

            action();
        }

        private static string ToHex(string text)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(text ?? string.Empty);
            var builder = new System.Text.StringBuilder(bytes.Length * 3);
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
