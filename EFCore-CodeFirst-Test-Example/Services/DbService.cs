using EFCore_CodeFirst_Test_Example.Data;
using EFCore_CodeFirst_Test_Example.DTOs;
using EFCore_CodeFirst_Test_Example.Entities;
using EFCore_CodeFirst_Test_Example.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EFCore_CodeFirst_Test_Example.Services;

public class DbService(DatabaseContext ctx) : IDbService
{
    // --------------------------------------------------------------------------------
    // WZORZEC GET ALL (Opcjonalny filtr tekstowy + Projekcja DTO)
    // --------------------------------------------------------------------------------
    public async Task<ICollection<ExampleResponse>> GetExamplesAsync(string? filter, CancellationToken cancellationToken)
    {
        return await ctx.Examples
            .Where(e => filter == null || e.Name.Contains(filter))
            .Select(e => new ExampleResponse(
                e.Id,
                e.Name,
                e.OptionalCount,
                e.CreatedAt,
                DateOnly.FromDateTime(e.CreatedAt),
                ExampleStatus.Active,
                new NestedObjectResponse(e.RelatedParentId, e.EntityParent.Name),
                new List<NestedObjectResponse>()
            ))
            .ToListAsync(cancellationToken);
    }

    // --------------------------------------------------------------------------------
    // WZORZEC GET BY ID (Wyszukanie pojedynczego obiektu + limitowanie powiązanej kolekcji (.Take()) + NotFoundException)
    // --------------------------------------------------------------------------------
    public async Task<ExampleResponse> GetExampleByIdAsync(int id, int? limitCount, CancellationToken cancellationToken)
    {
        // Domyślny limit dla podkolekcji (np. z parametru titlesCount/limitCount)
        int itemsLimit = limitCount ?? 3;

        return await ctx.Examples
            .Where(e => e.Id == id)
            .Select(e => new ExampleResponse(
                e.Id,
                e.Name,
                e.OptionalCount,
                e.CreatedAt,
                DateOnly.FromDateTime(e.CreatedAt),
                ExampleStatus.Active,
                new NestedObjectResponse(e.RelatedParentId, e.EntityParent.Name),
                
                // WZORZEC LIMITOWANIA PODKOLEKCJI: tak limitujemy np. titlesCount lub rezerwacje
                // e.Children.Take(itemsLimit).Select(c => new NestedObjectResponse(c.Id, c.Title)).ToList()
                new List<NestedObjectResponse>()
            ))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Example with ID {id} was not found.");
    }

    // --------------------------------------------------------------------------------
    // WZORZEC GET ADVANCED (Filtrowanie dynamiczne, Sortowanie dynamiczne, Stronicowanie)
    // --------------------------------------------------------------------------------
    public async Task<ICollection<ExampleResponse>> GetAdvancedExamplesAsync(
        string? name,
        int? minYear,
        decimal? maxValue,
        string? sortBy,
        bool descending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = ctx.Examples.AsQueryable();

        // 1. Filtrowanie dynamiczne
        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(e => e.Name.Contains(name));
        }

        if (minYear.HasValue)
        {
            query = query.Where(e => e.Year >= minYear.Value);
        }

        if (maxValue.HasValue)
        {
            query = query.Where(e => e.Value <= maxValue.Value);
        }

        // 2. Sortowanie dynamiczne
        query = sortBy?.ToLower() switch
        {
            "name" => descending ? query.OrderByDescending(e => e.Name) : query.OrderBy(e => e.Name),
            "year" => descending ? query.OrderByDescending(e => e.Year) : query.OrderBy(e => e.Year),
            "value" => descending ? query.OrderByDescending(e => e.Value) : query.OrderBy(e => e.Value),
            _ => descending ? query.OrderByDescending(e => e.Id) : query.OrderBy(e => e.Id)
        };

        // 3. Stronicowanie (Paging)
        query = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        // 4. Projekcja do DTO
        return await query
            .Select(e => new ExampleResponse(
                e.Id,
                e.Name,
                e.OptionalCount,
                e.CreatedAt,
                DateOnly.FromDateTime(e.CreatedAt),
                ExampleStatus.Active,
                new NestedObjectResponse(e.RelatedParentId, e.EntityParent.Name),
                new List<NestedObjectResponse>()
            ))
            .ToListAsync(cancellationToken);
    }

    // --------------------------------------------------------------------------------
    // WZORZEC GET AGGREGATION / STATS (Agregacja, grupowanie i sprawdzanie istnienia rodzica)
    // --------------------------------------------------------------------------------
    public async Task<ParentStatsResponse> GetParentStatsAsync(int parentId, CancellationToken cancellationToken)
    {
        // Sprawdzamy czy rodzic istnieje
        var parentName = await ctx.RelatedParents
            .Where(rp => rp.RelatedParentId == parentId)
            .Select(rp => rp.Name)
            .FirstOrDefaultAsync(cancellationToken);

        if (parentName == null)
        {
            throw new NotFoundException($"Parent with ID {parentId} was not found.");
        }

        // Liczymy agregaty dla przykładów przypisanych do tego rodzica
        var stats = await ctx.Examples
            .Where(e => e.RelatedParentId == parentId)
            .GroupBy(e => e.RelatedParentId)
            .Select(g => new
            {
                Count = g.Count(),
                AverageValue = g.Average(e => e.Value),
                MaxValue = g.Max(e => e.Value),
                MinValue = g.Min(e => e.Value)
            })
            .FirstOrDefaultAsync(cancellationToken);

        // Jeśli rodzic istnieje, ale nie ma dzieci, zwracamy puste/zerowe wartości
        return new ParentStatsResponse(
            parentId,
            parentName,
            stats?.Count ?? 0,
            stats?.AverageValue ?? 0,
            stats?.MaxValue ?? 0,
            stats?.MinValue ?? 0
        );
    }
}
