namespace QAP.CharacterUserInterface
{
    internal class DirectoryController
    {
        public string ParentPath { get; init; }

        private readonly char SeparatorChar;

        public DirectoryController()
        {
            ParentPath = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName ?? throw new Exception("パスの取得に失敗しました");
            SeparatorChar = Path.DirectorySeparatorChar;

            // Dir作成
            if (!Directory.Exists(GetProblemDirPath()))
                Directory.CreateDirectory(GetProblemDirPath());
            if (!Directory.Exists(GetResultDirPath()))
                Directory.CreateDirectory(GetResultDirPath());
        }

        public string[] GetProblemsPath()
        {
            var problemDirPath = GetProblemDirPath();

            return Directory.GetFiles(problemDirPath, "*.dat", SearchOption.TopDirectoryOnly);
        }

        public string GetProblemDirPath()
        {
            return ParentPath + SeparatorChar + DirectoryConst.PROBLEM_DIR_NAME + SeparatorChar;
        }

        public string GetResultDirPath()
        {
            return ParentPath + SeparatorChar + DirectoryConst.RESULT_DIR_NAME + SeparatorChar;
        }
    }
}
