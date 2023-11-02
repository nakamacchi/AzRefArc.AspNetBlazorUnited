using AzRefArc.AspNetBlazorUnited.Client.Data;

namespace AzRefArc.AspNetBlazorUnited.Client.Pages.Samples
{
    public interface IInteractiveAutoListAuthorsService
    {
        public Task<List<Author>> GetAuthorsAsync();
    }
}
