namespace NihongoHub.API.Utilities
{
    public static class VocabularyNotebookValidator
    {
        public static Models.Entities.VocabularyNotebook GetDefaultNotebook(int userID)
        {
            Models.Entities.VocabularyNotebook notebookList = new()
            {
                id = -1,
                userid = userID,
                name = "default",
                note = "",
                tanngoidlist = new(),
                tags = new(),
                createdat = new(2000,01,01,0,0,0,DateTimeKind.Utc),
                updatedat = new(2000,01,01,0,0,0,DateTimeKind.Utc)
            };

            return notebookList;
        }
    }
}