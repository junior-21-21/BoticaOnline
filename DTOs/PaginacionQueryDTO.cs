namespace BoticaOnline.DTOs
{
    public class PaginacionQueryDTO
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchQuery { get; set; }
        public string? Category { get; set; }
        public bool LowStockOnly { get; set; } = false;
    }
}