﻿using System;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ArduinoDataReceiver
{
    public partial class MainWindow : Window
    {
        private SerialPort serialPort;
        private string receivedData = "";

        public MainWindow()
        {
            InitializeComponent();
            Closing += Window_Closing;
            UpdateStatus("Готов к мониторингу");
        }

        private void InitializeSerialPort()
        {
            if (serialPort == null)
            {
                serialPort = new SerialPort("COM3", 9600); // Укажите порт и скорость, на которых работает Arduino
                serialPort.DataReceived += SerialPort_DataReceived;
            }

            if (!serialPort.IsOpen)
            {
                try
                {
                    serialPort.Open();
                    UpdateStatus("Мониторинг запущен");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при открытии COM порта: " + ex.Message);
                }
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            receivedData += serialPort.ReadExisting();

            // Обновляем интерфейс в основном потоке
            Dispatcher.Invoke(() =>
            {
                txtReceivedData.Text = receivedData;
                txtReceivedData.ScrollToEnd(); // Прокрутка к новым данным
            });
        }

        private void StartMonitoring_Click(object sender, RoutedEventArgs e)
        {
            InitializeSerialPort();
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                await CloseSerialPort();
            }
        }

        private Task CloseSerialPort()
        {
            return Task.Run(() =>
            {
                serialPort.Close();
                serialPort.Dispose(); // Освобождаем ресурсы
                UpdateStatus("Мониторинг остановлен");
            });
        }

        private void UpdateStatus(string status)
        {
            Dispatcher.Invoke(() =>
            {
                statusTextBlock.Text = status;
            });
        }
    }
}
