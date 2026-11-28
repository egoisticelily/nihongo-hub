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
    
    public class DeleteVocabularyNotebookSummaryResponseData
    {
        public int? id {get;set;}
        public required string errormessage {get;set;}
        public string? errorcategory {get;set;}
    }

    public class DeleteVocabularyNotebookByListSummaryResponseData
    {
        public required List<DeleteVocabularyNotebookSummaryResponseData> responseDataList {get;set;}
    }

    public class DeleteVocabularyNotebookByListSummaryResponse : ApiResponse<DeleteVocabularyNotebookByListSummaryResponseData>;

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

        public static List<DeleteVocabularyNotebookSummaryResponseData> VocabularyNotebookOperationFailureListToResponseData(List<Services.VocabularyNotebookOperationFailure> failuresList)
        {
            List<DeleteVocabularyNotebookSummaryResponseData> responseDataList = new();
            foreach(var failures in failuresList)
            {
                responseDataList.Add(new()
                {
                    id = failures.id,
                    errormessage = failures.errorMessage,
                    errorcategory = failures.errorCategory.ToString(),
                });
            }
            return responseDataList;
        }

        public static List<Message> WordOperationFailureListToResponseData(List<Services.WordOperationFailure> failuresList)
        {
            List<Message> responseDataList = new();
            foreach(var failures in failuresList)
            {
                responseDataList.Add(new()
                {
                    id = failures.id,
                    errormessage = failures.errorMessage,
                    errorcategory = failures.errorCategory.ToString(),
                });
            }
            return responseDataList;
        }
    }
    
}