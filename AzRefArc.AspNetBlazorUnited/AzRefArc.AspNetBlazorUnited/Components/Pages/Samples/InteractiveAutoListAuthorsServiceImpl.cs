using AzRefArc.AspNetBlazorUnited.Client.Pages.Samples;
using AzRefArc.AspNetBlazorUnited.Data;
using AzRefArc.AspNetBlazorUnited.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace AzRefArc.AspNetBlazorUnited.Components.Pages.Samples
{
    public class InteractiveAutoListAuthorsServiceServerImpl : IInteractiveAutoListAuthorsService
    {
        private IDbContextFactory<PubsDbContext> dbContextFactory { get; set; }
        private ILogger<InteractiveAutoListAuthorsServiceClientImpl> logger { get; set; }

        public InteractiveAutoListAuthorsServiceServerImpl(IDbContextFactory<PubsDbContext> dbContextFactory, ILogger<InteractiveAutoListAuthorsServiceClientImpl> logger)
        {
            this.dbContextFactory = dbContextFactory;
            this.logger = logger;
        }

        public async Task<List<Author>> GetAuthorsAsync()
        {
            using (var pubs = dbContextFactory.CreateDbContext())
            {
                return await pubs.Authors.ToListAsync();
            }
        }
    }
}
