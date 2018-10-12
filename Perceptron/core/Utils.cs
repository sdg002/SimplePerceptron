using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perceptron.entity;

namespace Perceptron.core
{
    public class Utils
    {
        /// <summary>
        /// Creates a new multilayer perceptron network
        /// </summary>
        /// <param name="inputs">No of inputs received by the network</param>
        /// <param name="nodesperlayer">Specified the nodes per layer, starting with the first hidden layer</param>
        /// <returns></returns>
        public static MultilayerPerceptron CreateNetwork(int inputs, params int[] nodesperlayer)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Randomize the network wts using Xavier approach
        /// </summary>
        /// <param name="networkXOR"></param>
        public static void RandomizeNetworkWeights(MultilayerPerceptron networkXOR)
        {
            throw new NotImplementedException();
        }

        public static double[] ComputeNetworkOutput(MultilayerPerceptron networkXOR, Vector vector)
        {
            throw new NotImplementedException();
        }
    }
}
