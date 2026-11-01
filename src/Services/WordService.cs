///负责

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

        public static Models.Entities.Word? FindWordById(int wordIds)
        {
            var words = Data.WordRepository.FindWordById(wordIds);

            return words;
        }

        public static List<WordOperationFailure> CreateWord(List<Models.Dtos.Requests.CreateWordRequestData> words,int userId)
        {
            List<WordOperationFailure> failures = new();

            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;

            foreach(var word in words)
            {
                if(!Data.WordRepository.Insert(word.ToWordEntity(userId),ref errorMessage,ref errorCategory))
                {
                    //创建时单词还不具有id,这里返回kana明确错误的单词
                    failures.Add(new(
                        $"{errorMessage} WordKana:'{word.kana}'",errorCategory
                    ));
                }
            }

            return failures;
        }

        public static List<WordOperationFailure> UpdateWord(List<Models.Dtos.Requests.UpdateWordRequestData> words,int userId)
        {
            List<WordOperationFailure> failures = new();

            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;

            foreach (var word in words)
            {
                if(!Data.WordRepository.Update(word.ToWordEntity(userId),ref errorMessage,ref errorCategory))
                {
                    failures.Add(new(errorMessage,errorCategory,word.id));
                }  
            }

            return failures;
        }
    }
}