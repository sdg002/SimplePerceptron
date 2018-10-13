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
        private static Random rnd = new Random(DateTime.Now.Millisecond);

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
                layer.Comments = $"Layer={layerindex}";
                for (int nodeindex=0;nodeindex<countOfNodes;nodeindex++)
                {
                    Neuron node = new Neuron();
                    if (layerindex == 0)
                    {
                        node.Weights = _CreateArrayOfWeights(inputs);
                    }
                    else
                    {
                        //No of wts would be equal to the no of nodes in the previous layer
                        node.Weights = _CreateArrayOfWeights(nodesperlayer[layerindex - 1]);  //new Weight[nodesperlayer[layerindex - 1]];
                    }
                    nodes.Add(node);
                    node.Bias = new Weight();
                }
                layer.Nodes = nodes.ToArray();
                layers.Add(layer);
            }
            mlp.Layers = layers.ToArray();
            return mlp;
        }

        private static Weight[] _CreateArrayOfWeights(int inputs)
        {
            List<Weight> wts = new List<Weight>();
            for (int index = 0; index < inputs; index++) wts.Add(new Weight());
            return wts.ToArray();
        }

        /// <summary>
        /// Compute the deltas at each node
        ///     
        /// </summary>
        /// <param name="perceptron"></param>
        /// <param name="ctx"></param>
        internal static void DoBackwardPassComputeDeltas(MultilayerPerceptron network, VectorPropagationContext ctx)
        {
            for (int layerindex = network.Layers.Length-1; layerindex >= 0; layerindex--)
            {
                Layer layerCurrent = network.Layers[layerindex];
                Layer layerAhead = null;
                int countOfNodes = layerCurrent.Nodes.Length;
                if (layerindex == network.Layers.Length -1 )
                {
                    //We are on the last layer
                    ctx.OutputLayerErrors = new double[countOfNodes];
                    for (int nodeindex = 0; nodeindex < countOfNodes; nodeindex++)
                    {
                        Neuron nodeCurrent = layerCurrent.Nodes[nodeindex];
                        double outputExpected = ctx.Vector.Outputs[nodeindex];
                        double outputActual = ctx.NodeActivationCache[nodeCurrent.GetID()];
                        double errorAtNode = outputExpected - outputActual;
                        //ctx.NodeActivationCache[node.GetID()] = activation;
                        double dotproduct = ctx.NodeDotProductsCache[nodeCurrent.GetID()];
                        double derivative=ComputeDerivativeOfActivation(layerCurrent, nodeCurrent, dotproduct, outputActual);
                        double deltaNode = -errorAtNode * derivative;
                        ctx.NodeDeltaCache[nodeCurrent.GetID()] = deltaNode;
                    }
                    double mse = 0.5*ctx.OutputLayerErrors.Select(e => e * e).Sum();
                    ctx.MeanSquaredError = mse;
                }
                else
                {
                    //We are on the intermediate layers
                    layerAhead = network.Layers[layerindex + 1];
                    int countOfNodesAhead = layerAhead.Nodes.Length;
                    for (int nodeindex = 0; nodeindex < countOfNodes; nodeindex++)
                    {
                        Neuron nodeCurrent = layerCurrent.Nodes[nodeindex];
                        double activation = ctx.NodeActivationCache[nodeCurrent.GetID()];
                        double dotproduct= ctx.NodeDotProductsCache[nodeCurrent.GetID()];
                        double summationOfDeltas = 0.0;
                        //For every node ahead, sum up the weight and delta of that node
                        for (int nodeindex_ahead = 0; nodeindex_ahead < countOfNodesAhead; nodeindex_ahead++)
                        {
                            Neuron nodeAhead = layerAhead.Nodes[nodeindex_ahead];
                            double wt_from_layer_current_to_ahead = nodeAhead.Weights[nodeindex].Value;
                            double deltaNodeAhead = ctx.NodeDeltaCache[nodeAhead.GetID()];
                            summationOfDeltas += wt_from_layer_current_to_ahead * deltaNodeAhead;
                        }
                        double derivative = ComputeDerivativeOfActivation(layerCurrent, nodeCurrent,dotproduct,activation);
                        ctx.NodeDeltaCache[nodeCurrent.GetID()] = summationOfDeltas*derivative;
                    }
                }
            }
        }
        /// <summary>
        /// Randomize the network wts between -1 and +1
        /// </summary>
        /// <param name="network"></param>
        public static void RandomizeNetworkWeights(MultilayerPerceptron network)
        {
            Neuron[] allNodes = network.Layers.SelectMany(l => l.Nodes).ToArray();
            foreach(var node in allNodes)
            {
                foreach(var wt in node.Weights)
                {
                    wt.Value = -1.0 + rnd.NextDouble() * 2.0;
                }
                node.Bias.Value = -1.0 + rnd.NextDouble() * 2.0;
            }
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
        /// <summary>
        /// Computes the derivative of the activation function - depending on the type of derivative
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="node"></param>
        /// <param name="dotproduct"></param>
        /// <param name="activation"></param>
        /// <returns></returns>
        public static double ComputeDerivativeOfActivation(Layer layer, Neuron node, double dotproduct, double activation)
        {
            if (layer.Activation == ActivationType.Sigmoid)
            {
                double output = activation * (1 - activation);
                return output;
            }
            else
            {
                throw new NotImplementedException($"This activation type is not yet implemented. {layer.Activation}");
            }
        }
        /// <summary>
        /// Serializes the network state to JSON
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        public static string SaveNetworkToJson(Perceptron.entity.MultilayerPerceptron network)
        {
            //var ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            var settings = new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };
            string json= Newtonsoft.Json.JsonConvert.SerializeObject(network, Newtonsoft.Json.Formatting.Indented,settings);
            return json;
        }
        public static Perceptron.entity.MultilayerPerceptron LoadNetworkFromJson(string json)
        {
            //var ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            var network = Newtonsoft.Json.JsonConvert.DeserializeObject<Perceptron.entity.MultilayerPerceptron>(json);
            return network;
        }
    }
}
