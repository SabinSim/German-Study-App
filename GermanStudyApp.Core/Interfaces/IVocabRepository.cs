using GermanStudyApp.Core.Models;

namespace GermanStudyApp.Core.Interfaces;

public interface IVocabRepository
{
    Task SaveAsync(VocabEntry entry, CancellationToken ct = default);
    
    Task<List<VocabEntry>> GetAllAsync(CancellationToken ct = default);
}