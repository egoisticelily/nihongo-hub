///词汇本相关响应

namespace NihongoHub.Models.Dtos.Responses
{
    public class VocabularyNotebookSummaryResponseData
    {
        public required int id {get;set;}
        public required string name {get;set;}
        public string? note {get;set;}
        public required List<string> tags {get;set;}
        public required DateTime createdat {get;set;}
        public required DateTime updatedat {get;set;}
    }

    public class VocabularyNotebookSummaryResponse : ApiResponse<VocabularyNotebookSummaryResponseData>;

    public class VocabularyNotebookListSummaryResponseData
    {
        public required List<VocabularyNotebookSummaryResponseData> VocabularyNotebookList {get;set;}
    }
    public class VocabularyNotebookListSummaryResponse : ApiResponse<VocabularyNotebookListSummaryResponseData>;

    public static class Auxiliary{
        public static List<VocabularyNotebookSummaryResponseData> DatabaseVocabularyNotebookToResponseData(List<Models.Entities.VocabularyNotebook> databaseNotebook)
        {
            List<VocabularyNotebookSummaryResponseData> notebookResponseList = new();
            foreach(var notebook in databaseNotebook)
            {
                notebookResponseList.Add(new()
                {
                    id = notebook.id,
                    name = notebook.name,
                    note = notebook.note,
                    tags = notebook.tags,
                    createdat = notebook.createdat,
                    updatedat = notebook.updatedat
                });
            }
            return notebookResponseList;
        }
    }
    
}