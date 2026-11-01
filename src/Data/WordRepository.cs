using LiteDB;

namespace NihongoHub.Data
{
    public static class WordRepository
    {
        public static bool FindWordById(int id,out Models.Entities.Word? word,out string errorMessage,out Logging.LogCategory errorCategory)
        {
            errorMessage = "";
            errorCategory = Logging.LogCategory.None;

            try
            {
                word = Init.DatabaseInit.Current.Words.FindById(id);
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]查询单词时数据库错误",ex);

                word = null;
                errorMessage=  "查询单词时数据库错误";
                errorCategory = Logging.LogCategory.Database;
                return false;
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]查询单词时发生未知错误",ex);

                word = null;
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
        public static Models.Entities.Word? FindWordById(int id)
        {
            if (!FindWordById(id, out var word, out _, out _))
            {
                return null;
            }
            return word;
        }

        public static bool Insert(Models.Entities.Word word,out string errorMessage,out Logging.LogCategory errorCategory)
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
        public static bool Update(Models.Entities.Word word,out string errorMessage,out Logging.LogCategory errorCategory)
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

        public static bool Delete(int id,out string errorMessage,out Logging.LogCategory errorCategory)
        {
            errorMessage = "";
            errorCategory = Logging.LogCategory.None;

            try
            {
                if (!Init.DatabaseInit.Current.Words.Delete(id))
                {
                    errorMessage = "未能删除单词数据(可能是单词不存在)";
                    errorCategory = Logging.LogCategory.Database;
                    return false;
                }
            }
            catch(LiteException ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]修改单词时数据库错误",ex);

                errorMessage="删除单词时数据库错误";
                errorCategory = Logging.LogCategory.Database;
                return false;
            }
            catch(Exception ex)
            {
                Logging.LogReporter.Report(Logging.LogCategory.Database,Logging.LogLevel.Error,"[ERROR]修改单词时发生未知错误",ex);

                errorMessage="删除单词时发生未知错误";
                errorCategory = Logging.LogCategory.Unknown;
                return false;
            }
            return true;
        }
    }
}