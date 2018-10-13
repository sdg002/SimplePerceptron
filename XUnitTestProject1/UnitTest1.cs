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
            Perceptron.entity.MultilayerPerceptron networkXOR = Perceptron.core.Utils.CreateNetwork(2,  2, 1);
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
            trainer.LearningRate = 0.3;
            trainer.MaxEpochs = 25000;
            trainer.Train();
            ///
            /// Training complete - evaluate the results
            ///
            {
                int errors = 0;
                foreach (Perceptron.entity.Vector vector in trainer.Vectors)
                {
                    double[] outputs = Perceptron.core.Utils.ComputeNetworkOutput(networkXOR, vector);
                    double[] outputsQuantized = outputs.Select(opt => (opt > 0.5) ? 1.0 : 0.0).ToArray();
                    if (outputsQuantized.SequenceEqual(vector.Outputs) == false) errors++;
                }
                Trace.WriteLine($"The network generated {errors} errors after training.");
                Assert.True(errors == 0);
            }
        }

        private void Trainer_OnNotifyProgressArgs(object sender, Perceptron.interfaces.NotifyProgressArgs e)
        {
            e.Cancel = false;
        }
        [Fact]        
        public void CreateNewNetwork()
        {
            var mlp = Perceptron.core.Utils.CreateNetwork(3, 4, 8, 2);
            Assert.True(mlp.Layers.Length ==  3);
            Assert.True(mlp.Inputs == 3);
            Assert.True(mlp.Layers[0].Nodes.Length == 4);
            Assert.True(mlp.Layers[1].Nodes.Length == 8);
            Assert.True(mlp.Layers[2].Nodes.Length == 2);
            for (int layerindex = 0; layerindex < mlp.Layers.Length; layerindex++)
            {
                var nodes = mlp.Layers[layerindex].Nodes;
                foreach(var node in nodes)
                {
                    if (layerindex == 0)
                    {
                        //first layer
                        Assert.True(node.Weights.Length == mlp.Inputs);
                        foreach (var wt in node.Weights) Assert.NotNull(wt);
                    }
                    else
                    {
                        //hidden layer or output layer
                        Assert.True(node.Weights.Length == mlp.Layers[layerindex - 1].Nodes.Length);
                        foreach (var wt in node.Weights) Assert.NotNull(wt);
                    }
                    Assert.NotNull(node.Bias);
                }
            }
        }
        [Fact]
        void ComputeActivationSigmoid()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            Perceptron.entity.Layer l = new Perceptron.entity.Layer();
            l.Activation = Perceptron.entity.ActivationType.Sigmoid;

            double dotproduct = rnd.NextDouble();
            double activationActual=Perceptron.core.Utils.ComputeActivation(l, null,dotproduct);
            double activationExpected = 1 / (1 + Math.Exp(-dotproduct));
            Assert.Equal(activationActual, activationActual, 5);
        }
        [Fact]
        void ComputeDerivativeSigmoid()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            Perceptron.entity.Layer l = new Perceptron.entity.Layer();
            l.Activation = Perceptron.entity.ActivationType.Sigmoid;

            double activation = rnd.NextDouble();
            double derivativeActual = Perceptron.core.Utils.ComputeDerivativeOfActivation(l, null, 0,activation);
            double derivativeExpected = activation * (1 - activation);
            Assert.Equal(derivativeActual, derivativeActual, 5);
        }
        [Fact]
        void ComputeNetworkOutput()
        {
            throw new NotImplementedException();
        }
        [Fact]
        void SaveNetwork2Json()
        {
            throw new NotImplementedException();
        }
    }
}
