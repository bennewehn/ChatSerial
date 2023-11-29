using System;
using System.IO.Ports;
using System.Text;
using System.Windows;

namespace ChatSerial
{
    public partial class MainWindow : Window
    {
        readonly SerialPort serialPort = new();

        public MainWindow()
        {
            InitializeComponent();
            GetComPorts();

            serialPort.BaudRate = 9600;
            serialPort.Parity = Parity.None;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.DataReceived += SerialPort_DataReceived;
            txtUsername.Text = "user1";
        }

        private async void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] data = new byte[serialPort.BytesToRead];
            await serialPort.BaseStream.ReadAsync(data, 0, data.Length);

            await Dispatcher.InvokeAsync(() =>
            {
                txtChat.Text += Encoding.ASCII.GetString(data);
                txtChat.ScrollToEnd();
            });
        }

        private void GetComPorts()
        {
            string[] ports = SerialPort.GetPortNames();
            comPortsBox.ItemsSource = ports;
            comPortsBox.SelectedIndex = 0;
        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            GetComPorts();
        }

        private async void Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                await serialPort.BaseStream.WriteAsync(
                    Encoding.ASCII.GetBytes(txtUsername.Text + ": " + txtMessage.Text + '\n'));
                txtChat.Text += txtUsername.Text + ": " + txtMessage.Text + '\n';
                txtMessage.Text = "";
                txtChat.ScrollToEnd();
            }
        }

        private void open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                    open.Content = "Open";
                    sendBtn.IsEnabled = false;
                }
                else
                {
                    serialPort.PortName = comPortsBox.SelectedValue.ToString();
                    serialPort.Open();
                    open.Content = "Close";
                    sendBtn.IsEnabled = true;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                open.Content = "Open";
            }
        }
    }
}
