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

        internal static void DoBackwardPass(MultilayerPerceptron perceptron, VectorPropagationContext ctx)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateNetworkWeights(MultilayerPerceptron perceptron, VectorPropagationContext ctx)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Randomize the network wts using Xavier approach
        /// </summary>
        /// <param name="network"></param>
        public static void RandomizeNetworkWeights(MultilayerPerceptron network)
        {
            //TODO implementation required
        }
        /// <summary>
        /// This function computes the output values at the output layer using the given input vector and trained network
        /// </summary>
        /// <param name="network"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static double[] ComputeNetworkOutput(MultilayerPerceptron network, Vector vector)
        {
            VectorPropagationContext vectorContext = new VectorPropagationContext(vector);
            DoForwardPass(network,vectorContext);
            Layer layerLast = network.Layers.Last();
            double[] activationsFromLastLayer=layerLast.
                Nodes.Select(nd => vectorContext.NodeActivationCache[nd.GetID()]).ToArray();
            return activationsFromLastLayer;
        }
        /// <summary>
        /// Internal function which will do the forward pass from the first layer to the last.
        /// Dotproducts and activations at each node will be computed and cached in the context object
        /// </summary>
        /// <param name="network">The MLP</param>
        /// <param name="ctx">The context wrapper over the training vector</param>
        internal static void DoForwardPass(
            MultilayerPerceptron network, VectorPropagationContext ctx)
        {
            for (int layerindex = 0; layerindex < network.Layers.Length; layerindex++)
            {
                Layer layerCurrent = network.Layers[layerindex];
                Layer layerPrevious = null;
                int countOfNodes = layerCurrent.Nodes.Length;
                double[] incomingvalues = null;//Values coming into this layer
                if (layerindex == 0)
                {
                    //We are on the first layer, so the inputs are the vector itself
                    incomingvalues = ctx.Vector.Inputs;
                }
                else
                {
                    //We are on hidden layers, so the inputs are the activations of the previous layer
                    layerPrevious = network.Layers[layerindex - 1];
                    incomingvalues = layerPrevious.Nodes.Select(nd => ctx.NodeActivationCache[nd.GetID()]).ToArray();
                }
                for (int nodeindex = 0; nodeindex < countOfNodes; nodeindex++)
                {
                    Neuron node = layerCurrent.Nodes[nodeindex];
                    double dotproduct = 0;
                    for (int i = 0; i < node.Weights.Length; i++)
                    {
                        dotproduct = dotproduct + incomingvalues[i] * node.Weights[i].Value;
                    }
                    dotproduct += node.Bias.Value;
                    ctx.NodeDotProductsCache[node.GetID()] = dotproduct;
                    double activation = ComputeActivation(layerCurrent, node, dotproduct);
                    ctx.NodeActivationCache[node.GetID()] = activation;
                }
            }
        }
        /// <summary>
        /// Computes the activation using the dotproduct
        /// </summary>
        /// <param name="layer">The layer object which contains the neuron</param>
        /// <param name="node">The neuron on which activation is being computed</param>
        /// <param name="dotproduct">The dotproduct value which will be used for computing the activation</param>
        /// <returns></returns>
        public static double ComputeActivation(Layer layer, Neuron node, double dotproduct)
        {
            if (layer.Activation == ActivationType.Sigmoid)
            {
                double output = 1 / (1 + Math.Exp(-dotproduct));
                return output;
            }
            else
            {
                throw new NotImplementedException($"This activation type is not yet implemented. {layer.Activation}");
            }
        }
        public static string SaveNetworkToJson(Perceptron.entity.MultilayerPerceptron network)
        {
            throw new NotImplementedException();
        }
        public static Perceptron.entity.MultilayerPerceptron LoadNetworkFromJson(string json)
        {
            throw new NotImplementedException();
        }
    }
}
