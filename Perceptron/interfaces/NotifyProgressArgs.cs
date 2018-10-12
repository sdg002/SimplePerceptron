using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.interfaces
{
    public class NotifyProgressArgs : EventArgs
    {
        /// <summary>
        /// No of epochs that have elapsed
        /// </summary>
        public int Epochs { get; set; }
        /// <summary>
        /// If the caller sets this to True then the training will be halted
        /// </summary>
        public bool Cancel { get; set; }
    }
}
