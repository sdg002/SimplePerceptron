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

        
    }
}
