using LiteDB;

namespace NihongoHub.Models.Entities
{
    /// <summary>
    /// 声明：这里我对单词定义进行更改、每一个单词都存于一个专门的表中，拥有一个全局唯一的id
    /// 允许多个单词本包含同一个单词
    /// 单词本只包含对应单词的id，而不需要包含整个单词class
    /// 但单词必须有一个所属的单词本，应该将每一个单词都包含在"默认单词本"中
    /// </summary>
    
    public class Word
    {
        [BsonId]
        public int id {get;set;} //单词全局id
        public required int userid {get;set;}
        public string? kannji {get;set;} //汉字，可空
        public required string kana {get;set;} //假名
        public int? akusennto {get;set;} //音调(アクセント)
        public required string meaning {get;set;} //词义
        public string? note {get;set;} //备注
        public required DateTime createdat {get;set;} //UTC
        public required DateTime updatedat {get;set;} //UTC
    }
    
    /// <summary>
    /// 单词本。userid 与 User 的 id 一致,表示属于哪个用户
    /// tags 属于单词本(如 "N2"),单词本身没有 tag
    /// </summary>

    public class VocabularyNotebook
    {
        [BsonId]
        public int id {set;get;}
        public required int userid {set;get;}
        public required string name {get;set;}
        public string? note {get;set;} //表示用户的标记
        public required List<int> tanngoidlist {get;set;} //表示包含单词的id
        public required List<string> tags {get;set;} //供前端使用的tag
        public required DateTime createdat {get;set;} //UTC
        public required DateTime updatedat {get;set;} //UTC
    }
}