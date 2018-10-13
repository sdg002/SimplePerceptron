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
        public Layer()
        {
            Activation= ActivationType.Sigmoid;
        }
        /// <summary>
        /// The nodes in this layer
        /// </summary>
        public Neuron[] Nodes { get; set; }
        /// <summary>
        /// Sets/gets the type of activation on this layer
        /// </summary>
        public entity.ActivationType Activation { get; set; }
        /// <summary>
        /// Used defined comments
        /// </summary>
        public string Comments { get; set; }
        public override string ToString()
        {
            int countOfNodes = (this.Nodes == null) ? 0 : this.Nodes.Length;
            return $"Nodes={countOfNodes};Comments={Comments}";
        }
    }
}
