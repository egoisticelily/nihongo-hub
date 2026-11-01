///单词搜索、单词操作等请求

namespace NihongoHub.Models.Dtos.Requests
{
    public class SearchWordsRequestData
    {
        /// <summary>
        /// 这里的query既可以是kannji、kana，也可以是单词的意思等，需要后端自行判断
        /// </summary>
        public required string query {get;set;}
    }
    public class SearchWordsRequest : ApiRequest<SearchWordsRequestData>;


    public class CreatWordRequestData
    {
        public string? kannji {get;set;}
        public required string kana {get;set;}
        public int? akusennto {get;set;}
        public required string meaning {get;set;}
        public string? note {get;set;}

        /// <summary>
        /// 将CreatWordRequestData转换为Models.Entities.Word
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Entities.Word DataWordTo(int userId)
        {
            return new Models.Entities.Word()
            {
                userid = userId,
                kannji = kannji,
                kana = kana,
                akusennto = akusennto,
                meaning = meaning,
                note = note,
                createdat = DateTime.UtcNow,
                updatedat = DateTime.UtcNow
            };
        }
    }
    public class CreatWordRequest : ApiRequest<CreatWordRequestData>;

    public class UpdateWordRequestData
    {
        public required int id {get;set;}
        public string? kannji {get;set;}
        public required string kana {get;set;}
        public int? akusennto {get;set;}
        public required string meaning {get;set;}
        public string? note {get;set;}

        public Entities.Word DataWordTo(int userId)
        {
            return new Entities.Word()
            {
                userid = userId,
                kannji = kannji,
                kana = kana,
                akusennto = akusennto,
                meaning = meaning,
                note = note,
                updatedat = DateTime.UtcNow
            }
        }
    }
    public class UpdateWordRequest : ApiRequest<UpdateWordRequestData>;
}