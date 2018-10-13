using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Perceptron.entity;

namespace XUnitTestProject1.util
{
    public class Helper
    {
        static Random _rnd = new Random(DateTime.Now.Second);
        internal static IEnumerable<Vector> GenerateTrainingPointsForXor()
        {
            List<Vector> xordatapoints = new List<Vector>();
            //TODO implementation required
            //provide some points for the unit test to work
            double[] CLASS1 = new double[] { 1 };
            double[] CLASS0 = new double[] { 0 };
            double radius = 0.2;
            Vector[] pointsAround_1_1 = GeneratePointsAroundXY(1.0, 1.0, radius, 10,CLASS0, "1-1");

            Vector[] pointsAround_0_0 = GeneratePointsAroundXY(0.0, 0.0, radius, 10,CLASS0, "0-0");

            Vector[] pointsAround_1_0 = GeneratePointsAroundXY(1.0, 0.0, radius, 10,CLASS1,"1-0");

            Vector[] pointsAround_0_1 = GeneratePointsAroundXY(0.0, 1.0, radius, 10,CLASS1,"0-1");

            xordatapoints.AddRange(pointsAround_0_0);
            xordatapoints.AddRange(pointsAround_0_1);
            xordatapoints.AddRange(pointsAround_1_0);
            xordatapoints.AddRange(pointsAround_1_1);
            return xordatapoints;
        }
        /// <summary>
        /// Internal method to create a XOR network and initialize with weights
        /// Not all wts produce converge to a solution. Hence this step of recording wts which led to successful training
        /// </summary>
        /// <returns></returns>
        internal static MultilayerPerceptron CreateXORNetwork()
        {
            var network=Perceptron.core.Utils.CreateNetwork(2, 2, 1);
            network.Layers[0].Nodes[0].Weights[0].Value = 0.015732870910192265;
            network.Layers[0].Nodes[0].Weights[1].Value = -0.45681309488453581;
            network.Layers[0].Nodes[0].Bias.Value = -0.78474869475921083;

            network.Layers[0].Nodes[1].Weights[0].Value = -0.76899536595167373;
            network.Layers[0].Nodes[1].Weights[1].Value = 0.3320003828648479;
            network.Layers[0].Nodes[1].Bias.Value = 0.77580667276671478;

            network.Layers[1].Nodes[0].Weights[0].Value = -0.7763338451163535;
            network.Layers[1].Nodes[0].Weights[1].Value = 0.081352289338294526;
            network.Layers[1].Nodes[0].Bias.Value = -0.23060310782427118;

            return network;
        }

        /// <summary>
        /// Internal method to evaluate the output of a vector on a trained network and then compare with expected output
        /// </summary>
        /// <param name="perceptron"></param>
        /// <param name="vectors"></param>
        /// <returns></returns>
        internal static Vector[] EvaluateVectors(MultilayerPerceptron perceptron, IEnumerable<Vector> vectors)
        {
            List<Vector> vectorsFailed = new List<Vector>();
            foreach (Perceptron.entity.Vector vector in vectors)
            {
                double[] outputs = Perceptron.core.Utils.ComputeNetworkOutput(perceptron, vector);
                double[] outputsQuantized = outputs.Select(opt => (opt > 0.5) ? 1.0 : 0.0).ToArray();
                if (outputsQuantized.SequenceEqual(vector.Outputs) == false) vectorsFailed.Add(vector);
            }
            return vectorsFailed.ToArray();
        }

        /// <summary>
        /// Generate the specified number of random points around the specified coordinates.
        /// </summary>
        /// <param name="xCentroidOfCluster">Centroid of cluster</param>
        /// <param name="yCentroidOfCluster">Centroid of cluster</param>
        /// <param name="radiusofcluster">Determines the maximum extent from the cluster </param>
        /// <param name="count">Number of points to generate</param>
        /// <param name="comments">The comment string that will be tagged to this vector for easy identification</param>
        /// <param name="output">The expected output vector</param>
        /// <returns></returns>
        private static Vector[] GeneratePointsAroundXY(
            double xCentroidOfCluster, double yCentroidOfCluster,double radiusofcluster ,int count,
            double[] output,string comments)
        {
            Thread.Sleep(100);
            List<Vector> pts = new List<Vector>();
            for (int i = 0; i < count; i++)
            {
                Vector pt = new Vector();
                Double x0 = (xCentroidOfCluster - radiusofcluster) + 2 * radiusofcluster * _rnd.NextDouble();
                Double y0 = (yCentroidOfCluster - radiusofcluster) + 2 * radiusofcluster * _rnd.NextDouble();
                pt.Inputs = new double[] { x0,y0 };
                pt.Outputs = output;
                pt.Comments = comments;
                pts.Add(pt);
            }
            return pts.ToArray();
        }
    }
}
