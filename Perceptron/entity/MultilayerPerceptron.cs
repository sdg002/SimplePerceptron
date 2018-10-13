using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.entity
{
    public class MultilayerPerceptron
    {
        int _noOfInputs;
        Layer[] _layers;
        /// <summary>
        /// Size of the input vector. E.g. for a point in 3d space, this would be 3
        /// </summary>
        public int Inputs
        {
            get
            {
                return _noOfInputs;
            }
            set
            {
                if (value <= 0) throw new ArgumentException("No of inputs should be positive");
                _noOfInputs = value;
            }
        }
        /// <summary>
        /// Sets/gets the array of layers in the network. This is the combined collection of hidden and output layers.
        /// The input layer is not considered to be a member of this array.
        /// </summary>
        public Layer[] Layers
        {
            get => _layers;
            set
            {
                if (value == null) throw new ArgumentNullException("Layers cannot be NULL");
                if (value.Length == 0) throw new ArgumentException("Layers array cannot be empty");
                _layers = value;
            }
        }
        public override bool Equals(object obj)
        {
            MultilayerPerceptron target = obj as MultilayerPerceptron;
            if (target == null) return false;
            if (target.Inputs != this.Inputs) return false;
            if (target.Layers != null && this.Layers == null) return false;
            if (target.Layers == null && this.Layers != null) return false;
            if (target.Layers!=null && this.Layers!=null)
            {
                if (target.Layers.Length != this.Layers.Length) return false;
                for (int layerindex = 0; layerindex < this.Layers.Length; layerindex++)
                {
                    Layer thisLayer = this.Layers[layerindex];
                    Layer targetLayer = target.Layers[layerindex];
                    for (int nodeindex = 0; nodeindex < thisLayer.Nodes.Length; nodeindex++)
                    {
                        Neuron nodeSource = thisLayer.Nodes[nodeindex];
                        Neuron nodeTarget = targetLayer.Nodes[nodeindex];
                        if (nodeSource.Equals(nodeTarget) == false) return false;
                    }
                }
            }
            return true;
        }

    }
}
