namespace NihongoHub.API
{
    public static class WordValidator
    {
        public static Models.Dtos.Responses.WordResponse? DatabaseWordEntitiesToResponse(Models.Entities.Word word)
        {
            List<Models.Dtos.Responses.VocabularyNotebookReferenceResponse> notebookList = new();

            if(!Data.VocabularyNotebookRepository.FindVocabularyNotebooksByWordId(word.id,out var noteBooksByDB,out _,out _))
            {
                return null;
            }

            foreach(var notebookdata in noteBooksByDB)
            {
                notebookList.Add(new()
                {
                    id = notebookdata.id,
                    name = notebookdata.name,
                    tags = notebookdata.tags
                });
            }

            return new()
            {
                id = word.id,
                kannji = word.kannji,
                kana = word.kana,
                akusennto = word.akusennto,
                meaning = word.meaning,
                note = word.note,
                vocabularynotebooks = notebookList,
                createdat = word.createdat,
                updatedat = word.updatedat
            };
        }

        public static Models.Dtos.Responses.WordsResponseData DatabaseWordEntitiesToResponseByList(List<Models.Entities.Word> wordList)
        {
            List<Models.Dtos.Responses.WordResponse> words = new();

            foreach(var word in wordList)
            {
                var wordByResponse = DatabaseWordEntitiesToResponse(word);

                if(wordByResponse == null)
                    continue;

                words.Add(wordByResponse);
            }

            return new()
            {
                words = words
            };
        }
    }
}