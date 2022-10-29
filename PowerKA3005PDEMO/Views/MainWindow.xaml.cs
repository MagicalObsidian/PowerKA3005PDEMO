using System.Windows;
using System.IO.Ports;
using System;

namespace PowerKA3005PDEMO.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //串口实例
        SerialPort serialPort = new SerialPort();

        public MainWindow()
        {
            InitializeComponent();

            //获取串口列表
            cbxSerialPortList.DataContext = SerialPort.GetPortNames();
            //设置可选波特率
            cbxBaudRate.DataContext = new object[] { 9600, 19200 };
            //设置可选奇偶校验
            cbxParity.DataContext = new object[] { "None", "Odd", "Even", "Mark", "Space" };
            //设置可选数据位
            cbxDataBits.DataContext = new object[] { 5, 6, 7, 8 };
            //设置可选停止位
            cbxStopBits.DataContext = new object[] { 1, 1.5, 2 };

            //设置默认选中项
            cbxSerialPortList.SelectedIndex = 0;
            cbxBaudRate.SelectedIndex = 0;
            cbxParity.SelectedIndex = 0;
            cbxDataBits.SelectedIndex = 3;
            cbxStopBits.SelectedIndex = 0;


        }

        #region 按钮方法
        /// <summary>
        /// 连接或关闭电源(串口) 按钮 的方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (SerialPort.GetPortNames().Length == 0)
            {
                MessageBox.Show("没有检测到串口！");
            }
            else
            {
                if (!serialPort.IsOpen)
                {
                    //设定参数
                    serialPort.PortName = cbxSerialPortList.SelectedItem.ToString();
                    serialPort.BaudRate = (int)cbxBaudRate.SelectedItem;
                    serialPort.Parity = GetSelectedParity();
                    serialPort.DataBits = (int)cbxDataBits.SelectedItem;
                    serialPort.StopBits = GetSelectedStopBits();

                    try
                    {
                        //打开串口
                        serialPort.Open();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("无法打开此串口，请检查是否被占用");
                        return;
                    }

                    //切换文本
                    tbxStatus.Text = "已连接";
                    btnSwitch.Content = "断开连接";

                    //切换可用状态
                    cbxSerialPortList.IsEnabled = false;
                    cbxBaudRate.IsEnabled = false;
                    cbxParity.IsEnabled = false;
                    cbxDataBits.IsEnabled = false;
                    cbxStopBits.IsEnabled = false;
                }
                else
                {
                    if (serialPort != null)
                    {
                        //关闭串口
                        serialPort.Close();
                    }

                    //切换文本
                    tbxStatus.Text = "未连接";
                    btnSwitch.Content = "连接电源";

                    //切换可用状态
                    cbxSerialPortList.IsEnabled = true;
                    cbxBaudRate.IsEnabled = true;
                    cbxParity.IsEnabled = true;
                    cbxDataBits.IsEnabled = true;
                    cbxStopBits.IsEnabled = true;
                }
            }

        }
        #endregion

        #region 获取预设参数方法
        /// <summary>
        /// 获取窗体选中的奇偶校验
        /// </summary>
        /// <returns></returns>
        private Parity GetSelectedParity()
        {
            switch (cbxParity.SelectedItem.ToString())
            {
                case "Odd":
                    return Parity.Odd;
                case "Even":
                    return Parity.Even;
                case "Mark":
                    return Parity.Mark;
                case "Space":
                    return Parity.Space;
                case "None":
                default:
                    return Parity.None;
            }
        }

        /// <summary>
        /// 获取窗体选中的停止位
        /// </summary>
        /// <returns></returns>
        private StopBits GetSelectedStopBits()
        {
            switch (Convert.ToDouble(cbxStopBits.SelectedItem))
            {
                case 1:
                    return StopBits.One;
                case 1.5:
                    return StopBits.OnePointFive;
                case 2:
                    return StopBits.Two;
                default:
                    return StopBits.One;
            }
        }
        #endregion
    }
}
