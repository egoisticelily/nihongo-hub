///词汇本相关响应

namespace NihongoHub.Models.Dtos.Responses
{
    public class VocabularyNotebookSummaryResponseData
    {
        public required int id {get;set;}
        public required string name {get;set;}
        public string? note {get;set;}
        public required List<string> tags {get;set;}
        public required int wordcount {get;set;}
        public required DateTime createdat {get;set;}
        public required DateTime updatedat {get;set;}
    }

    public class VocabularyNotebookSummaryResponse : ApiResponse<VocabularyNotebookSummaryResponseData>;
}