using AzRefArc.AspNetBlazorUnited.Shared.Data;

namespace AzRefArc.AspNetBlazorUnited.Client.Pages.Samples
{
    public interface IInteractiveAutoListAuthorsService
    {
        public Task<List<Author>> GetAuthorsAsync();
    }
}
