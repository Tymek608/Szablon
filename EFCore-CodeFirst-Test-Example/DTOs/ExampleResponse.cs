using System.Text.Json.Serialization;

namespace EFCore_CodeFirst_Test_Example.DTOs;

/// <summary>
/// Szablon DTO odpowiedzi - jeden rekord zawierający po jednym przykładzie każdego typu.
/// </summary>
public record ExampleResponse(
    int Id,                                 // Typ prosty
    string Name,                            // Typ referencyjny
    int? OptionalAge,                       // Typ nullable
    DateTime CreatedAt,                     // Data i czas
    DateOnly BirthDate,                     // Sama data
    
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    ExampleStatus Status,                   // Enum (jako string w JSON)

    NestedObjectResponse Details,           // Zagnieżdżony obiekt
    List<NestedObjectResponse> Items        // Kolekcja obiektów (lista)
);

public record NestedObjectResponse(
    int Id,
    string Title
);

public enum ExampleStatus
{
    Active = 1,
    Cancelled = 2
}

public record ParentStatsResponse(
    int ParentId,
    string ParentName,
    int TotalExamples,
    decimal AverageValue,
    decimal MaxValue,
    decimal MinValue
);

