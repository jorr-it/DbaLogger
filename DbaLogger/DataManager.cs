using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbaLogger
{
    public class DataManager
    {
        private DbaLogModel db;
        private Queue<Tuple<DateTime, byte>> queue;
        private bool enabled;
        public DateTime LastStorage;
        public DateTime LastProcessing;

        public DataManager()
        {
            this.db = new DbaLogModel();
            this.queue = new Queue<Tuple<DateTime, byte>>();
        }

        public void AddRawData(byte[] data)
        {
            foreach (var b in data)
            {
                this.queue.Enqueue(new Tuple<DateTime, byte>(DateTime.Now, b));
            }

            if (this.enabled == true && this.queue.Count > 1000)
            {
                this.enabled = false;
                this.processRawData();
                this.enabled = true;
            }
        }

        public bool Enable()
        {
            var possible = this.db.Database.Exists();
            this.enabled = possible;
            return possible;
        }

        public void Disable()
        {
            this.enabled = false;
        }

        public int GetQueueSize()
        {
            return this.queue.Count;
        }

        private void processRawData()
        {
            this.LastProcessing = DateTime.Now;

            var newLogs = false;

            while (this.queue.Count > 4)
            {
                var command = this.queue.Dequeue().Item2;

                if (command == 165) // new command is received
                {
                    var function = this.queue.Dequeue().Item2;

                    if (function == 13) // function of command is measured reading
                    {
                        var data1 = this.queue.Dequeue(); // first byte of measurement
                        var data2 = this.queue.Dequeue(); // second byte of measurement
                        var bcd = this.bcd2int(data1.Item2, data2.Item2); // calculation of dba (with one decimal) by BCD code
                        var loggedDate = data1.Item1.Trim(TimeSpan.TicksPerSecond); // received date rounded by seconds

                        if (db.DbaLogs.Find(loggedDate) == null)
                        {
                            db.DbaLogs.Add(new DbaLog { Id = loggedDate, Dba = bcd });
                            newLogs = true;
                        }
                    }
                }
            }

            if (newLogs)
            {
                db.SaveChanges();
                this.LastStorage = DateTime.Now;
            }
        }

        private int bcd2int(params byte[] bcds)
        {
            int result = 0; 
            foreach (byte bcd in bcds)
            {
                result *= 100;
                result += (10 * (bcd >> 4));
                result += bcd & 0xf;
            }
            return result;
        }
    }
}
