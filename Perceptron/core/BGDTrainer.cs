using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perceptron.entity;
using Perceptron.interfaces;

namespace Perceptron.core
{
    public class BGDTrainer : interfaces.IBackPropagationAlgo
    {
        int _epochsElapsed;
        public double LearningRate { get; set; }
        public MultilayerPerceptron Perceptron { get; set; }
        public int NotificationEpochs { get; set; }
        public IEnumerable<Vector> Vectors { get; set; }
        public int MaxEpochs { get; set; }
        public int EpochsElapsed
        {
            get
            {
                return _epochsElapsed;
            }
        }

        public event EventHandler<NotifyProgressArgs> OnNotifyProgressArgs;
        public event EventHandler<EpochBeginArgs> OnEpochBegin;
        /// <summary>
        /// This method will execute the training in batch mode. 
        /// All the vectors will be processed in a single batch
        /// </summary>
        public void Train()
        {
            for (int epochs=0;epochs<this.MaxEpochs;epochs++)
            {
                _epochsElapsed++;
                VectorPropagationContext[] wrappers = this.Vectors.Select(v => new VectorPropagationContext(v)).ToArray();
                Parallel.ForEach(wrappers, this.DoForwardAndBackProp);
                MergeWeights(wrappers);
                bool cancel = NotifyProgress(wrappers);
                if (cancel) return;
            }
        }
        /// <summary>
        /// The wt increments for each of the training vectors has been computed - no sum up the increments
        /// </summary>
        /// <param name="wrappers"></param>
        private void MergeWeights(VectorPropagationContext[] wrappers)
        {
            Neuron[] allNeurons = this.Perceptron.Layers.SelectMany(l => l.Nodes).ToArray();
            foreach(Neuron node in allNeurons)
            {
                foreach(Weight wt in node.Weights)
                {
                    long wtid = wt.GetID();
                    double sumOfWtIncrementsFromAllVectors = wrappers.Select(ctx => ctx.WeightUpdateCache[wtid]).Sum();
                    wt.Value = wt.Value+sumOfWtIncrementsFromAllVectors;
                }
                long biasid = node.Bias.GetID();
                double sumOfBiasIncrementsFromAllVectors = wrappers.Select(ctx => ctx.WeightUpdateCache[biasid]).Sum();
                node.Bias.Value = node.Bias.Value + sumOfBiasIncrementsFromAllVectors;
            }
        }

        private void DoForwardAndBackProp(VectorPropagationContext ctx)
        {
            core.Utils.DoForwardPass(this.Perceptron, ctx);
            core.Utils.DoBackwardPassComputeDeltas(this.Perceptron, ctx);
            this.ComputeWeightUpdates(ctx);
        }
        /// <summary>
        /// You have the deltas at every node. Now compute the wt updates.
        /// But, unlike SGD, hold on to the values. We will need to sum and the increments after the entire batch is done
        /// </summary>
        /// <param name="ctx"></param>
        private void ComputeWeightUpdates(VectorPropagationContext ctx)
        {
            for (int layerindex = 0; layerindex < this.Perceptron.Layers.Length; layerindex++)
            {
                Layer layerCurrent = this.Perceptron.Layers[layerindex];
                Layer layerPrevious = null;
                int countOfNodes = layerCurrent.Nodes.Length;
                double[] incomingvalues = null;//Values coming into this layer
                if (layerindex == 0)
                {
                    //We are on the first layer
                    incomingvalues = ctx.Vector.Inputs;
                }
                else
                {
                    //We are on intermediate layers
                    layerPrevious = this.Perceptron.Layers[layerindex - 1];
                    incomingvalues = layerPrevious.Nodes.Select(nd => ctx.NodeActivationCache[nd.GetID()]).ToArray();
                }
                for (int nodeindex = 0; nodeindex < countOfNodes; nodeindex++)
                {
                    Neuron node = layerCurrent.Nodes[nodeindex];
                    double deltaAtNode = ctx.NodeDeltaCache[node.GetID()];
                    int noOfWts = node.Weights.Length;
                    for (int wtindex = 0; wtindex < noOfWts; wtindex++)
                    {
                        Weight wt = node.Weights[wtindex];
                        double outputFromPreviousNodeThroughThisWeight = incomingvalues[wtindex];
                        double derivative = deltaAtNode * outputFromPreviousNodeThroughThisWeight;
                        double wt_existing = wt.Value;
                        double wt_increment = -this.LearningRate * derivative;
                        //double wt_new = wt_existing + wt_increment;
                        //wt.Value = wt_new;
                        ctx.WeightUpdateCache[wt.GetID()] = wt_increment;
                    }
                    double bias_derivative = deltaAtNode * 1.0;
                    double bias_increment = -this.LearningRate * bias_derivative;
                    //node.Bias.Value = node.Bias.Value + bias_increment;
                    ctx.WeightUpdateCache[node.Bias.GetID()] = bias_increment;
                }
            }
        }
        private bool NotifyProgress(VectorPropagationContext[] wrappers)
        {
            if (this.OnNotifyProgressArgs == null) return false;
            if (this.NotificationEpochs <= 0) return false;
            if (_epochsElapsed == 0) return false;
            if (_epochsElapsed % this.NotificationEpochs != 0) return false;
            NotifyProgressArgs args = new NotifyProgressArgs
            {
                Cancel = false,
                Vectors = wrappers.ToArray()
                //Epochs =_epochsElapsed
            };
            this.OnNotifyProgressArgs(this, args);
            return args.Cancel;
        }
    }
}
