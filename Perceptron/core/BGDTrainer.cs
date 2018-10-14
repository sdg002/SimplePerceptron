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

        public void Train()
        {
            throw new NotImplementedException();
        }
    }
}
