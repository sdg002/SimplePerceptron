using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.interfaces
{
    public class NotifyProgressArgs : EventArgs
    {
        //Not required - can be obtained from Trainer class
        ///// <summary>
        ///// No of epochs that have elapsed
        ///// </summary>
        //public int Epochs { get; set; }
        /// <summary>
        /// If the caller sets this to True then the training will be halted
        /// </summary>
        public bool Cancel { get; set; }
        /// <summary>
        /// An array of objects, 1 per every original training vector. Provides more information about the training
        /// </summary>
        public entity.VectorPropagationContext[] Vectors { get; set; }
    }
}
