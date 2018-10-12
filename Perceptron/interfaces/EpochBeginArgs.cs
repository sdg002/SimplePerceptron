using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.interfaces
{
    /// <summary>
    /// Fired right before the start of an epoch
    /// </summary>
    public class EpochBeginArgs : EventArgs
    {
        /// <summary>
        /// No of epochs that have elapsed already
        /// </summary>
        public int Epochs { get; set; }
    }
}
