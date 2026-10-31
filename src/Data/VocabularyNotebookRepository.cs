using LiteDB;

namespace NihongoHub.Data
{
    public static class VocabularyNotebookRepository
    {
        public static Models.Entities.VocabularyNotebook? FindVocabularyNotebookById(int id)
        {
            return Init.DatabaseInit.Current.VocabularyNotebooks.FindById(id);
        }
        public static List<Models.Entities.VocabularyNotebook> FindVocabularyNotebooksByUserId(int userId)
        {
            return Init.DatabaseInit.Current.VocabularyNotebooks.Find(x => x.userid == userId).ToList();
        }

        public static bool Insert(Models.Entities.VocabularyNotebook vocabularyNotebook,ref string errorMessage,ref Logging.LogCategory errorCategory)
        {
            errorMessage = "";
            errorCategory = Logging.LogCategory.None;

            try
            {
                Init.DatabaseInit.Current.VocabularyNotebooks.Insert(vocabularyNotebook);
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]插入单词本时数据库错误",ex);

                errorMessage="插入单词本时数据库错误";
                errorCategory = Logging.LogCategory.Database;
                return false;
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]插入单词本时发生未知错误",ex);

                errorMessage="插入单词本时发生未知错误";
                errorCategory = Logging.LogCategory.Unknown;
                return false;
            }
            return true;
        }
        public static bool Update(Models.Entities.VocabularyNotebook vocabularyNotebook,ref string errorMessage,ref Logging.LogCategory errorCategory)
        {
            errorMessage = "";
            errorCategory = Logging.LogCategory.None;

            try
            {
                if (!Init.DatabaseInit.Current.VocabularyNotebooks.Update(vocabularyNotebook))
                {
                    errorMessage = "未能更改单词本数据(可能是单词本不存在)";
                    errorCategory = Logging.LogCategory.Database;
                    return false;
                }
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]修改单词本时数据库错误",ex);

                errorMessage="修改单词本时数据库错误";
                errorCategory = Logging.LogCategory.Database;
                return false;
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]修改单词本时发生未知错误",ex);

                errorMessage="修改单词本时发生未知错误";
                errorCategory = Logging.LogCategory.Unknown;
                return false;
            }
            return true;
        }
    }
}