using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.entity
{
    public class Neuron
    {
        private static long _idtracker = 1;
        long _id;
        public Neuron()
        {
            _id = System.Threading.Interlocked.Increment(ref _idtracker);
        }
        /// <summary>
        /// Returns an unique ID - unique per session and does not get persisted when saving the network model
        /// </summary>
        /// <returns></returns>
        public long GetID()
        {
            
            return _id;
        }
        public Weight[] Weights { get; set; }
        public Weight Bias { get; set; }
        public override string ToString()
        {
            return $"Weights={Weights.Length}; Bias={this.Bias.Value}; ID={_id}";
        }
        public override bool Equals(object obj)
        {
            Neuron target = obj as Neuron;
            if (target == null) return false;
            if (this.Weights != null && target.Weights != null)
            {
                if (this.Weights.Length != target.Weights.Length) return false;
                for(int wtindex=0;wtindex<this.Weights.Length;wtindex++)
                {
                    double wtTarget = target.Weights[wtindex].Value;
                    double wtSource = this.Weights[wtindex].Value;
                    if (Math.Round(wtTarget, 4) != Math.Round(wtSource, 4)) return false;
                }
            }
            else
            {
                return false;
            }
            if (Math.Round(target.Bias.Value, 4) != Math.Round(this.Bias.Value, 4)) return false;
            return true;
        }
    }
}
