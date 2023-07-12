using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tremendous1192.SelfEmployed.MatrixSharp;

namespace QAP.Model
{
    internal class DistanceModel
    {
        public Matrix DistanceMatrix { get; init; }
        
        private DistanceModel(Matrix distanceMatrix)
        {
            DistanceMatrix = distanceMatrix;
        }

        public DistanceModel(in double[,] array) : this(new Matrix(array)) { }


    }
}
