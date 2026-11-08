using System.Net;

namespace NihongoHub.Services
{
    public class WordOperationFailure
    {
        public WordOperationFailure(string _errorMessage,Logging.LogCategory? _errorCategory,int? _id = null)
        {
            id = _id;
            errorMessage = _errorMessage;
            errorCategory = _errorCategory;
        }

        public int? id {get;}
        public string errorMessage {get;set;}
        public Logging.LogCategory? errorCategory {get;set;}
    }

    public static class WordService
    {
        public static List<Models.Entities.Word> FindWordsByIds(List<int> wordIds,int userId,out List<WordOperationFailure> failures)
        {
            List<Models.Entities.Word> words = new();
            failures = new();
            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;
            Models.Entities.Word? word = null;

            foreach(var id in wordIds)
            {
                if(!Data.WordRepository.FindWordById(id,out word,out errorMessage,out errorCategory))
                {
                    failures.Add(new(errorMessage,errorCategory,id));
                    continue;
                }
                if(word == null)
                {
                    failures.Add(new($"单词不存在",errorCategory,id));
                    continue;
                }

                if(word.userid != userId)
                {
                    failures.Add(new($"该单词不属于该用户",Logging.LogCategory.None,id));
                    continue;
                }

                words.Add(word);
            }
            return words;
        }

        public static Models.Entities.Word? FindWordById(int wordId,int userId,out WordOperationFailure? failure)
        {
            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;
            Models.Entities.Word? word = null;
            failure = null;

            if(!Data.WordRepository.FindWordById(wordId,out word,out errorMessage,out errorCategory))
            {
                failure = new(errorMessage,errorCategory,wordId);
                return null;
            }
            if(word == null)
            {
                failure = new($"单词不存在",Logging.LogCategory.None,wordId);
                return null;
            }
            if(word.userid != userId)
            {
                failure = new($"该单词不属于该用户",Logging.LogCategory.None,wordId);
                return null;
            }

            return word;
        }

        public static List<Models.Entities.Word> FindWordsByQuery(string query,int userID, out WordOperationFailure? failure)
        {
            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;
            List<Models.Entities.Word> words = new();
            failure = null;

            if(string.IsNullOrWhiteSpace(query)){
                failure = new($"不允许查询值为空",Logging.LogCategory.Unknown);
                return new();
            }
            //去掉空白
            query = query.Trim();

            if (!Data.WordRepository.FindWordsByQuery(query,userID,out words,out errorMessage,out errorCategory))
            {
                failure = new(errorMessage,errorCategory);
                return new();
            }

            return SortWordsByRelevance(words, query);
        }

        public static List<WordOperationFailure> CreateWords(Models.Dtos.Requests.CreateWordsRequestData words,int userId)
        {
            List<WordOperationFailure> failures = new();

            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;

            foreach(var word in words.words)
            {
                if(!Data.WordRepository.Insert(word.ToWordEntity(userId),out errorMessage,out errorCategory))
                {
                    //创建时单词还不具有id,这里返回kana明确错误的单词
                    failures.Add(new(
                        $"{errorMessage} WordKana:'{word.kana}'",errorCategory
                    ));
                }
            }

            return failures;
        }

        public static WordOperationFailure? CreateWord(Models.Dtos.Requests.CreateWordRequestData word,int userId)
        {
            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;

            if(!Data.WordRepository.Insert(word.ToWordEntity(userId),out errorMessage,out errorCategory))
            {
                return new(errorMessage,errorCategory);
            }
            return null;
        }

        public static List<WordOperationFailure> UpdateWords(Models.Dtos.Requests.UpdateWordsRequestData words,int userId)
        {
            List<WordOperationFailure> failures = new();

            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;

            foreach (var word in words.words)
            {
                if(!Data.WordRepository.FindWordById(word.id,out var _word,out errorMessage,out errorCategory))
                {
                    failures.Add(new(errorMessage,errorCategory,word.id));
                    continue;
                }
                if(_word == null)
                {
                    failures.Add(new($"该单词不存在",Logging.LogCategory.None,word.id)); //这不算数据库错误，而是用户自己传入错误
                    continue;
                }

                if(_word.userid != userId)
                {
                    failures.Add(new($"该单词不属于该用户",Logging.LogCategory.None,word.id));
                    continue;
                }

                if(!Data.WordRepository.Update(word.ToWordEntity(userId,_word.createdat),out errorMessage,out errorCategory))
                {
                    failures.Add(new(errorMessage,errorCategory,word.id));
                }
            }

            return failures;
        }

        public static WordOperationFailure? UpdateWord(Models.Dtos.Requests.UpdateWordRequestData word,int userId)
        {
            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;

            if(!Data.WordRepository.FindWordById(word.id,out var _word,out errorMessage,out errorCategory))
            {
                return new(errorMessage,errorCategory,word.id);
            }

            if(_word == null)
            {
                return new($"该单词不存在",Logging.LogCategory.None,word.id);
            }

            if(_word.userid != userId)
            {
                return new($"该单词不属于该用户",Logging.LogCategory.None,word.id);
            }

            if(!Data.WordRepository.Update(word.ToWordEntity(userId,_word.createdat),out errorMessage,out errorCategory))
            {
                return new(errorMessage,errorCategory,word.id);
            }
            return null;
        }

        public static List<WordOperationFailure> DeleteWords(Models.Dtos.Requests.DeleteWordsRequestData words,int userID)
        {
            List<WordOperationFailure> failures = new();
            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;
            
            foreach(var word in words.words)
            {
                if(!Data.WordRepository.FindWordById(word.id,out var _word,out errorMessage,out errorCategory))
                {
                    failures.Add(new(errorMessage,errorCategory,word.id));
                    continue;
                }

                if(_word == null)
                {
                    failures.Add(new($"该单词不存在",Logging.LogCategory.None,word.id));
                    continue;
                }

                if(_word.userid != userID)
                {
                    failures.Add(new($"该单词不属于该用户",Logging.LogCategory.None,word.id));
                    continue;
                }

                var DeleteNotebooksInWordFailure = VocabularyNotebookService.RemoveWordFromNotebooks(word.id);

                if(DeleteNotebooksInWordFailure.Count() != 0)
                {
                    failures.Add(new($"更新包含单词的单词本时出现问题 - 详细请查看后台日志，如果您是管理员，可以尝试自行修复，否则，建议您联系管理员",DeleteNotebooksInWordFailure[0].errorCategory,word.id));
                    continue;
                }

                if (!Data.WordRepository.Delete(word.id,out errorMessage,out errorCategory))
                {
                    failures.Add(new(
                        $"{errorMessage} - 单词本关联已经被清理，但单词本身删除失败。 \n 该单词本身可能已不属于任何单词本，请尝试重新删除 \n 如果再次操作仍然失误，如果您是管理员，您可以尝试自行修复。否则，建议您联系管理员。",
                        errorCategory,
                        word.id
                    ));
                }
            }
            return failures;
        }

        public static WordOperationFailure? DeleteWord(Models.Dtos.Requests.DeleteWordRequestData word,int userID)
        {
            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;

            if(!Data.WordRepository.FindWordById(word.id,out var _word,out errorMessage,out errorCategory))
            {
                return new(errorMessage,errorCategory,word.id);
            }

            if(_word == null)
            {
                return new($"该单词不存在",Logging.LogCategory.None,word.id);
            }

            if(_word.userid != userID)
            {
                return new($"该单词不属于该用户",Logging.LogCategory.None,word.id);
            }

            var DeleteNotebooksInWordFailure = VocabularyNotebookService.RemoveWordFromNotebooks(word.id);

            if(DeleteNotebooksInWordFailure.Count() != 0)
            {
                return new($"更新包含单词的单词本时出现问题 - 详细请查看后台日志，如果您是管理员，可以尝试自行修复，否则，建议您联系管理员",DeleteNotebooksInWordFailure[0].errorCategory,word.id);
            }

            if(!Data.WordRepository.Delete(word.id,out errorMessage,out errorCategory))
            {
                return new(
                    $"{errorMessage} - 单词本关联已经被清理，但单词本身删除失败。 \n 该单词本身可能已不属于任何单词本，请尝试重新删除 \n 如果再次操作仍然失误，如果您是管理员，您可以尝试自行修复。否则，建议您联系管理员。",
                    errorCategory,
                    word.id
                );
            }

            return null;
        }
        
        private class WordMatchResult
        {
            public WordMatchResult(bool exactMatch,double matchRatio,int matchPosition,Models.Entities.Word word)
            {
                IsExactMatch = exactMatch;
                MatchRatio = matchRatio;
                MatchPosition = matchPosition;
                Word = word;
            }
            public int WordId {get;}
            public bool IsExactMatch {get;}
            public double MatchRatio {get;}
            public int MatchPosition {get;}
            public Models.Entities.Word Word {get;}
        }
        private static List<Models.Entities.Word> SortWordsByRelevance(List<Models.Entities.Word> words,string query)
        {   
            List<WordMatchResult> wordMatches = new();

            foreach(var word in words)
            {

                List<(double matchRatio, int matchPosition)> fieldMatches = new();
                bool isExactMatch = (word.kannji != null && word.kannji == query) ||
                    word.kana == query ||
                    word.meaning == query;

                bool isKanaMatch = word.kana.Contains(query,StringComparison.Ordinal);
                bool isMeaningMatch = word.meaning.Contains(query,StringComparison.Ordinal);

                if (word.kannji != null && word.kannji.Contains(query,StringComparison.Ordinal))
                {
                    fieldMatches.Add(
                        (
                            (double)query.Length / word.kannji.Length,
                            word.kannji.IndexOf(query,StringComparison.Ordinal)
                        )
                    );
                }
                
                if(isKanaMatch)
                {
                    fieldMatches.Add(
                        (
                            (double)query.Length / word.kana.Length,
                            word.kana.IndexOf(query,StringComparison.Ordinal)
                        )
                    );
                }
                
                if(isMeaningMatch)
                {
                    fieldMatches.Add(
                        (
                            (double)query.Length / word.meaning.Length,
                            int.MaxValue
                        )
                    );
                }

                var bestMatch = fieldMatches
                    .OrderByDescending(x => x.matchRatio)
                    .ThenBy(x => x.matchPosition)
                    .First();

                wordMatches.Add(new(
                    isExactMatch,
                    bestMatch.matchRatio,
                    bestMatch.matchPosition,
                    word
                ));
            }

            //排序
            var sortedWords = wordMatches
            .OrderByDescending(x => x.IsExactMatch)
            .ThenByDescending(x => x.MatchRatio)
            .ThenBy(x => x.MatchPosition)
            .ThenBy(x => x.WordId)
            .ToList();

            return sortedWords.Select(x => x.Word).ToList();
        }
    }
}