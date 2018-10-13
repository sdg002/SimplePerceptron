using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xunit;

namespace XUnitTestProject1
{
    /// <summary>
    /// All unit tests related to SGDTrainer class are here
    /// </summary>
    public class SGDTrainerTests
    {
        [Fact]
        public void BasicXOR()
        {
            Perceptron.interfaces.IBackPropagationAlgo trainer = new Perceptron.core.SGDTrainer();
            Perceptron.entity.MultilayerPerceptron networkXOR = Perceptron.core.Utils.CreateNetwork(2, 2, 1);
            trainer.Vectors = util.Helper.GenerateTrainingPointsForXor();
            trainer.Perceptron = networkXOR;
            ///
            /// Randomize the network wts, ensure that the network produces atleast 1 error 
            /// so that we can test for success of training
            ///
            while (true)
            {
                Perceptron.core.Utils.RandomizeNetworkWeights(networkXOR);
                int errors = 0;
                Perceptron.entity.Vector[] vectorsWhichFailed = util.Helper.EvaluateVectors(trainer.Perceptron, trainer.Vectors);
                errors = vectorsWhichFailed.Count();
                Trace.WriteLine($"TRAIN DATA - The network generated {errors} errors before training.");
                if (errors > 0) break;
            }
            trainer.NotificationEpochs = 10;
            trainer.OnNotifyProgressArgs += Trainer_OnNotifyProgressArgs;
            trainer.LearningRate = 0.3;
            trainer.MaxEpochs = 25000;
            trainer.Train();
            ///
            /// Training complete - evaluate the results
            ///
            {
                int errors = 0;
                Perceptron.entity.Vector[] vectorsWhichFailed = util.Helper.EvaluateVectors(trainer.Perceptron, trainer.Vectors);
                Trace.WriteLine($"TRAIN DATA - The network generated {errors} errors after training.");
                errors = vectorsWhichFailed.Count();
                Assert.True(errors == 0);
            }
            ///
            /// Evaluate against test data
            ///
            {
                int errors = 0;
                var vectorsTest= util.Helper.GenerateTrainingPointsForXor();
                Perceptron.entity.Vector[] vectorsWhichFailed = util.Helper.EvaluateVectors(trainer.Perceptron, vectorsTest);
                Trace.WriteLine($"TEST DATA - The network generated {errors} errors after training.");
                errors = vectorsWhichFailed.Count();
                Assert.True(errors == 0);
            }
        }
        private void Trainer_OnNotifyProgressArgs(object sender, Perceptron.interfaces.NotifyProgressArgs e)
        {
            //e.Cancel = false;
            //int errors = 0;
            //Perceptron.entity.Vector[] vectorsWhichFailed = util.Helper.EvaluateVectors(trainer.Perceptron, trainer.Vectors);
            //Trace.WriteLine($"TEST DATA - The network generated {errors} errors after training.");
            //errors = vectorsWhichFailed.Count();
            //Assert.True(errors == 0);
        }
    }
}
