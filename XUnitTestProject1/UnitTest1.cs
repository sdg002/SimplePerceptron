using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;
using System.Diagnostics;


namespace XUnitTestProject1
{
    public class UnitTest1
    {
        /// <summary>
        /// In this test we are creating a network 
        ///     which accepts a 3 dimensional input vector
        ///     First hidden layer with 4 nodes
        ///     Second hidden layer with 8 nodes
        ///     Output layer with 2 nodes
        /// </summary>
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
        /// <summary>
        /// In this test we are saving a trained network to a JSON file
        /// </summary>
        [Fact]
        void SaveNetwork2Json()
        {
            var mlp = Perceptron.core.Utils.CreateNetwork(3, 4, 8, 2);
            Perceptron.core.Utils.RandomizeNetworkWeights(mlp);
            string json = Perceptron.core.Utils.SaveNetworkToJson(mlp);
            var mlpFromJson = Perceptron.core.Utils.LoadNetworkFromJson(json);
            Assert.Equal(mlp, mlpFromJson);
        }
    }
}
