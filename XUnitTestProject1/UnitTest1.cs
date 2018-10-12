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
                    }
                    else
                    {
                        //hidden layer or output layer
                        Assert.True(node.Weights.Length == mlp.Layers[layerindex - 1].Nodes.Length);
                    }

                }
            }
        }
    }
}
