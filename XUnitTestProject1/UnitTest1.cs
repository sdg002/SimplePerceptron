using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using System.Diagnostics;

namespace XUnitTestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void BasicXOR()
        {
            Perceptron.interfaces.IBackPropagationAlgo trainer = new Perceptron.core.SGDTrainer();
            Perceptron.entity.MultilayerPerceptron networkXOR = Perceptron.core.Utils.CreateNetwork(10, 2, 2, 1);
            trainer.Vectors = util.Helper.GenerateTrainingPointsForXor();
            ///
            /// Randomize the network wts, ensure that the network produces atleast 1 error 
            /// so that we can test for success of training
            ///
            while (true)
            {
                Perceptron.core.Utils.RandomizeNetworkWeights(networkXOR);
                int errors = 0;
                foreach(Perceptron.entity.Vector vector in trainer.Vectors)
                {
                    double[] outputs = Perceptron.core.Utils.ComputeNetworkOutput(networkXOR, vector);
                    double[] outputsQuantized = outputs.Select(opt => (opt > 0.5) ? 1.0 : 0.0).ToArray();
                    if (outputsQuantized.SequenceEqual(vector.Outputs) == false) errors++;
                }
                Trace.WriteLine($"The network generated {errors} errors before training.");
                if (errors > 0) break;
            }
            trainer.Perceptron = networkXOR;
            trainer.NotificationEpochs = 10;
            trainer.OnNotifyProgressArgs += Trainer_OnNotifyProgressArgs;
            trainer.LearningRate = 0.1;
            trainer.Vectors = (new Perceptron.entity.Vector[] { }) as IEnumerable<Perceptron.entity.Vector>;
            trainer.Train();

            //you were here///keep implementing SGD , without bothering about filling in other functions
            //establish the signatures
            //try simialr exercise with BGD
        }

        private void Trainer_OnNotifyProgressArgs(object sender, Perceptron.interfaces.NotifyProgressArgs e)
        {
            e.Cancel = false;
        }
    }
}
