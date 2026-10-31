using LiteDB;

namespace NihongoHub.Data
{
    public static class WordRepository
    {
        public static Models.Entities.Word? FindWordById(int id)
        {
            return Init.DatabaseInit.Current.Words.FindById(id);
        }
        public static List<Models.Entities.Word> FindWordsByIds(List<int> ids, List<int> missingIds)
        {
            missingIds.Clear();

            List<Models.Entities.Word> wordList = new();

            foreach (int id in ids)
            {
                var word =  Init.DatabaseInit.Current.Words.FindById(id);
                if (word == null)
                {
                    missingIds.Add(id);
                    continue;
                }
                wordList.Add(word);
            }

            return wordList;
        }

        public static bool Insert(Models.Entities.Word word,ref string errorMessage,ref Logging.LogCategory errorCategory)
        {
            errorMessage = "";
            errorCategory = Logging.LogCategory.None;

            try
            {
                Init.DatabaseInit.Current.Words.Insert(word);
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]插入单词时数据库错误",ex);

                errorMessage="插入单词时数据库错误";
                errorCategory = Logging.LogCategory.Database;
                return false;
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]插入单词时发生未知错误",ex);

                errorMessage="插入单词时发生未知错误";
                errorCategory = Logging.LogCategory.Unknown;
                return false;
            }
            return true;
        }
        public static bool Update(Models.Entities.Word word,ref string errorMessage,ref Logging.LogCategory errorCategory)
        {
            errorMessage = "";
            errorCategory = Logging.LogCategory.None;
            try
            {
                if (!Init.DatabaseInit.Current.Words.Update(word))
                {
                    errorMessage = "未能更改单词数据(可能是单词不存在)";
                    errorCategory = Logging.LogCategory.Database;
                    return false;
                }
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]修改单词时数据库错误",ex);

                errorMessage="修改单词时数据库错误";
                errorCategory = Logging.LogCategory.Database;
                return false;
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]修改单词时发生未知错误",ex);

                errorMessage="修改单词时发生未知错误";
                errorCategory = Logging.LogCategory.Unknown;
                return false;
            }
            return true;
        }
    }
}