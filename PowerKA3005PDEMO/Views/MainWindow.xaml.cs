using System.Windows;
using System.IO.Ports;
using System;
using System.Text;
using System.Windows.Documents;
using System.Numerics;
using System.Data;
using System.Collections.Generic;
using System.IO;
using ImTools;
using System.Collections;

using PowerKA3005P;
using System.Threading;

namespace PowerKA3005PDEMO.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //串口实例
        SerialPort serialPort = new SerialPort();

        //电源 库 的实例
        PowerOperate powerOperate = new PowerOperate();
        PowerSet powerSet = new PowerSet();
        PowerRead powerRead = new PowerRead();

        //按钮的状态
        bool OCP_STATUS = false;
        bool OVP_STATUS = false;
        bool OUT_STATUS = false;
        bool LOCK_STATUS = false;

        //默认选中的单选按钮
        int radioButtonChecked = 1;

        // 存储结构体 电流和电压
        struct IV
        {
            public double I;
            public double V;
        }
        //Directory<int, IV> MemoIV = new Dictionary<int, IV>();
        IV IV_M1;
        IV IV_M2;
        IV IV_M3;
        IV IV_M4;
        IV IV_M5;

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

        /// <summary>
        /// 按钮 OCP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_OCP_Click(object sender, RoutedEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                if(!OCP_STATUS)
                {
                    serialPort.Write(powerOperate.OCP_Cmd_Operate(true));
                    OCP_STATUS = true;
                    
                }    
                else
                {
                    serialPort.Write(powerOperate.OCP_Cmd_Operate(false));
                    OCP_STATUS = false; 
                }                
            }
        }

        /// <summary>
        /// 按钮 OVP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_OVP_Click(object sender, RoutedEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                if(!OVP_STATUS)
                {
                    serialPort.Write(powerOperate.OVP_Cmd_Operate(true));
                    OVP_STATUS = true;
                }
                else
                {
                    serialPort.Write(powerOperate.OVP_Cmd_Operate(false));
                    OVP_STATUS = false;
                }
            }
        }

        /// <summary>
        /// 按钮 OUT 输出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_OUT_Click(object sender, RoutedEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                if(!OUT_STATUS)
                {
                    serialPort.Write(powerOperate.OUT_Cmd_Operate(true));
                    OUT_STATUS = true;
                }
                else
                {
                    serialPort.Write(powerOperate.OUT_Cmd_Operate(false));
                    OUT_STATUS = false;
                }
            }
        }

        /// <summary>
        /// 按钮 LOCK 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_LOCK_Click(object sender, RoutedEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                if(!LOCK_STATUS)
                {
                    serialPort.Write(powerOperate.LOCK_Cmd_Operate(true));
                    LOCK_STATUS = true;
                }
                else
                {
                    serialPort.Write(powerOperate.LOCK_Cmd_Operate(false));
                    LOCK_STATUS = false;
                }
            }
        }

        /// <summary>
        /// 按钮 SAVE 保存设定值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            radioButtonChecked = GetRadioButton();
           
            if (serialPort.IsOpen)
            {
                //调出存储键
                serialPort.Write(powerOperate.RCL_Cmd_Operate(radioButtonChecked)); Thread.Sleep(100);

                //写入当前设置的电流电压值
                serialPort.Write(powerSet.ISET_Cmd_SET(I_Input.Value)); Thread.Sleep(100);
                serialPort.Write(powerSet.VSET_Cmd_SET(V_Input.Value)); Thread.Sleep(100);

                //写入存储键
                serialPort.Write(powerOperate.SAV_Cmd_Operate(radioButtonChecked)); Thread.Sleep(100);
                //AddMemo(radioButtonChecked, I_Input.Value, V_Input.Value);//测试
                
                ShowIV(I_Input.Value.ToString(), V_Input.Value.ToString());
            }
        }

        /// <summary>
        /// 按钮 RCL 读取设定值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Read_Click(object sender, RoutedEventArgs e)
        {
            //IV tmp;
            if (serialPort.IsOpen)
            {
                radioButtonChecked = GetRadioButton();

                //调出存储键
                serialPort.Write(powerOperate.RCL_Cmd_Operate(radioButtonChecked)); Thread.Sleep(100);
                //tmp = ReadMemo(radioButtonChecked);

                //读出存储键的设定值
                serialPort.Write(powerSet.ISET_Read); Thread.Sleep(100);
                string i = GetReceiveMsg();
                serialPort.Write(powerSet.VSET_Read); Thread.Sleep(100);
                string v = GetReceiveMsg();

                ShowIV(i, v);
            }
                
        }

        /// <summary>
        /// 按钮 读取当前 实际输出电压、电流
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ReadCurrent_Click(object sender, RoutedEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Write(powerRead.IOUT_Read); Thread.Sleep(100);
                string i = GetReceiveMsg();

                serialPort.Write(powerRead.VOUT_Read); Thread.Sleep(100);
                string v = GetReceiveMsg();

                ShowIV(i, v);
            }
        }

        /*
        private void RadioButtonSelected()
        {
            if(M1.IsChecked == true)
                serialPort.Write(powerOperate.RCL_Cmd_Operate(radioButtonChecked));
        }
        */
        #endregion

        #region 预设参数方法

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

        /// <summary>
        /// 获取接收到的数据(串口)，返回 字符串
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private string GetReceiveMsg()
        {
            //读取串口缓冲区的字节数据
            byte[] result = new byte[serialPort.BytesToRead];
            serialPort.Read(result, 0, serialPort.BytesToRead);

            //将接收到的字节数组转换为指定的消息格式
            string str = $"{Encoding.UTF8.GetString(result)}";

            return str;
        }

        /// <summary>
        /// 获取当前选中的单选按钮
        /// </summary>
        /// <returns></returns>
        private int GetRadioButton()
        {
            //switch()

            if((bool)M1.IsChecked)
            {
                return 1;
            }
            else if((bool)M2.IsChecked)
            {
                return 2;
            }
            else if((bool)M3.IsChecked)
            {
                return 3;
            }
            else if ((bool)M4.IsChecked)
            {
                return 4;
            }
            else if((bool)M5.IsChecked)
            {
                return 5;
            }
            return 0;
        }

        /// <summary>
        /// 添加 电流、电压到 存储（不用）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="i"></param>
        /// <param name="v"></param>
        private void AddMemo(int id, double i, double v)
        {
            switch (id)
            {
                case 1: IV_M1.I = i; IV_M1.V = v; break;
                case 2: IV_M2.I = i; IV_M2.V = v; break;
                case 3: IV_M3.I = i; IV_M3.V = v; break;
                case 4: IV_M4.I = i; IV_M4.V = v; break;
                case 5: IV_M5.I = i; IV_M5.V = v; break;

                default:break;
            }
        }

        /// <summary>
        /// 读取 当前 存储值（不用）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private IV ReadMemo(int id)
        {
            switch (id)
            {
                case 1: return IV_M1;
                case 2: return IV_M2;
                case 3: return IV_M3;
                case 4: return IV_M4;
                case 5: return IV_M5;

                default:break;
            }
            return IV_M1;
        }

        /// <summary>
        /// 显示 电流、电压值 到 界面
        /// </summary>
        /// <param name="i"></param>
        /// <param name="v"></param>
        private void ShowIV(string i, string v)
        {
            text_currentI.Text = i;
            text_currentV.Text = v;
        }

        #endregion

    }
}
