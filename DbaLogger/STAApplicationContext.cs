using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DbaLogger
{
    public class STAApplicationContext : ApplicationContext
    {
        public STAApplicationContext()
        {
            _dataManager = new DataManager();
            _deviceManager = new DeviceManager(_dataManager);
            _viewManager = new ViewManager(_deviceManager);

            _deviceManager.OnStatusChange += _viewManager.OnStatusChange;
        }

        private ViewManager _viewManager;
        private DeviceManager _deviceManager;
        private DataManager _dataManager;

        // Called from the Dispose method of the base class
        protected override void Dispose(bool disposing)
        {
            if ((_deviceManager != null) && (_viewManager != null))
            {
                _deviceManager.OnStatusChange -= _viewManager.OnStatusChange;
            }
            if (_deviceManager != null)
            {
                _deviceManager.Terminate();
            }
            _deviceManager = null;
            _viewManager = null;
            _dataManager = null;
        }
    }
}
