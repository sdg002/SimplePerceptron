using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.entity
{
    /// <summary>
    /// The activation functions supported by a layer of the network
    /// </summary>
    public enum ActivationType
    {
        Sigmoid,
        Relu,
        Unity,
        Custom
    }
}
