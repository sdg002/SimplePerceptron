using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.entity
{
    /// <summary>
    /// Represents a multidimensional vector
    /// </summary>
    public class Vector
    {
        /// <summary>
        /// To store any information that might be useful for troubleshooting. E.g. the name of the picture file which produced the training vector
        /// </summary>
        public string Comments { get; set; }
        /// <summary>
        /// To hold any user defined custom data object
        /// </summary>
        public Object Tag { get; set; }
        /// <summary>
        /// Unique ID initialized in the constructor
        /// </summary>
        public Guid OID { get; set; }
        /// <summary>
        /// The input attributes from this 
        /// </summary>
        public double[] Inputs { get; set; }
        /// <summary>
        /// Expected output attributes from this training/test vector
        /// </summary>
        public double[] Outputs { get; set; }
        public override string ToString()
        {
            int countOfInputs = (this.Inputs == null) ? 0 : this.Inputs.Length;
            int countOfOutputs = (this.Outputs== null) ? 0 : this.Outputs.Length;
            return $"Comments={this.Comments}, Inputs={countOfInputs}; Outputs={countOfOutputs}";
        }
    }
}
