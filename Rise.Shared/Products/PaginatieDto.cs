namespace Rise.Shared.Products
{
    public class PaginatieDto
    {
        public required IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
        public required int TotalCount { get; set; }
    }
}
