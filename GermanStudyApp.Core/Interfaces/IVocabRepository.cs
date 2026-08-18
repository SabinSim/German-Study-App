using GermanStudyApp.Core.Models;

namespace GermanStudyApp.Core.Interfaces;

public interface IVocabRepository
{
    Task SaveAsync(VocabEntry entry, CancellationToken ct = default);

    Task<List<VocabEntry>> GetAllAsync(CancellationToken ct = default);

    Task UpdateAsync(VocabEntry entry, CancellationToken ct = default);

    Task DeleteAsync(VocabEntry entry, CancellationToken ct = default);

    Task DeleteAllAsync(CancellationToken ct = default);
}
