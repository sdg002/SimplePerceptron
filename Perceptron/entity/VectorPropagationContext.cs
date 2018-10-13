using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.entity
{
    /// <summary>
    /// Stores detailed information as a Vector propagates forward 
    /// and then backward through the layers
    /// </summary>
    public class VectorPropagationContext
    {
        Vector _vec;
        Dictionary<long, double> _nodeDotProductsCache;
        Dictionary<long, double> _nodeActivationCache;
        Dictionary<long, double> _nodeDeltaCache;
        Dictionary<long, double> _weightUpdatesCache;
        /// <summary>
        /// Encapsulates an outer training vectors
        /// </summary>
        /// <param name="vec"></param>
        public VectorPropagationContext(Vector vec)
        {
            _vec = vec;
            _nodeActivationCache = new Dictionary<long, double>();
            _nodeDotProductsCache = new Dictionary<long, double>();
            _nodeDeltaCache = new Dictionary<long, double>();
            _weightUpdatesCache = new Dictionary<long, double>();
        }
        public Vector Vector { get => _vec;  }
        /// <summary>
        /// Caches the dot products that was computed at every Node for this vector as this vector moved ahead
        /// </summary>
        public Dictionary<long, double> NodeDotProductsCache { get => _nodeDotProductsCache;  }
        /// <summary>
        /// Caches the activation values that was computed at every Node for this vector as this vectory moved head
        /// </summary>
        public Dictionary<long, double> NodeActivationCache { get => _nodeActivationCache;  }
        /// <summary>
        /// Caches the Delta values that was computed at every Node for this vector in the backward pass
        /// </summary>
        public Dictionary<long, double> NodeDeltaCache { get => _nodeDeltaCache;  }
        /// <summary>
        /// Caches the weight updates that were computed for this training vector when the backward pass had completed
        /// </summary>
        public Dictionary<long, double> WeightUpdateCache { get => _weightUpdatesCache;  }
        /// <summary>
        /// Used for recording the errors at the output layer
        /// </summary>
        public double[] OutputLayerErrors;
        /// <summary>
        /// Used for recording the MSE at the output layer. This is derived from the values held in OutputLayerErrors
        /// </summary>
        public double MeanSquaredError { get; internal set; }
        /// <summary>
        /// Actual outputs produced at the output layer. These are the activations on the output layer
        /// Could have been obtained from the NodeActivationCache. Added for simplicity of reporting
        /// </summary>
        public double[] Outputs { get; internal set; }
    }
}
