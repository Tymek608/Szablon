namespace EFCore_CodeFirst_Test_Example.Entities;

public class ExampleEntity
{
    // 1. KLUCZ GŁÓWNY (Primary Key) - int automatycznie staje się IDENTITY(1,1) w bazie
    public int Id { get; set; }

    // 2. TYP STRING (wymagany) - w bazie VARCHAR / NVARCHAR (wartość nie może być NULL)
    public string Name { get; set; } = string.Empty;

    // 3. TYP STRING (opcjonalny / dopuszczający NULL) - dodajemy pytajnik "?"
    public string? OptionalDescription { get; set; }

    // 4. TYP DECIMAL (zmiennoprzecinkowy) - wymagany (np. kwota, ocena, waga)
    public decimal Value { get; set; }

    // 5. TYP DECIMAL (opcjonalny / dopuszczający NULL) - np. ocena, która może nie być jeszcze wystawiona
    public decimal? OptionalGrade { get; set; }

    // 6. TYP INT (całkowity) - wymagany (np. rok, ilość)
    public int Year { get; set; }

    // 7. TYP INT (opcjonalny / dopuszczający NULL)
    public int? OptionalCount { get; set; }

    // 8. TYP DATETIME (data i czas) - wymagany (np. data utworzenia, data wypożyczenia)
    public DateTime CreatedAt { get; set; }

    // 9. TYP DATETIME (opcjonalny / dopuszczający NULL) - np. data oddania książki
    public DateTime? ReturnedAt { get; set; }

    // 10. TYP DATEONLY (sama data bez czasu) - wymagany (np. data urodzenia, data wymeldowania)
    public DateOnly CheckOut { get; set; }
    // --------------------------------------------------------------------------------
    // RELACJE BAZODANOWE (NAWIGACJA)
    // --------------------------------------------------------------------------------

    // Relacja 1:N (Jeden do Wielu) - Klucz Obcy (Foreign Key) wskazujący na inną encję (np. Department)
    public int RelatedParentId { get; set; }
    public virtual RelatedParent EntityParent { get; set; } = null!;

    // Relacja N:M lub 1:N (Nawigacja kolekcji) - ta encja ma wiele powiązanych dzieci
    // public virtual ICollection<ChildEntity> Children { get; set; } = new List<ChildEntity>();
}

// Pomocnicza klasa dla celów demonstracji relacji powyżej
public class RelatedParent
{
    public int RelatedParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual ICollection<ExampleEntity> Examples { get; set; } = new List<ExampleEntity>();
}
