///负责

namespace NihongoHub.Services
{
    public class missingId
    {
        public missingId(string _errorMessage,Logging.LogCategory? _errorCategory,int? _id = null)
        {
            id = _id;
            errorMessage = _errorMessage;
            errorCategory = _errorCategory;
        }

        public int? id {get;}
        public string errorMessage;
        public Logging.LogCategory? errorCategory;
    }

    public static class WordService
    {
        public static List<Models.Entities.Word> FindWordsById(List<int> wordId,ref List<int> missingIds)
        {
            missingIds.Clear();

            var wordList =  Data.WordRepository.FindWordsByIds(wordId,missingIds = new());
            
            return wordList;
        }

        public static Models.Entities.Word? FindWordsById(int wordId)
        {
            var word = Data.WordRepository.FindWordById(wordId);

            return word;
        }

        public static List<missingId> CreateWord(List<Models.Dtos.Requests.CreatWordRequestData> word,int userId)
        {
            List<missingId> missings = new();

            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;

            foreach(var _word in word)
            {
                if(!Data.WordRepository.Insert(_word.DataWordTo(userId),ref errorMessage,ref errorCategory))
                {
                    //创建时单词还不具有id,这里返回kana明确错误的单词
                    missings.Add(new(
                        $"{errorMessage} WordKana:'{_word.kana}'",errorCategory
                    ));
                }
            }

            return missings;
        }

        public static List<missingId> UpdateWord(List<Models.Dtos.Requests.UpdateWordRequestData> word,int userId)
        {
            List<missingId> missings = new();

            string errorMessage = "";
            Logging.LogCategory errorCategory = Logging.LogCategory.None;

            foreach (var _word in word)
            {
                if(!Data.WordRepository.Update(_word.DataWordTo(userId),ref errorMessage,ref errorCategory))
                {
                    missings.Add(new(errorMessage,errorCategory,_word.id));
                }  
            }

            return missings;
        }
    }
}