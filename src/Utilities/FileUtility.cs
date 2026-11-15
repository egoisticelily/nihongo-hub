using System.Text;

namespace NihongoHub.Utilities
{
    public static class FileUtility
    {
        /// <summary>
        /// 仅接受相对路径(相对可执行文件目录)
        /// </summary>
        /// <param name="content"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="errorMessage"></param>
        /// <param name="errorCategory"></param>
        /// <returns></returns>
        public static bool WriteFile(string content,string path,string fileName,out string errorMessage,out Logging.LogCategory errorCategory)
        {
            errorMessage = "";
            errorCategory = Logging.LogCategory.None;

            if (String.IsNullOrWhiteSpace(path))
            {
                errorCategory = Logging.LogCategory.File;
                errorMessage = "[ERROR] Path为空";
                return false;
            }

            try
            {
                if (!Directory.Exists(Path.Join(AppContext.BaseDirectory,path)))
                {
                    errorCategory = Logging.LogCategory.File;
                    errorMessage = "Path不存在或不合法";
                    return false;
                }
                string filePath = Path.Join(AppContext.BaseDirectory,path,fileName);

                if (filePath.Split('/','\\').Any(x => x == ".."))
                {
                    errorCategory = Logging.LogCategory.File;
                    errorMessage = "Path不合法";
                    return false;
                }

                File.WriteAllText(filePath,content,new UTF8Encoding(false));
            }
            catch (IOException ex)
            {
                errorCategory = Logging.LogCategory.File;
                errorMessage = $"检查/写入文件时 I/O问题 -\n-Error: {ex}";
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                errorCategory = Logging.LogCategory.File;
                errorMessage = $"检查/写入文件时 权限问题 -\n-Error: {ex}";
                return false;
            }
            catch(Exception ex)
            {
                errorCategory = Logging.LogCategory.Unknown;
                errorMessage = $"检查/写入文件时 未知错误 可能是Path不合法或权限问题 -\n-Error: {ex}";
                return false;
            }
            return true;
        }
    }
}