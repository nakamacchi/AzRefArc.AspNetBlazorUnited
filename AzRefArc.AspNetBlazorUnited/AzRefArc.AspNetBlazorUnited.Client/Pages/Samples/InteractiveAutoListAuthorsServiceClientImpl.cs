using AzRefArc.AspNetBlazorUnited.Shared.Data;
using System.Net.Http.Json;

namespace AzRefArc.AspNetBlazorUnited.Client.Pages.Samples
{
    public class InteractiveAutoListAuthorsServiceClientImpl : IInteractiveAutoListAuthorsService
    {
        private HttpClient httpClient { get; set; }
        private ILogger<InteractiveAutoListAuthorsServiceClientImpl> logger { get; set; }

        public InteractiveAutoListAuthorsServiceClientImpl(HttpClient httpClient, ILogger<InteractiveAutoListAuthorsServiceClientImpl> logger) { 
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<List<Author>> GetAuthorsAsync()
        {
            return await httpClient.GetFromJsonAsync<List<Author>>("/api/Samples/InteractiveAutoListAuthors/GetAuthors");
        }
    }
}
