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
            if (inputs <= 0) throw new ArgumentException("No of inputs should be positive");
            if (nodesperlayer == null || nodesperlayer.Length == 0) throw new ArgumentNullException("Nodes per layer cannot be empty");
            var mlp = new MultilayerPerceptron();
            mlp.Inputs = inputs;
            List<Layer> layers = new List<Layer>();
            for(int layerindex=0;layerindex<nodesperlayer.Length;layerindex++)
            {
                List<Neuron> nodes = new List<Neuron>();
                var layer = new Layer {  };
                int countOfNodes = nodesperlayer[layerindex];
                if (countOfNodes <= 0)
                    throw new ArgumentException($"The count of nodes must be positive in every layer. Layer index={layerindex}");
                for(int nodeindex=0;nodeindex<countOfNodes;nodeindex++)
                {
                    Neuron node = new Neuron();
                    if (layerindex == 0)
                    {
                        node.Weights = new Weight[inputs];
                    }
                    else
                    {
                        //No of wts would be equal to the no of nodes in the previous layer
                        node.Weights = new Weight[nodesperlayer[layerindex - 1]];
                    }
                    nodes.Add(node);
                }
                layer.Nodes = nodes.ToArray();
                layers.Add(layer);
            }
            mlp.Layers = layers.ToArray();
            return mlp;
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
