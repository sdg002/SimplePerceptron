using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perceptron.entity;

namespace Perceptron.interfaces
{
    public interface IBackPropagationAlgo
    {
        /// <summary>
        /// Sets/gets the learning rate
        /// </summary>
        double LearningRate { get; set; }
        /// <summary>
        /// Sets/gets the multilayer perceptron object
        /// </summary>
        MultilayerPerceptron Perceptron { get; set; }
        /// <summary>
        /// The event OnNotifyProgress is fired after these many epochs have elapsed. 
        /// When set to less than equal to 0 then the event is never fired
        /// </summary>
        int NotificationEpochs { get; set; }
        /// <summary>
        /// Max no of epochs for which the training will run
        /// </summary>
        int MaxEpochs { get; set; }
        /// <summary>
        /// Returns the epochs that have completed
        /// </summary>
        int EpochsElapsed { get; }
        /// <summary>
        /// The collection of vectors which will be used by the backpropagation training algorithm
        /// </summary>
        IEnumerable<Vector> Vectors { get; set; }
        /// <summary>
        /// Notifies the caller about the progress of training.
        /// </summary>
        event EventHandler<NotifyProgressArgs> OnNotifyProgressArgs;
        //Commenting out because no functional need was found for such an event.
        ///// <summary>
        ///// Fired immediately before the beginning of every epoch
        ///// </summary>
        //event EventHandler<EpochBeginArgs> OnEpochBegin;
        /// <summary>
        /// Implements the training algorithm
        /// </summary>
        void Train();

        //Commenting out because this method is centrally implemented in the Utils class
        ///// <summary>
        ///// Computes the output from the final layers using the specified vector.
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //double[] ComputeNetworkOutput(entity.Vector input);
    }
}
