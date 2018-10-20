using Perceptron.entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trainer
{
    /// <summary>
    /// Iris dataset trainer
    /// </summary>
    public class Iris
    {
        public const string IRIS_CLASS_SETOSA= "Iris-setosa";
        public const string IRIS_CLASS_VERSICOLOR = "Iris-versicolor";
        public const string IRIS_CLASS_VIRGINICA = "Iris-virginica";
        public string[] IRIS_CLASSES = new string[] 
        {
            IRIS_CLASS_SETOSA,
            IRIS_CLASS_VERSICOLOR,
            IRIS_CLASS_VIRGINICA
        };
        public void Train(Dictionary<string,string> args)
        {
            Trace.WriteLine("Iris.Train");
            AllVectors allVectors = LoadIrisVectorsFromFile(@"iris\Iris.csv",0.8);
            MultilayerPerceptron network = CreateBlankNetwork();
            Perceptron.core.BGDTrainer trainer = new Perceptron.core.BGDTrainer();
            trainer.LearningRate = 0.2;
            trainer.NotificationEpochs = 10;
            trainer.MaxEpochs = 10000;
            trainer.OnNotifyProgressArgs += Trainer_OnNotifyProgressArgs;
        }

        private MultilayerPerceptron CreateBlankNetwork()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Training and testing vectors will be loaded from the specified file
        /// Each of the dimensions will be normalized using MIN-MAX approach
        /// </summary>
        /// <param name="filename">Path to the file with IRIS training data</param>
        /// <param name="fractionTraining">The fraction of vectors out of all those found in the file which will be used for training. The rest will be used for testing</param>
        /// <returns></returns>
        private AllVectors LoadIrisVectorsFromFile(string filename, double fractionTraining)
        {
            string[] allLines = System.IO.File.ReadAllLines("iris\\iris.csv");
            List<Vector> vectorsUnnormalized = new List<Vector>();
            foreach(string line in allLines)
            {
                string[] fragments = line.Split(',');
                Vector vec = new Vector();
                vec.Comments = line;
                vec.Outputs = CreateExpectedOutputVectorFromIrisClassName(fragments[4].Trim());
                vec.Inputs = new double[]
                {
                    double.Parse(fragments[0]),
                    double.Parse(fragments[1]),
                    double.Parse(fragments[2]),
                    double.Parse(fragments[3])
                };
                vectorsUnnormalized.Add(vec);

            }
            you were here
            //TODO normalize the vectors
            //TODO split into training and testing
            throw new NotImplementedException();
        }
        /// <summary>
        /// Depending on the name of the IRISI class, a 3 dimensional vector will be returned back
        /// Depending on the class name, only of the 1 bits of the 3dimensional vector will be 1, rest will be zero
        /// </summary>
        /// <param name="irisclassname">The name of the IRIS class , as it is in the CSV</param>
        /// <returns></returns>
        private double[] CreateExpectedOutputVectorFromIrisClassName(string irisclassname)
        {
            List<double> outputs = new List<double>();
            foreach(string classname in IRIS_CLASSES)
            {
                if (classname.ToLower() == irisclassname.ToLower())
                {
                    outputs.Add(1);
                }
                else
                {
                    outputs.Add(0);
                }
            }
            return outputs.ToArray();
        }
        private void Trainer_OnNotifyProgressArgs(object sender, Perceptron.interfaces.NotifyProgressArgs args)
        {
            Perceptron.interfaces.IBackPropagationAlgo trainer = sender as Perceptron.interfaces.IBackPropagationAlgo;
            double mseTotal = args.Vectors.Sum(v => v.MeanSquaredError);
            Vector[] vectorsFailed = Utils.GetFailedVectors(args.Vectors);
            //TODO also test for Test vectors
            Trace.WriteLine($"  Epoch={trainer.EpochsElapsed} MSE={mseTotal}, Classification errors={vectorsFailed.Count()}");
        }
    }
    /// <summary>
    /// Holds a list of training vectors and testing vectors
    /// </summary>
    public class AllVectors
    {
        public Vector[] Training { get; set; }
        public Vector[] Test { get; set; }
    }
}
