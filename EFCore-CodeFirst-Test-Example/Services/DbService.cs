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
    // WZORZEC POST (Walidacja biznesowa + Transakcja)
    // --------------------------------------------------------------------------------
    public async Task AddExampleAsync(ExampleRequest request, CancellationToken cancellationToken)
    {
        // 1. Rozpoczynamy transakcję bazodanową
        using var transaction = await ctx.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // 2. Przykład walidacji: Sprawdzenie unikalności nazwy
            var alreadyExists = await ctx.Examples
                .AnyAsync(e => e.Name.Trim().ToLower() == request.Name.Trim().ToLower(), cancellationToken);
            if (alreadyExists)
            {
                throw new ConflictException($"Example with name '{request.Name}' already exists.");
            }

            // 3. Przykład walidacji: Sprawdzenie istnienia powiązanego rekordu (np. rodzica)
            /*
            var parentExists = await ctx.Parents.AnyAsync(p => p.Id == request.ParentId, cancellationToken);
            if (!parentExists)
            {
                throw new NotFoundException($"Parent with ID {request.ParentId} was not found.");
            }
            */

            // 4. Tworzenie i dodanie głównego obiektu
            var newEntity = new ExampleEntity
            {
                Name = request.Name,
                Value = request.Age
            };
            await ctx.Examples.AddAsync(newEntity, cancellationToken);
            await ctx.SaveChangesAsync(cancellationToken); // Save generuje wygenerowane przez bazę ID dla newEntity

            // 5. Przykład dodawania powiązanych rekordów (np. w tabeli łączącej)
            /*
            var junction = new JunctionEntity
            {
                FirstId = newEntity.Id,
                SecondId = request.SecondId
            };
            await ctx.Junctions.AddAsync(junction, cancellationToken);
            await ctx.SaveChangesAsync(cancellationToken);
            */

            // 6. Zatwierdzamy transakcję
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            // 7. W razie błędu wycofujemy wszystkie operacje
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}


