using GermanStudyApp.Core.Interfaces;
using GermanStudyApp.Core.Models;
using GermanStudyApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GermanStudyApp.Infrastructure;

public class  DeckRepository : IDeckRepository
{
    public async Task SaveAsync(Deck deck, CancellationToken ct = default)
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();
        db.Decks.Add(deck);
        await db.SaveChangesAsync(ct);
    }

    public async Task<List<Deck>> GetAllAsync(CancellationToken ct = default)
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();
        return await db.Decks.ToListAsync(ct);
    }

    public async Task UpdateAsync(Deck deck, CancellationToken ct = default)
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();
        db.Decks.Update(deck);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Deck deck, CancellationToken ct = default)
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();
        db.Decks.Remove(deck);
        await db.SaveChangesAsync(ct);
    }
}