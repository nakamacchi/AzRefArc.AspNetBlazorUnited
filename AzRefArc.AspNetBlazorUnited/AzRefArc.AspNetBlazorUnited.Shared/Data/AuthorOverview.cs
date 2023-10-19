namespace AzRefArc.AspNetBlazorUnited.Shared.Data
{
    public record AuthorOverview
    {
        public string AuthorId { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? State { get; set; }
        public bool Contract { get; set; }
    }
}
