using GermanStudyApp.Core.Models;

namespace GermanStudyApp.Core.Interfaces;

public interface IDeckRepository
{
    Task SaveAsync(Deck deck, CancellationToken ct = default);
    Task<List<Deck>> GetAllAsync(CancellationToken ct = default);
    Task UpdateAsync(Deck deck, CancellationToken ct = default);
    Task DeleteAsync(Deck deck, CancellationToken ct = default);
}