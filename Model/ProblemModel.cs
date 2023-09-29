using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tremendous1192.SelfEmployed.MatrixSharp;

namespace QAP.Model
{
    internal class ProblemModel
    {
        private DistanceModel Factory { get; init; }
        private FlowModel Flow { get; init; }

        public ProblemModel(DistanceModel factoy, FlowModel flow)
        {
            Factory = factoy;
            Flow = flow;
        }

        public double GetScore(IReadOnlyList<int> indexList)
        {
            // sum(f[i,j]*d[k,l]*x[i,k]*x[j,l],(i,j,k,l));

            var x = IndexMatrixFromList(indexList);

            var xe1 = x * new Matrix(new double[,] { { 1, 1, 1, 1}, { 1, 1, 1, 1}, { 1, 1, 1, 1}, { 1, 1, 1, 1} });
            var xe2 = x.Transpose() * new Matrix(new double[,] { { 1, 1, 1, 1}, { 1, 1, 1, 1}, { 1, 1, 1, 1}, { 1, 1, 1, 1} });
 
            var res = x * Factory.DistanceMatrix * x.Transpose();

            return sum(Flow.DistanceMatrix, res);

            // throw new NotImplementedException();
        }

        private Matrix IndexMatrixFromList(IReadOnlyList<int> list)
        {
            return new Matrix(new double[,] { { 1, 0, 0, 0 }, { 0, 0, 1, 0 }, { 0, 1, 0, 0 }, { 0, 0, 0, 1 } });
            // throw new NotImplementedException();
        }

        private double sum(Matrix x, Matrix y)
        {
            double res = 0;

            for (var i = 0; i < x.Row; i++)
            {
                for (var j = 0; j < x.Column; j++)
                {
                    res += x[i, j] * y[i, j];
                }
            }
            return res;
        }
    }
}
