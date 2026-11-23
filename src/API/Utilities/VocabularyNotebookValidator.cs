namespace NihongoHub.API.Utilities
{
    public static class VocabularyNotebookValidator
    {
        public static bool GetDefaultNotebook(int userID,out Models.Entities.VocabularyNotebook notebookList,out string? errorMessage,out string? errorCategory)
        {
            errorCategory = "";
            errorMessage = "";
            notebookList = new()
            {
                id = -1,
                userid = userID,
                name = "default",
                note = "",
                tanngoidlist = new(),
                tags = new(),
                createdat = DateTime.UtcNow,
                updatedat = DateTime.UtcNow
            };

            var wordsList = Services.WordService.FindWordsByUserid(userID,out var failure);

            if(failure != null)
            {
                errorCategory = failure.errorCategory.ToString();
                errorMessage = failure.errorMessage;
                return false;
            }
            
            foreach(var word in wordsList)
            {
                notebookList.tanngoidlist.Add(word.id);
            }

            return true;
        }
    }
}