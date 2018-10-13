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
            Perceptron.entity.MultilayerPerceptron networkXOR = util.Helper.CreateXORNetwork();
            trainer.Vectors = util.Helper.GenerateTrainingPointsForXor();
            trainer.Perceptron = networkXOR;
            ///
            /// Randomize the network wts, ensure that the network produces atleast 1 error 
            /// so that we can test for success of training
            ///
            while (true)
            {
                int errors = 0;
                Perceptron.entity.Vector[] vectorsWhichFailed = util.Helper.EvaluateVectors(trainer.Perceptron, trainer.Vectors);
                errors = vectorsWhichFailed.Count();
                Trace.WriteLine($"TRAIN DATA - The network generated {errors} errors before training.");
                if (errors > 0) break;
                Perceptron.core.Utils.RandomizeNetworkWeights(networkXOR);
            }
            string jsonUntrained = Perceptron.core.Utils.SaveNetworkToJson(networkXOR);
            System.IO.File.WriteAllText("XOR.untrained.json", jsonUntrained);
            trainer.NotificationEpochs = 10;
            trainer.LearningRate = 0.3;
            trainer.MaxEpochs = 5000;
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
            string jsonTrained = Perceptron.core.Utils.SaveNetworkToJson(networkXOR);
            System.IO.File.WriteAllText("XOR.trained.json", jsonTrained);
        }
        /// <summary>
        /// Scenario:
        ///     We are testing the notification eventing mechanism
        /// </summary>
        [Fact]
        public void NotifyProgress()
        {

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
