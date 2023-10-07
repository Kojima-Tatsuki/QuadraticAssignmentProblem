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

        public int GetScore(IReadOnlyList<int> indexList)
        {
            // sum(f[i,j]*d[k,l]*x[i,k]*x[j,l],(i,j,k,l));

            // F * X D X^Y

            var x = IndexMatrixFromList(indexList);

            var res = x * Factory.DistanceMatrix * x.Transpose();

            return (int)sum(Flow.DistanceMatrix, res) / 2;
        }

        private Matrix IndexMatrixFromList(IReadOnlyList<int> list)
        {
            double[,] matrix = new double[list.Count, list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                matrix[list[i] - 1, i] = 1;
            }

            return new Matrix(matrix);
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
