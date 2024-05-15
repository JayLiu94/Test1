using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Tracer
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {

        public SerialPort serialPort;
        private DispatcherTimer comPortCheckTimer;
        public MainWindow()
        {

            InitializeComponent();

            // initial port
            serialPort = new SerialPort();


            // 設定串口屬性
            serialPort.BaudRate = 115200;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.Parity = Parity.None;

            // 設定串口接收事件處理程序
            serialPort.DataReceived += SerialPort_DataReceived;

            // 設定串口接收事件處理程序
            serialPort.DataReceived += SerialPort_DataReceived;

            // 初始化計時器
            comPortCheckTimer = new DispatcherTimer();
            comPortCheckTimer.Interval = TimeSpan.FromSeconds(5); // 設定檢查間隔為 5 秒
            comPortCheckTimer.Tick += ComPortCheckTimer_Tick;
            comPortCheckTimer.Start();

            // Enumerate COM PORT
            EnumerateSerialPorts();


            // Handle command-line arguments
            string[] args = Environment.GetCommandLineArgs();
            CommandHandle.HandleCommandLineArguments(args);

        }


        // 定時檢查 COM PORT 變更
        private void ComPortCheckTimer_Tick(object sender, EventArgs e)
        {
            EnumerateSerialPorts();
        }

        // 列舉 COM PORT
        private void EnumerateSerialPorts()
        {
            // 儲存當前選擇的 COM PORT
            string selectedPort = comboBoxPorts.SelectedItem?.ToString();

            // 清空 COM PORT 列表
            comboBoxPorts.Items.Clear();

            // 重新列舉並加入 COM PORT
            string[] ports = SerialPort.GetPortNames();
            foreach (var port in ports)
            {
                comboBoxPorts.Items.Add(port);
            }

            // 還原之前的選擇
            if (!string.IsNullOrEmpty(selectedPort) && comboBoxPorts.Items.Contains(selectedPort))
            {
                comboBoxPorts.SelectedItem = selectedPort;
            }
        }


        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // 在這裡處理接收到的數據，例如將其附加到文本框
            string data = serialPort.ReadExisting();

            // 使用 Dispatcher 以在 UI 線程上更新 UI
            Dispatcher.Invoke(() =>
            {
                textBox1.AppendText(data);

                // 滾動到最底部
                textBox1.ScrollToEnd();
            });
        }
   


        public void StartButton_Click(object sender, RoutedEventArgs e)
        {

            // 获取用户选择的串口
            string selectedPort = comboBoxPorts.SelectedItem as string;

            if (string.IsNullOrEmpty(selectedPort) || !int.TryParse(BaudRateBox.Text, out int baudRate))
            {
                textBox1.AppendText("Please input correct parameters!" + "\n");
                return;
            }

            // 创建新的SerialPort实例
            serialPort = new SerialPort(selectedPort, baudRate, Parity.None, 8, StopBits.One);
            //textBox1.AppendText("Setting OK! " + "\n");

            if (!serialPort.IsOpen)
            {
                serialPort.Open();
                serialPort.DataReceived += SerialPort_DataReceived;
                textBox1.AppendText("Opened Serial Port!!!  " + "\n");
            }
        }

        







        public void StopButton_Click(object sender, RoutedEventArgs e)
        {
            // 使用 Dispatcher.Invoke 在 UI 线程上执行关闭串口和注销事件处理程序的操作
            Dispatcher.Invoke(() =>
            {
                // 关闭串口
                serialPort.Close();

                // 注销DataReceived事件处理程序
                serialPort.DataReceived -= SerialPort_DataReceived;

                textBox1.AppendText("Stop Received UART! " + "\n");
            });
        }


        public void SaveTextToFile(TextBox textBox, string filePath)
        {
            Dispatcher.BeginInvoke(new Action(() => { File.WriteAllText(filePath, textBox.Text); }));
        }




        public void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Clear();
        }

        

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 檢查是否存在 LOG 資料夾，若不存在則建立
                string logFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LOG");
                if (!Directory.Exists(logFolder))
                {
                    Directory.CreateDirectory(logFolder);
                }

                // 生成檔案名稱
                string fileName = "output_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";

                // 生成完整檔案路徑
                string filePath = System.IO.Path.Combine(logFolder, fileName);

                // 保存 TextBox 中的內容到檔案
                SaveTextToFile(textBox1, filePath);

                // 在完成保存後顯示訊息
                textBox1.AppendText("Saved to " + filePath + "!!!" + "\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox1.ScrollToEnd();
        }
    }
}



