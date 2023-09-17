using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tremendous1192.SelfEmployed.MatrixSharp;

namespace QAP.Model
{
    internal class FlowModel
    {
        public Matrix DistanceMatrix { get; init; }

        private FlowModel(Matrix distanceMatrix)
        {
            DistanceMatrix = distanceMatrix;
        }

        public FlowModel(in double[,] array) : this(new Matrix(array)) { }
    }
}
