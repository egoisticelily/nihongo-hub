///单词相关响应

namespace NihongoHub.Models.Dtos.Responses
{
    ///返回多个单词
    /// 
    /// 这里定义的Word可供其他返回api使用
    
    public class VocabularyNotebookReferenceResponse
    {
        public required int id {get;set;}
        public required string name {get;set;}
        public required List<string> tags {get;set;}
    }
    
    public class WordResponse
    {
        public required int id {get;set;}
        public string? kannji {get;set;} //可空
        public required string kana {get;set;}
        public int? akusennto {get;set;}
        public required string meaning {get;set;}
        public string? note {get;set;}
        public List<VocabularyNotebookReferenceResponse>? vocabularynotebooks {get;set;} //表示包含该单词的单词本
        public required DateTime createdat {get;set;} //UTC
        public required DateTime updatedat {get;set;} //UTC
    }
    public class WordsResponseData
    {
        public required List<WordResponse> words {get;set;}
    }
    public class VocabularyNotebookWordsResponse : ApiResponse<WordsResponseData>;

    public class SearchWordsResponse : ApiResponse<WordsResponseData>;
}