using LiteDB;

namespace NihongoHub.Data
{
    public static class VocabularyNotebookRepository
    {
        public static bool FindVocabularyNotebookById(int id,out Models.Entities.VocabularyNotebook? vocabularyNotebook,out string errorMessage,out Logging.LogCategory errorCategory)
        {
            errorMessage = "";
            errorCategory = Logging.LogCategory.None;

            try
            {
                vocabularyNotebook = Init.DatabaseInit.Current.VocabularyNotebooks.FindById(id);
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]查询单词时数据库错误",ex);

                vocabularyNotebook = null;
                errorMessage=  "查询单词时数据库错误";
                errorCategory = Logging.LogCategory.Database;
                return false;
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]查询单词时发生未知错误",ex);

                vocabularyNotebook = null;
                errorMessage="查询单词时发生未知错误";
                errorCategory = Logging.LogCategory.Unknown;
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// 仅供Services快捷调用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Models.Entities.VocabularyNotebook? FindVocabularyNotebookById(int id)
        {
            if(!FindVocabularyNotebookById(id,out var vocabularyNotebook, out _,out _))
            {
                return null;
            }
            return vocabularyNotebook;
        }

        public static bool FindVocabularyNotebooksByWordId(int wordID,out List<Models.Entities.VocabularyNotebook> noteBooks,out string errorMessage,out Logging.LogCategory errorCategory)
        {
            errorMessage = "";
            errorCategory = Logging.LogCategory.None;

            try
            {
                noteBooks = Init.DatabaseInit.Current.VocabularyNotebooks.Find(x => x.tanngoidlist.Contains(wordID)).ToList();
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]查询单词本时数据库错误",ex);

                noteBooks = new();
                errorMessage=  "查询单词本时数据库错误";
                errorCategory = Logging.LogCategory.Database;
                return false;
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]查询单词本时发生未知错误",ex);

                noteBooks = new();
                errorMessage="查询单词本时发生未知错误";
                errorCategory = Logging.LogCategory.Unknown;
                return false;
            }
            return true;
        }

        public static bool Insert(Models.Entities.VocabularyNotebook vocabularyNotebook,out string errorMessage,out Logging.LogCategory errorCategory)
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
        public static bool Update(Models.Entities.VocabularyNotebook vocabularyNotebook,out string errorMessage,out Logging.LogCategory errorCategory)
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

        public static bool Delete(int id,out string errorMessage,out Logging.LogCategory errorCategory)
        {
            errorMessage = "";
            errorCategory = Logging.LogCategory.None;

            try
            {
                if (!Init.DatabaseInit.Current.VocabularyNotebooks.Delete(id))
                {
                    errorMessage = "未能删除单词本数据(可能是单词不存在)";
                    errorCategory = Logging.LogCategory.Database;
                    return false;
                }
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]删除单词本时数据库错误",ex);

                errorMessage="删除单词本时数据库错误";
                errorCategory = Logging.LogCategory.Database;
                return false;
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]删除单词本时发生未知错误",ex);

                errorMessage="删除单词本时发生未知错误";
                errorCategory = Logging.LogCategory.Unknown;
                return false;
            }
            return true;
        }
    }
}