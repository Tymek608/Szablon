using EFCore_CodeFirst_Test_Example.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFCore_CodeFirst_Test_Example.Data;

public class DatabaseContext(DbContextOptions<DatabaseContext> opt, IConfiguration configuration) : DbContext(opt)
{
    // Rejestracja tabel w bazie
    public virtual DbSet<ExampleEntity> Examples { get; set; }
    public virtual DbSet<RelatedParent> RelatedParents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ustawienie domyślnego schematu bazy z pliku konfiguracyjnego appsettings.json
        modelBuilder.HasDefaultSchema(configuration["DB:DefaultSchema"]);

        // ================================================================================
        // WZORZEC MAPOWANIA DANYCH: FLUENT API
        // ================================================================================
        modelBuilder.Entity<ExampleEntity>(opt =>
        {
            // 1. KLUCZ GŁÓWNY (Primary Key)
            opt.HasKey(x => x.Id);

            // 2. TYP STRING - WYMAGANY (VARCHAR/NVARCHAR o określonej długości, NOT NULL)
            opt.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired(); // IsRequired() wymusza NOT NULL w bazie

            // 3. TYP STRING - OPCJONALNY (dopuszcza NULL w bazie)
            opt.Property(x => x.OptionalDescription)
                .HasMaxLength(500); // brak .IsRequired() pozwala na NULL w bazie

            // 4. TYP DECIMAL - WYMAGANY (np. wagi, oceny, kwoty) - ZAWSZE podawaj HasColumnType!
            opt.Property(x => x.Value)
                .HasColumnType("decimal(10,2)") // decimal(precyzja, skala) np. 12345678.90
                .IsRequired();

            // 5. TYP DECIMAL - OPCJONALNY (dopuszcza NULL)
            opt.Property(x => x.OptionalGrade)
                .HasColumnType("decimal(2,1)"); // np. ocena typu 4.5. Bez .IsRequired(), bo jest nullable

            // 6. TYP INT - WYMAGANY (NOT NULL w bazie)
            opt.Property(x => x.Year)
                .IsRequired();

            // 7. TYP INT - OPCJONALNY (NULL w bazie)
            opt.Property(x => x.OptionalCount); // int? mapuje się automatycznie jako NULL

            // 8. TYP DATETIME - WYMAGANY (NOT NULL)
            opt.Property(x => x.CreatedAt)
                .IsRequired();

            // 9. TYP DATETIME - OPCJONALNY (NULL w bazie)
            opt.Property(x => x.ReturnedAt); // DateTime? mapuje się automatycznie jako NULL

            // 10. TYP DATEONLY - WYMAGANY (NOT NULL)
            opt.Property(x => x.CheckOut)
                .IsRequired();


            // --------------------------------------------------------------------------------
            // KONFIGURACJA RELACJI 1:N (Jeden do Wielu) - np. Jeden rodzic ma wielu przykładów
            // --------------------------------------------------------------------------------
            opt.HasOne(x => x.EntityParent)             // Obiekt-rodzic w tej encji
                .WithMany(parent => parent.Examples)     // Kolekcja dzieci w klasie rodzica
                .HasForeignKey(x => x.RelatedParentId);  // Kolumna z kluczem obcym (Foreign Key)
        });

        // WZORZEC DLA RODZICA (1:N)
        modelBuilder.Entity<RelatedParent>(opt =>
        {
            opt.HasKey(x => x.RelatedParentId);
            opt.Property(x => x.Name).HasMaxLength(100).IsRequired();
        });

        // ================================================================================
        // WZORZEC MAPOWANIA TABELI ŁĄCZĄCEJ (Relacja N:M z atrybutami, np. Plecak/Backpack)
        // ================================================================================
        /*
        modelBuilder.Entity<JunctionEntity>(opt =>
        {
            // 1. ZŁOŻONY KLUCZ GŁÓWNY (Composite Primary Key) - zrobiony z dwóch kluczy obcych
            opt.HasKey(x => new { x.FirstId, x.SecondId });

            // 2. Opcjonalne dodatkowe atrybuty w tabeli łączącej (np. ilość, status, ocena)
            opt.Property(x => x.Amount).IsRequired();
            opt.Property(x => x.Status).HasMaxLength(50).IsRequired();
            opt.Property(x => x.Grade).HasColumnType("decimal(2,1)"); // dopuszcza NULL

            // 3. Konfiguracja dwóch relacji 1:N, które tworzą relację N:M
            opt.HasOne(x => x.FirstEntity)
                .WithMany(first => first.Junctions)
                .HasForeignKey(x => x.FirstId);

            opt.HasOne(x => x.SecondEntity)
                .WithMany(second => second.Junctions)
                .HasForeignKey(x => x.SecondId);
        });
        */
    }
}
