using GermanStudyApp.Core.Interfaces;
using GermanStudyApp.Core.Models;
using GermanStudyApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GermanStudyApp.Infrastructure;

public class VocabRepository : IVocabRepository
{
    public async Task SaveAsync(VocabEntry entry, CancellationToken ct = default)
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();
        db.VocabEntries.Add(entry);
        await db.SaveChangesAsync(ct);
    }

    public async Task<List<VocabEntry>> GetAllAsync(CancellationToken ct = default)
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();
        return await db.VocabEntries.ToListAsync(ct);
    }

    public async Task UpdateAsync(VocabEntry entry, CancellationToken ct = default)
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();
        db.VocabEntries.Update(entry);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(VocabEntry entry, CancellationToken ct = default)
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();
        db.VocabEntries.Remove(entry);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAllAsync(CancellationToken ct = default)
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();
        db.VocabEntries.RemoveRange(db.VocabEntries);
        await db.SaveChangesAsync(ct);
    }
}