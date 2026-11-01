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


    public class CreateWordRequestData
    {
        public string? kannji {get;set;}
        public required string kana {get;set;}
        public int? akusennto {get;set;}
        public required string meaning {get;set;}
        public string? note {get;set;}

        /// <summary>
        /// 将CreateWordRequestData转换为Models.Entities.Word
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Entities.Word ToWordEntity(int userId)
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
    public class CreateWordRequest : ApiRequest<CreateWordRequestData>;

    public class CreateWordsRequestData
    {
        public required List<CreateWordRequestData> words {get;set;}
    }

    public class CreateWordsRequest : ApiRequest<CreateWordsRequestData>;

    public class UpdateWordRequestData
    {
        public required int id {get;set;}
        public string? kannji {get;set;}
        public required string kana {get;set;}
        public int? akusennto {get;set;}
        public required string meaning {get;set;}
        public string? note {get;set;}

        public Entities.Word ToWordEntity(int userId,DateTime createDat)
        {
            return new Entities.Word()
            {
                id = id,
                userid = userId,
                kannji = kannji,
                kana = kana,
                akusennto = akusennto,
                meaning = meaning,
                note = note,
                createdat = createDat,
                updatedat = DateTime.UtcNow
            };
        }
    }
    public class UpdateWordRequest : ApiRequest<UpdateWordRequestData>;

    public class UpdateWordsRequestData
    {
        public required List<UpdateWordRequestData> words {get;set;}
    }
    
    public class UpdateWordsRequest : ApiRequest<UpdateWordsRequestData>;

    public class DeleteWordRequestData
    {
        public required int id {get;set;}
    }


    public class DeleteWordRequest : ApiRequest<DeleteWordRequestData>;

    public class DeleteWordsRequestData
    {
        public required List<DeleteWordRequestData> words {get;set;}
    }
    public class DeleteWordsRequest : ApiRequest<DeleteWordsRequestData>;
}