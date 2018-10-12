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
        public double LearningRate { get ; set ; }
        public MultilayerPerceptron Perceptron { get ; set ; }
        public int NotificationEpochs { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IEnumerable<Vector> Vectors { get ; set ; }
        public int MaxEpochs { get ; set ; }

        public event EventHandler<NotifyProgressArgs> OnNotifyProgressArgs;
        public event EventHandler<EpochBeginArgs> OnEpochBegin;

        public double[] ComputeNetworkOutput(Vector input)
        {
            throw new NotImplementedException();
        }

        public void Train()
        {
            for(int epochs=0;epochs<this.MaxEpochs;epochs++)
            {
                foreach (Vector vec in this.Vectors)
                {
                    VectorPropagationContext ctx = new VectorPropagationContext(vec);
                    core.Utils.DoForwardPass(this.Perceptron, ctx);
                    core.Utils.DoBackwardPass(this.Perceptron, ctx);
                    core.Utils.UpdateNetworkWeights(this.Perceptron, ctx);
                }
                you were here.
            }
            throw new NotImplementedException();
        }
    }
}
