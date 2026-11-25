using NihongoHub.Models.Entities;
using System.Diagnostics.CodeAnalysis;

namespace NihongoHub.Services
{
    public class VocabularyNotebookOperationFailure
    {
        public VocabularyNotebookOperationFailure(string _errorMessage,Logging.LogCategory? _errorCategory,int? _id = null)
        {
            id = _id;
            errorMessage = _errorMessage;
            errorCategory = _errorCategory;
        }

        public int? id {get;}
        public string errorMessage {get;set;}
        public Logging.LogCategory? errorCategory {get;set;}
    }

    public static class VocabularyNotebookService
    {
        public static List<Models.Entities.VocabularyNotebook> FindVocabularyNotebooksByIds(List<int> ids,int userID,out List<VocabularyNotebookOperationFailure> failures)
        {
            List<Models.Entities.VocabularyNotebook> notebooks = new();
            failures = new();
            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;
            Models.Entities.VocabularyNotebook? notebook = null;

            foreach(var id in ids)
            {
                if(!Data.VocabularyNotebookRepository.FindVocabularyNotebookById(id,out notebook,out errorMessage,out errorCategory))
                {
                    failures.Add(new(errorMessage,errorCategory,id));
                    continue;
                }
                if(notebook == null)
                {
                    failures.Add(new($"单词本不存在",errorCategory,id));
                    continue;
                }
                if(notebook.userid != userID)
                {
                    failures.Add(new($"该单词本不属于该用户",errorCategory,id));
                    continue;
                }
                notebooks.Add(notebook);
            }
            return notebooks;
        }

        public static Models.Entities.VocabularyNotebook? FindVocabularyNotebookById(int id,int userID,[NotNullWhen(true)] out VocabularyNotebookOperationFailure? failure)
        {
            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;
            Models.Entities.VocabularyNotebook? notebook = null;
            failure = null;

            if(!Data.VocabularyNotebookRepository.FindVocabularyNotebookById(id,out notebook,out errorMessage,out errorCategory))
            {
                failure = new(errorMessage,errorCategory,id);
                return null;
            }
            if(notebook == null)
            {
                failure = new($"该单词本不存在",Logging.LogCategory.None,id);
                return null;
            }
            if(notebook.userid != userID)
            {
                failure = new($"该单词本不属于该用户",Logging.LogCategory.None,id);
                return null;
            }
            return notebook;
        }

        public static List<Models.Entities.VocabularyNotebook> FindVocabularyNotebooksByUserid(int userid,out VocabularyNotebookOperationFailure? failures)
        {
            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;
            List<Models.Entities.VocabularyNotebook> notebookList = new();
            failures = null;

            if(!Data.VocabularyNotebookRepository.FindVocabularyNotebooksByUserId(userid,out notebookList,out errorMessage,out errorCategory))
            {
                failures = new(errorMessage,errorCategory,userid);
                return new();
            }
            return notebookList;
        }

        public static List<Models.Entities.VocabularyNotebook> FindVocabularyNotebooksByTag(string tag,int userid,out VocabularyNotebookOperationFailure? failures)
        {
            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;
            List<Models.Entities.VocabularyNotebook> notebookList = new();
            failures = null;

            if(!Data.VocabularyNotebookRepository.FindVocabularyNotebooksByTag(tag,userid,out notebookList,out errorMessage,out errorCategory))
            {
                failures = new(errorMessage,errorCategory,userid);
                return new();
            }
            return notebookList;
        }

        public static VocabularyNotebookOperationFailure? CreateVocabularyNotebook(Models.Dtos.Requests.CreateVocabularyNotebookRequestData noteBook,int userID)
        {
            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;

            if (!Data.VocabularyNotebookRepository.Insert(noteBook.ToNotebookEntity(userID),out errorMessage,out errorCategory))
            {
                return new(errorMessage,errorCategory);
            }
            return null;
        }

        public static VocabularyNotebookOperationFailure? UpdateVocabularyNotebook(Models.Dtos.Requests.UpdateVocabularyNotebookRequestData noteBook,int userID)
        {
            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;

            if(!Data.VocabularyNotebookRepository.FindVocabularyNotebookById(noteBook.id,out var _noteBook,out errorMessage,out errorCategory))
            {
                return new(errorMessage,errorCategory,noteBook.id);
            }

            if(_noteBook == null)
            {
                return new($"该单词本不存在",Logging.LogCategory.None,noteBook.id);
            }

            if(_noteBook.userid != userID)
            {
                return new($"该单词本不属于该用户",Logging.LogCategory.None,noteBook.id);
            }

            if (!Data.VocabularyNotebookRepository.Update(noteBook.ToNotebookEntity(userID, _noteBook.tanngoidlist, _noteBook.createdat),out errorMessage,out errorCategory))
            {
                return new(errorMessage,errorCategory);
            }
            return null;
        }

        public static VocabularyNotebookOperationFailure? DeleteVocabularyNotebook(Models.Dtos.Requests.DeleteVocabularyNotebookRequestData noteBook,int userID)
        {
            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;

            if(!Data.VocabularyNotebookRepository.FindVocabularyNotebookById(noteBook.id,out var _noteBook,out errorMessage,out errorCategory))
            {
                return new(errorMessage,errorCategory,noteBook.id);
            }

            if(_noteBook == null)
            {
                return new($"该单词本不存在",Logging.LogCategory.None,noteBook.id);
            }

            if(_noteBook.userid != userID)
            {
                return new($"该单词本不属于该用户",Logging.LogCategory.None,noteBook.id);
            }

            if(!Data.VocabularyNotebookRepository.Delete(noteBook.id,out errorMessage,out errorCategory))
            {
                return new(errorMessage,errorCategory,noteBook.id);
            }

            return null;
        }

        public static List<VocabularyNotebookOperationFailure> DeleteNotebooks(Models.Dtos.Requests.DeleteVocabularyNotebooksRequestData noteBooks,int userID)
        {
            List<VocabularyNotebookOperationFailure> failures = new();
            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;

            foreach (var noteBook in noteBooks.notebooks)
            {
                if(!Data.VocabularyNotebookRepository.FindVocabularyNotebookById(noteBook.id,out var _noteBook,out errorMessage,out errorCategory))
                {
                    failures.Add(new(errorMessage,errorCategory,noteBook.id));
                    continue;
                }

                if(_noteBook == null)
                {
                    failures.Add(new($"该单词本不存在",Logging.LogCategory.None,noteBook.id));
                    continue;
                }

                if(_noteBook.userid != userID)
                {
                    failures.Add(new($"该单词本不属于该用户",Logging.LogCategory.None,noteBook.id));
                    continue;
                }

                if(!Data.VocabularyNotebookRepository.Delete(noteBook.id,out errorMessage,out errorCategory))
                {
                    failures.Add(new(errorMessage,errorCategory,noteBook.id));
                }
            }
            
            return failures;
        }

        public static List<VocabularyNotebookOperationFailure> RemoveWordFromAllNotebooks(int wordID)
        {
            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;
            List<VocabularyNotebookOperationFailure> failures = new();

            if(!Data.VocabularyNotebookRepository.FindVocabularyNotebooksByWordId(wordID,out var noteBooks,out errorMessage,out errorCategory))
            {
                failures.Add(new($"[严重错误] - 获取对应单词本时发生严重错误 \n - {errorMessage}",errorCategory,wordID));
                return failures;
            }

            foreach(var noteBook in noteBooks)
            {
                noteBook.tanngoidlist.RemoveAll(x => x == wordID);
                noteBook.updatedat = DateTime.UtcNow;

                if(!Data.VocabularyNotebookRepository.Update(noteBook,out errorMessage,out errorCategory))
                {
                    failures.Add(new(errorMessage,errorCategory,noteBook.id));
                    continue;
                }
            }
            
            ///先写入log,后续再考虑其他形式
            int errorNumer = 0;

            foreach(var log in failures)
            {
                ++errorNumer;
                Logging.LogCategory category = log.errorCategory ?? Logging.LogCategory.None;
                Logging.LogReporter.Report(category,Logging.LogLevel.Error,$"[wordID - {wordID}] [No.{errorNumer}] - {log.errorMessage}");
            }
            
            return failures;
        }

        public static VocabularyNotebookOperationFailure? AddWordInNotebook(int wordID,int notebookID,int userID)
        {
            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;
            
            var notebookList = FindVocabularyNotebooksByUserid(userID,out var failure);

            if(failure != null)
                return failure;

            var notebook = notebookList.Find(x => x.id == notebookID);

            if(notebook == null)
            {
                return new($"单词本不存在",Logging.LogCategory.None,notebookID);
            }

            var word = Services.WordService.FindWordById(wordID,userID,out var _failure);

            if(_failure != null)
                return new(_failure.errorMessage,_failure.errorCategory,_failure.id);
            if(word == null)
                return new("单词不存在",Logging.LogCategory.None);

            if(notebook.tanngoidlist.Contains(word.id))
            {
                return new($"单词已存在",Logging.LogCategory.None,wordID);
            }
            
            notebook.tanngoidlist.Add(word.id);
            notebook.updatedat = DateTime.UtcNow;
            if(!Data.VocabularyNotebookRepository.Update(notebook,out errorMessage,out errorCategory))
            {
                return new($"更新单词本时发生错误 \n - {errorMessage}",errorCategory,wordID);
            }
            
            return null;
        }

        public static VocabularyNotebookOperationFailure? RemoveWordFromNotebook(int wordID,int notebookID,int userID)
        {
            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;
            
            var notebookList = FindVocabularyNotebooksByUserid(userID,out var failure);

            if(failure != null)
                return failure;

            var notebook = notebookList.Find(x => x.id == notebookID);

            if(notebook == null)
            {
                return new($"单词本不存在",Logging.LogCategory.None,notebookID);
            }

            var word = Services.WordService.FindWordById(wordID,userID,out var _failure);

            if(_failure != null)
                return new(_failure.errorMessage,_failure.errorCategory,_failure.id);
            if(word == null)
                return new("单词不存在",Logging.LogCategory.None);
            
            notebook.tanngoidlist.RemoveAll(x => x == word.id);
            notebook.updatedat = DateTime.UtcNow;

            if(!Data.VocabularyNotebookRepository.Update(notebook,out errorMessage,out errorCategory))
            {
                return new($"更新单词本时发生错误 \n - {errorMessage}",errorCategory,wordID);
            }
            
            return null;
        }
    }
}