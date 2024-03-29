﻿using System;
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
                errors = vectorsWhichFailed.Count();
                Trace.WriteLine($"TRAIN DATA - The network generated {errors} errors after training.");
                Assert.True(errors == 0);
            }
            ///
            /// Evaluate against test data
            ///
            {
                int errors = 0;
                var vectorsTest= util.Helper.GenerateTrainingPointsForXor();
                Perceptron.entity.Vector[] vectorsWhichFailed = util.Helper.EvaluateVectors(trainer.Perceptron, vectorsTest);
                errors = vectorsWhichFailed.Count();
                Trace.WriteLine($"TEST DATA - The network generated {errors} errors after training.");
                Assert.True(errors == 0);
            }
            string jsonTrained = Perceptron.core.Utils.SaveNetworkToJson(networkXOR);
            System.IO.File.WriteAllText("XOR.trained.json", jsonTrained);
        }
        /// <summary>
        /// Scenario:
        ///     We are testing the notification eventing mechanism.
        ///     NotificationEpochs=0
        /// Expected
        ///     No events should be fired
        /// </summary>
        [Fact]
        public void NotifyProgress_NotificationEpochs_is_zero()
        {
            Perceptron.interfaces.IBackPropagationAlgo trainer = new Perceptron.core.SGDTrainer();
            Perceptron.entity.MultilayerPerceptron networkXOR = util.Helper.CreateXORNetwork();
            trainer.Vectors = util.Helper.GenerateTrainingPointsForXor();
            trainer.Perceptron = networkXOR;
            trainer.LearningRate = 0.3;
            trainer.MaxEpochs = 5000;
            trainer.NotificationEpochs = 0;
            int countOfEventsFired = 0;
            Action<object,Perceptron.interfaces.NotifyProgressArgs> fnNotifyHandler=delegate(object sender, Perceptron.interfaces.NotifyProgressArgs args)
            {
                countOfEventsFired++;
            };
            trainer.OnNotifyProgressArgs += new EventHandler<Perceptron.interfaces.NotifyProgressArgs>(fnNotifyHandler);
            trainer.Train();
            var vectorsTest = util.Helper.GenerateTrainingPointsForXor();
            Perceptron.entity.Vector[] vectorsWhichFailed = util.Helper.EvaluateVectors(trainer.Perceptron, vectorsTest);
            Trace.WriteLine($"TEST DATA - The network generated {vectorsWhichFailed.Count()} errors after training.");
            ///
            /// No events should have been fired
            ///
            Assert.True(countOfEventsFired == 0);
            Assert.True(vectorsWhichFailed.Count() == 0);
            Assert.True(trainer.EpochsElapsed == trainer.MaxEpochs);
        }
        /// <summary>
        /// Scenario:
        ///     We are testing whether the notification mechanism is fired at the correct intervals
        ///     NotificationEpochs=2
        /// Expected
        ///     Count of events fired > 0 and should be 1/2 of EpochsElapsed
        /// </summary>
        [Fact]
        public void NotifyProgress_NotificationEpochs_is_2()
        {
            Perceptron.interfaces.IBackPropagationAlgo trainer = new Perceptron.core.SGDTrainer();
            Perceptron.entity.MultilayerPerceptron networkXOR = util.Helper.CreateXORNetwork();
            trainer.Vectors = util.Helper.GenerateTrainingPointsForXor();
            trainer.Perceptron = networkXOR;
            trainer.LearningRate = 0.3;
            trainer.MaxEpochs = 5000;
            trainer.NotificationEpochs = 2;
            int countOfEventsFired = 0;
            Action<object, Perceptron.interfaces.NotifyProgressArgs> fnNotifyHandler = delegate (object sender, Perceptron.interfaces.NotifyProgressArgs args)
            {
                Assert.True(Object.ReferenceEquals(sender,trainer));
                Assert.True(args.Vectors.Length == trainer.Vectors.Count());
                //Assert.True(args.Epochs == trainer.EpochsElapsed);
                countOfEventsFired++;
            };
            trainer.OnNotifyProgressArgs += new EventHandler<Perceptron.interfaces.NotifyProgressArgs>(fnNotifyHandler);
            trainer.Train();
            var vectorsTest = util.Helper.GenerateTrainingPointsForXor();
            Perceptron.entity.Vector[] vectorsWhichFailed = util.Helper.EvaluateVectors(trainer.Perceptron, vectorsTest);
            Trace.WriteLine($"TEST DATA - The network generated {vectorsWhichFailed.Count()} errors after training.");
            ///
            /// Some events should have been fired
            ///
            Assert.True(countOfEventsFired > 0);
            Assert.True(countOfEventsFired == trainer.EpochsElapsed/2);
            Assert.True(vectorsWhichFailed.Count() == 0);
            Assert.True(trainer.EpochsElapsed == trainer.MaxEpochs);
        }
        /// <summary>
        /// Scenario:
        ///     We are testing whether the notification mechanism can be used to halt the training by setting e.Cancel=true
        ///     NotificationEpochs=2
        /// Expected
        ///     Count of events fired > 0 and should be 1/2 of EpochsElapsed
        /// </summary>
        [Fact]
        public void NotifyProgress_StopTraining_WhenTrainingIsSuccessful()
        {
            Perceptron.interfaces.IBackPropagationAlgo trainer = new Perceptron.core.SGDTrainer();
            Perceptron.entity.MultilayerPerceptron networkXOR = util.Helper.CreateXORNetwork();
            trainer.Vectors = util.Helper.GenerateTrainingPointsForXor();
            trainer.Perceptron = networkXOR;
            trainer.LearningRate = 0.3;
            trainer.MaxEpochs = 5000;
            trainer.NotificationEpochs = 2;
            int countOfEventsFired = 0;
            ///
            /// Evaluate the success of the training data inside the event handler. When zero failures is reached halt the training
            ///
            bool haltedInEventHandler = false;
            Action<object, Perceptron.interfaces.NotifyProgressArgs> fnNotifyHandler = delegate (object sender, Perceptron.interfaces.NotifyProgressArgs args)
            {
                countOfEventsFired++;
                double mseTotal = args.Vectors.Sum(v => v.MeanSquaredError);
                int classificationerrors = 0;
                foreach(var vec in args.Vectors)
                {
                    double[] outputActual = vec.Outputs;
                    double[] outputActualQuantized=outputActual.Select(opt => (opt > 0.5) ? 1.0 : 0.0).ToArray();
                    double[] outputExpected = vec.Vector.Outputs;
                    if (outputActualQuantized.SequenceEqual(outputExpected) == false) classificationerrors++;
                }
                Trace.WriteLine($"  Epoch={trainer.EpochsElapsed} MSE={mseTotal}, Classification errors={classificationerrors}");
                if (classificationerrors == 0)
                {
                    Trace.WriteLine("Halting training because zero errors were reached");
                    args.Cancel = true;
                    haltedInEventHandler = true;
                }
            };
            trainer.OnNotifyProgressArgs += new EventHandler<Perceptron.interfaces.NotifyProgressArgs>(fnNotifyHandler);
            trainer.Train();
            ///
            /// Some events should have been fired
            ///
            Assert.True(haltedInEventHandler);
            Assert.True(countOfEventsFired > 0);
            Assert.True(countOfEventsFired == trainer.EpochsElapsed / 2);
        }


    }
}
