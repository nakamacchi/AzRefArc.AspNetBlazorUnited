namespace AzRefArc.AspNetBlazorUnited.Client.Data
{
    public class PublisherOverview
    {
        public string PublisherId { get; set; } = string.Empty;
        public string PublisherName { get; set; } = string.Empty;
        public List<TitleOverview> TitleOverviews { get; set; } = new List<TitleOverview>();
    }
}
