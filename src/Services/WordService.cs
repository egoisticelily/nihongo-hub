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
        public static List<Models.Entities.Word> FindWordsByIds(List<int> wordIds,out List<int> missingIds)
        {
            var wordList =  Data.WordRepository.FindWordsByIds(wordIds,missingIds = new());
            
            return wordList;
        }

        public static Models.Entities.Word? FindWordById(int wordId)
        {
            var word = Data.WordRepository.FindWordById(wordId);

            return word;
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
                var _word = Data.WordRepository.FindWordById(word.id);
                if(_word == null)
                {
                    failures.Add(new($"该单词不存在",Logging.LogCategory.Unknown,word.id)); //这不算数据库错误，而是用户自己传入错误
                    continue;
                }

                if(_word.userid != userId)
                {
                    failures.Add(new($"该单词不属于该用户",Logging.LogCategory.Unknown,word.id));
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

            var _word = Data.WordRepository.FindWordById(word.id);

            if(_word == null)
            {
                return new($"该单词不存在",Logging.LogCategory.Unknown,word.id);
            }

            if(_word.userid != userId)
            {
                return new($"该单词不属于该用户",Logging.LogCategory.Unknown,word.id);
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
                var _word = Data.WordRepository.FindWordById(word.id);

                if(_word == null)
                {
                    failures.Add(new($"该单词不存在",Logging.LogCategory.Unknown,word.id));
                    continue;
                }

                if(_word.userid != userID)
                {
                    failures.Add(new($"该单词不属于该用户",Logging.LogCategory.Unknown,word.id));
                    continue;
                }

                if (!Data.WordRepository.Delete(word.id,out errorMessage,out errorCategory))
                {
                    failures.Add(new(errorMessage,errorCategory,word.id));
                }
            }
            return failures;
        }

        public static WordOperationFailure? DeleteWord(Models.Dtos.Requests.DeleteWordRequestData word,int userID)
        {
            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;

            var _word = Data.WordRepository.FindWordById(word.id);

            if(_word == null)
            {
                return new($"该单词不存在",Logging.LogCategory.Unknown,word.id);
            }

            if(_word.userid != userID)
            {
                return new($"该单词不属于该用户",Logging.LogCategory.Unknown,word.id);
            }

            if(!Data.WordRepository.Delete(word.id,out errorMessage,out errorCategory))
            {
                return new(errorMessage,errorCategory,word.id);
            }

            return null;
        }
    }
}