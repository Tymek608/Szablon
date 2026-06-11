using System.ComponentModel.DataAnnotations;

namespace EFCore_CodeFirst_Test_Example.DTOs;

/// <summary>
/// Szablon DTO żądania - jeden rekord zawierający po jednym przykładzie walidacji.
/// </summary>
public record ExampleRequest(
    [Required(ErrorMessage = "Name jest wymagane.")]
    [MaxLength(100, ErrorMessage = "Maksymalnie 100 znaków.")]
    string Name,                            // Walidacja tekstu

    [Required]
    [Range(1, 150, ErrorMessage = "Przedział 1-150.")]
    int Age,                                // Walidacja liczby

    [Required]
    [EmailAddress(ErrorMessage = "Niepoprawny format email.")]
    string Email,                           // Walidacja formatu email (lub [Phone], lub [RegularExpression])

    [Required]
    NestedRequest Address,                  // Zagnieżdżony obiekt

    [Required]
    [MinLength(1, ErrorMessage = "Min. 1 element.")]
    List<NestedRequest> Items               // Kolekcja obiektów (lista)
);

public record NestedRequest(
    [Required, MaxLength(100)] string Street
);



