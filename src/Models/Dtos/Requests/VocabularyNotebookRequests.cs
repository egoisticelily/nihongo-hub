///词汇本相关请求

namespace NihongoHub.Models.Dtos.Requests
{
    public class VocabularyNotebookWordsRequestData
    {
        public required int vocabularynotebookid {get;set;}
    }
    public class VocabularyNotebookWordsRequest : ApiRequest<VocabularyNotebookWordsRequestData>;
}