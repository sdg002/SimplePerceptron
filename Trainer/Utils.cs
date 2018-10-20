using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perceptron.entity;

namespace Trainer
{
    public class Utils
    {
        /// <summary>
        /// Returns an array of those vectors whose quantized actual outputs did not matchthe expected output
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        internal static Vector[] GetFailedVectors(VectorPropagationContext[] vectors)
        {
            List<Vector> failedvectors = new List<Vector>();
            foreach (var vecContext in vectors)
            {
                double[] outputActual = vecContext.Outputs;
                double[] outputActualQuantized = outputActual.Select(opt => (opt > 0.5) ? 1.0 : 0.0).ToArray();
                double[] outputExpected = vecContext.Vector.Outputs;
                if (outputActualQuantized.SequenceEqual(outputExpected) == false) failedvectors.Add(vecContext.Vector);
            }
            return failedvectors.ToArray();
        }
    }
}
