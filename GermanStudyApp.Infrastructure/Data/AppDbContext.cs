using GermanStudyApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GermanStudyApp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<VocabEntry> VocabEntries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=germanstudyapp.db");
    }
}