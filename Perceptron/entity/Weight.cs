using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Perceptron.entity
{
    public class Weight
    {
        private static long _idtracker = 1;
        long _id;
        private double _value;
        /// <summary>
        /// Gets/sets the value of the weight
        /// </summary>
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public Weight()
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


    }
}
