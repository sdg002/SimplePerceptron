﻿using System;
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
    }
}