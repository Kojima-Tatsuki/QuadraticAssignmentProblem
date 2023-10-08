using QAP.Model;
using System.Text.RegularExpressions;

namespace QAP.Controller
{
    // string から ProblemModel に変換する
    internal class ProblemSeparator
    {
        // 読み込み元の記述形式
        // [配列の要素数]
        // [距離配列]
        // [流量配列]
        public static ProblemModel ToModel(string text)
        {
            text = text.Replace("\r\n", "\n").Replace("\r", "\n"); // 改行コードを統一
            text = Regex.Replace(text, "[\n]+", " ", RegexOptions.Multiline); // 連続する改行を1つの空白に変換

            // 改行又は空白で分割して、int 配列へ変換する
            var lines = Regex.Replace(text, "[ ]+", " ", RegexOptions.Multiline).Split(' ');
            var nums = lines.Where(x => x != "").Select(int.Parse)
                .ToArray();

            var length = nums[0]; // 配列の要素数            

            var currentIndex = 1; // 要素数の次の行から読み込む

            // 距離行列
            var distanceArray = new double[length, length];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    distanceArray[i, j] = nums[currentIndex];
                    currentIndex++;
                }
            }

            // 流量行列
            var flowArray = new double[length, length];
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    flowArray[i, j] = nums[currentIndex];
                    currentIndex++;
                }
            }

            return new ProblemModel(
                new DistanceModel(distanceArray),
                new FlowModel(flowArray)
            );
        }
    }
}
