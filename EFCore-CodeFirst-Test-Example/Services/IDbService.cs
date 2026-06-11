using EFCore_CodeFirst_Test_Example.DTOs;

namespace EFCore_CodeFirst_Test_Example.Services;

public interface IDbService
{
    // WZORZEC 1: GET ALL z opcjonalnym filtrem (np. /api/professors?lastName=Joh lub /api/guests?lastName=Smi)
    // Zwraca listę obiektów przefiltrowaną po przekazanym ciągu znaków
    Task<ICollection<ExampleResponse>> GetExamplesAsync(string? filter, CancellationToken cancellationToken);

    // WZORZEC 2: GET BY ID z limitowaniem podkolekcji (np. /api/characters/{id}?titlesCount=5) i wyjątkiem NotFound
    // Pobiera jeden główny obiekt oraz dołącza powiązane dane z limitem ilościowym
    Task<ExampleResponse> GetExampleByIdAsync(int id, int? limitCount, CancellationToken cancellationToken);

    // WZORZEC 3: GET z zaawansowanym filtrowaniem, sortowaniem i paginacją
    Task<ICollection<ExampleResponse>> GetAdvancedExamplesAsync(
        string? name,
        int? minYear,
        decimal? maxValue,
        string? sortBy,
        bool descending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    // WZORZEC 4: GET z agregacją / statystykami powiązanych danych dla rodzica
    Task<ParentStatsResponse> GetParentStatsAsync(int parentId, CancellationToken cancellationToken);
}
