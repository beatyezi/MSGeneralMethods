using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Threading;

namespace MSGeneralMethods
{
    public class SerialPortClass
    {
        public enum SerialBaudrate
        {
            baudrate_2400 = 2400,
            baudrate_4800 = 4800,
            baudrate_9600 = 9600,
            baudrate_19200 = 19200,
            baudrate_115200 = 115200,
            baudrate_256000 = 256000,
            baudrate_460800 = 460800,
            baudrate_921600 = 921600
        };

        public enum SerialPortName
        {
            com1 = 1,
            com2,
            com3,
            com4,
            com5,
            com6,
            com7,
            com8,
            com9,
            com10,
            com11,
            com12,
            com13,
            com14,
            com15,
            com16
        };

        public byte[] ReceivedData { get; set; }
        public char[] ReceivedDataInChar { get; set; }
        public bool ReceiveReady { get; set; }
        public bool SerialPortOpenCompeleted { get; set; }

        /*判断是否需要将收到的数据转为byte数据
         * true时将收到的数据转为byte型
         * false时将收到的数据转为char型*/
        public bool ByteMode { get; set; }

        private bool isBeginReceiving = false;
        private bool isSerialPortOpen = false;

        private void SetAfterSerialClosedOrLost()
        {
            //成功关闭串口后或串口丢失后的设置
            isSerialPortOpen = false;
            isBeginReceiving = false;
            ReceiveReady = false;
            SerialPortOpenCompeleted = false;
        }        

        public SerialPort CurrentSerialPort { get; set; } = new SerialPort();
        
        public bool OpenSerialPort(SerialPortName portName, SerialBaudrate baudrate, bool byteMode)
        {            
            string portNameAfterConvert = String.Format("COM{0:D}", portName);
            SerialPort tempSerialPort = new SerialPort(portNameAfterConvert, (int)baudrate);

            CurrentSerialPort = tempSerialPort;
            ByteMode = byteMode;
            if(!isSerialPortOpen)
            {
                try
                {
                    //尝试打开串口
                    CurrentSerialPort.ReadTimeout = 8000;//串口读超过8秒
                    CurrentSerialPort.WriteTimeout = 8000;//串口写超时8秒
                    CurrentSerialPort.ReadBufferSize = 1024;//数据读缓存
                    CurrentSerialPort.WriteBufferSize = 1024;//数据写缓存                    
                    CurrentSerialPort.Parity = Parity.None;
                    CurrentSerialPort.DataBits = 8;
                    CurrentSerialPort.StopBits = StopBits.One;
                    CurrentSerialPort.DataReceived += CurrentSerialPortDataReceived;
                    CurrentSerialPort.Open();
                    isBeginReceiving = true;
                    isSerialPortOpen = true;
                    SerialPortOpenCompeleted = true;
                    
                }
                catch(Exception e)
                {
                    SetAfterSerialClosedOrLost();                   
                    throw new Exception("无法打开串口\r\n" + e.Message);
                }
            }

            return isSerialPortOpen;
        }

        private void CurrentSerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            ReceiveReady = false;
            if(isBeginReceiving)
            {
                try
                {
                    //Thread.Sleep(50);
                    ReceivedData = new byte[CurrentSerialPort.BytesToRead];
                    ReceivedDataInChar = new char[CurrentSerialPort.BytesToRead];

                    if(ByteMode)
                    {
                        CurrentSerialPort.Read(ReceivedData, 0, CurrentSerialPort.BytesToRead);
                    }
                    else
                    {
                        CurrentSerialPort.Read(ReceivedDataInChar, 0, CurrentSerialPort.BytesToRead);
                    }
                    ReceiveReady = true;
                }
                catch(Exception)
                {
                    if(!CurrentSerialPort.IsOpen)
                    {
                        //此时串口已丢失
                        SetAfterSerialClosedOrLost();
                    }
                    else
                    {
                        throw new Exception("无法通过串口接收数据");
                    }
                }
            }
            else
            {
                //暂停接收
                CurrentSerialPort.DiscardInBuffer();//清除接收缓存
            }
        }

        public bool SerialSend(byte[] dataInByte)
        {
            try
            {
                CurrentSerialPort.Write(dataInByte, 0, dataInByte.Length);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public bool SerialSend(char[] dataInByte)
        {
            try
            {
                CurrentSerialPort.Write(dataInByte, 0, dataInByte.Length);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public bool SerialSend(string dataInString)
        {
            char[] dataInCharArray = dataInString.ToCharArray();
            return SerialSend(dataInCharArray);
        }

        public bool CloseSerialPort()
        {
            bool isSuccessClosed = true;
            try
            {
                CurrentSerialPort.DiscardInBuffer();//清接收缓存
                CurrentSerialPort.DiscardOutBuffer();//清发送缓存
                CurrentSerialPort.Close();//关闭串口
                SetAfterSerialClosedOrLost();
                isSuccessClosed = true;
            }
            catch(Exception e)
            {
                if(!CurrentSerialPort.IsOpen)
                {
                    //串口已丢失
                    SetAfterSerialClosedOrLost();
                    isSuccessClosed = true;
                }
                else
                {
                    isSuccessClosed = false;
                    throw new Exception("未知原因导致无法关闭串口\r\n" + e.Message);                    
                }
            }

            return isSuccessClosed;
        }        
    }
}
