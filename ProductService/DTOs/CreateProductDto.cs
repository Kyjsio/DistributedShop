using System.ComponentModel.DataAnnotations;

namespace ProductService.DTOs
{
    public record CreateProductDto(
        [Required(ErrorMessage = "Nazwa produktu jest wymagana")] string Name,
        string Description,
        [Range(0.01, 1000000, ErrorMessage = "Cena musi być większa od 0.")]
        decimal Price,
        [Range(0, 10000, ErrorMessage = "Ilość w magazynie nie może być ujemna")]
        int StockQuantity
    );
}