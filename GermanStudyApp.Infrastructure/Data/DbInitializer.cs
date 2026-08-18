using GermanStudyApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GermanStudyApp.Infrastructure.Data;

public static class DbInitializer
{
    /// <summary>
    /// 데이터베이스를 초기화하고 필요한 테이블과 기본 데이터를 생성합니다.
    /// </summary>
    public static void Initialize()
    {
        using var db = new AppDbContext();
        
        // 데이터베이스와 테이블 생성
        db.Database.EnsureCreated();
        
        // 기본 Deck이 없으면 생성
        if (!db.Decks.Any())
        {
            db.Decks.Add(new Deck
            {
                Name = "Default Deck",
                ParentDeckId = null
            });
            db.SaveChanges();
        }
    }
}

