using GermanStudyApp.Core.Models;

namespace GermanStudyApp.Core.Interfaces;

public interface IVocabImportService
{
    List<VocabEntry> ParseTxt(string fileContent, int deckId);
}