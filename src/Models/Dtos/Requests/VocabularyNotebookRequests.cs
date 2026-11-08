///词汇本相关请求

namespace NihongoHub.Models.Dtos.Requests
{
    public class SearchVocabularyNotebookRequestData
    {
        /// <summary>
        /// 根据tag查找单词本
        /// </summary>
        public required string tag {get;set;}
    }
    public class SearchVocabularyNotebookRequest : ApiRequest<SearchVocabularyNotebookRequestData>;

    public class CreateVocabularyNotebookRequestData
    {
        public required string name {get;set;}
        public string? note {get;set;}
        public required List<string> tags {get;set;}

        public Entities.VocabularyNotebook ToNotebookEntity(int userId)
        {
            return new Entities.VocabularyNotebook(){
                userid = userId,
                name = name,
                note = note,
                tanngoidlist = new(),
                tags = tags,
                createdat = DateTime.UtcNow,
                updatedat = DateTime.UtcNow
            };
        }
    }
    public class CreateVocabularyNotebookRequest : ApiRequest<CreateVocabularyNotebookRequestData>;

    public class UpdateVocabularyNotebookRequestData
    {
        public required int id {get;set;}
        public required string name {get;set;}
        public string? note {get;set;}
        public required List<string> tags {get;set;}
        
        public Entities.VocabularyNotebook ToNotebookEntity(int userId,List<int> tanngoIdList,DateTime createDat)
        {
            return new Entities.VocabularyNotebook()
            {
                id = id,
                userid = userId,
                name = name,
                note = note,
                tanngoidlist = tanngoIdList,
                tags = tags,
                createdat = createDat,
                updatedat = DateTime.UtcNow
            };
        }
    }
    public class UpdateVocabularyNotebookRequest : ApiRequest<UpdateVocabularyNotebookRequestData>;

    public class DeleteVocabularyNotebookRequestData
    {
        public required int id {get;set;}
    }
    public class DeleteVocabularyNotebookRequest : ApiRequest<DeleteVocabularyNotebookRequestData>;

    public class DeleteVocabularyNotebooksRequestData
    {
        public required List<DeleteVocabularyNotebookRequestData> noteBooks {get;set;}
    }
    public class DeleteVocabularyNotebooksRequest : ApiRequest<DeleteVocabularyNotebooksRequestData>;
}