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

        public event EventHandler<NotifyProgressArgs> OnNotifyProgressArgs;
        public event EventHandler<EpochBeginArgs> OnEpochBegin;

        public double[] ComputeNetworkOutput(Vector input)
        {
            throw new NotImplementedException();
        }

        public void Train()
        {
            throw new NotImplementedException();
        }
    }
}
