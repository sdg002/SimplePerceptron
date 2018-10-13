using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perceptron.entity;
using Perceptron.interfaces;

namespace Perceptron.core
{
    public class SGDTrainer : interfaces.IBackPropagationAlgo
    {
        int _epochsElapsed;
        public double LearningRate { get ; set ; }
        public MultilayerPerceptron Perceptron { get ; set ; }
        public int NotificationEpochs { get; set; }
        public IEnumerable<Vector> Vectors { get ; set ; }
        public int MaxEpochs { get ; set ; }
        public int EpochsElapsed
        {
            get
            {
                return _epochsElapsed;
            }
        }
        public event EventHandler<NotifyProgressArgs> OnNotifyProgressArgs;
        public event EventHandler<EpochBeginArgs> OnEpochBegin;

        public double[] ComputeNetworkOutput(Vector input)
        {
            throw new NotImplementedException();
        }

        public void Train()
        {
            _epochsElapsed = 0;
            for (int epochs=0;epochs<this.MaxEpochs;epochs++)
            {
                _epochsElapsed++;
                List<VectorPropagationContext> wrappers = new List<VectorPropagationContext>();
                foreach (Vector vec in this.Vectors)
                {
                    VectorPropagationContext ctx = new VectorPropagationContext(vec);
                    wrappers.Add(ctx);
                    core.Utils.DoForwardPass(this.Perceptron, ctx);
                    core.Utils.DoBackwardPassComputeDeltas(this.Perceptron, ctx);
                    this.ComputeWeightUpdates(ctx);
                }
                bool cancel=NotifyProgress( wrappers);
                if (cancel) return;
            }
        }
        /// <summary>
        /// You have the deltas at every node. Now compute the wt updates.
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
                        double wt_new = wt_existing + wt_increment;
                        wt.Value = wt_new;
                    }
                    double bias_derivative = deltaAtNode * 1.0;
                    double bias_increment = -this.LearningRate*bias_derivative;
                    node.Bias.Value = node.Bias.Value + bias_increment;
                }
            }
        }
        private bool NotifyProgress( List<VectorPropagationContext> wrappers)
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
