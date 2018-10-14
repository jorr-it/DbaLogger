using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbaLogger
{
    public class DeviceManager
    {
        public DeviceManager(DataManager dm)
        {
            this.dataManager = dm;
            Status = DeviceStatus.Uninitialised;
        }
        
        public delegate void StatusChangeEvent();
        public event StatusChangeEvent OnStatusChange;

        public string DeviceName { get; private set; }
        public DeviceStatus Status { get; private set; }
        public List<KeyValuePair<string, DateTime>> StatusFlags
        {
            get
            {
                List<KeyValuePair<string, DateTime>> statusFlags = new List<KeyValuePair<string, DateTime>>();

                switch (Status)
                {
                    case DeviceStatus.Running:
                        statusFlags.Add(new KeyValuePair<string, DateTime>("Last receival", this.lastReceival));
                        statusFlags.Add(new KeyValuePair<string, DateTime>("Queue size: " + this.dataManager.GetQueueSize().ToString(), DateTime.Now));
                        statusFlags.Add(new KeyValuePair<string, DateTime>("Last processing", this.dataManager.LastProcessing));
                        statusFlags.Add(new KeyValuePair<string, DateTime>("Last storage", this.dataManager.LastStorage));
                        break;
                    case DeviceStatus.Error:
                    case DeviceStatus.Uninitialised:
                    case DeviceStatus.Initialised:
                        break;
                }
                return statusFlags;
            }
        }

        private SerialPort serialPort;
        private DataManager dataManager;
        private DateTime lastReceival;

        public void Initialise()
        {
            if (Status == DeviceStatus.Uninitialised)
            {
                this.DeviceName = "Voltcraft SL-400";

                try
                {
                    this.serialPort = new SerialPort("COM3");
                    this.serialPort.BaudRate = 9600;
                    this.serialPort.Parity = Parity.None;
                    this.serialPort.StopBits = StopBits.One;
                    this.serialPort.DataBits = 8;
                    this.serialPort.Handshake = Handshake.None;
                    this.serialPort.DataReceived += new SerialDataReceivedEventHandler(dataReceivedHandler);
                    this.serialPort.Open();
                }
                catch (Exception ex)
                {
                    Status = DeviceStatus.Error;
                    OnStatusChange();
                    return;
                }

                var enabled = this.dataManager.Enable();
                if (!enabled)
                {
                    Status = DeviceStatus.Error;
                }
                else
                {
                    Status = DeviceStatus.Initialised;
                }

                OnStatusChange();
            }
        }

        public void Stop()
        {
            if (Status == DeviceStatus.Running)
            {
                this.serialPort.Close();
                this.dataManager.Disable();
                Status = DeviceStatus.Initialised;
                OnStatusChange();
            }
        }
        
        public void Terminate()
        {
            Stop();
            Status = DeviceStatus.Uninitialised;
        }

        private void dataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (Status == DeviceStatus.Initialised)
            {
                Status = DeviceStatus.Running;
                OnStatusChange();
            }

            SerialPort sp = (SerialPort)sender;

            byte[] buf = new byte[sp.BytesToRead];
            sp.Read(buf, 0, buf.Length);
            this.dataManager.AddRawData(buf);

            this.lastReceival = DateTime.Now;
        }

    }
}
