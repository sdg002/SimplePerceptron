using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.entity
{
    /// <summary>
    /// Represents either a hidden or an output layer
    /// </summary>
    public class Layer
    {
        /// <summary>
        /// The nodes in this layer
        /// </summary>
        public Neuron[] Nodes { get; set; }
    }
}
