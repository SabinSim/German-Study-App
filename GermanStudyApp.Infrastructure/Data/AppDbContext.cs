using GermanStudyApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GermanStudyApp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<VocabEntry> VocabEntries { get; set; }
    
    public DbSet<Deck> Decks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // 데이터베이스를 사용자 홈 디렉토리의 .germanstudyapp 폴더에 저장
        var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var appDataDir = Path.Combine(homeDir, ".germanstudyapp");
        
        // 디렉토리가 없으면 생성
        if (!Directory.Exists(appDataDir))
        {
            Directory.CreateDirectory(appDataDir);
        }
        
        var dbPath = Path.Combine(appDataDir, "germanstudyapp.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // VocabEntry와 Deck 간의 관계 설정
        modelBuilder.Entity<VocabEntry>()
            .HasOne<Deck>()
            .WithMany()
            .HasForeignKey(v => v.DeckId)
            .OnDelete(DeleteBehavior.Restrict);

        // 기본 Deck 시드 데이터
        modelBuilder.Entity<Deck>().HasData(
            new Deck { Id = 1, Name = "Default Deck", ParentDeckId = null }
        );
    }
}