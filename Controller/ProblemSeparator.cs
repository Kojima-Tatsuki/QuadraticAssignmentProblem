using QAP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            text = Regex.Replace(text, "^[\r\n]+", string.Empty, RegexOptions.Multiline); // 先頭の改行を削除

            // 改行又は空白で分割して、int 配列へ変換する
            var lines = text.Trim()
                .Replace("  ", " ")
                .Split('\n', ' ');
            var nums = lines.Select(int.Parse)
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
