﻿using AzRefArc.AspNetBlazorUnited.Data;
using AzRefArc.AspNetBlazorUnited.Client.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AzRefArc.AspNetBlazorUnited.WebApi.Samples
{
    [ApiController]
    [Route("/api/Samples/InteractiveAutoListAuthors")]
    public class InteractiveAutoListAuthorsController : ControllerBase
    {
        private IDbContextFactory<PubsDbContext> dbFactory { get; set; }

        public InteractiveAutoListAuthorsController(IDbContextFactory<PubsDbContext> dbFactory)
        {
            this.dbFactory = dbFactory ?? throw new ArgumentNullException("dbFactory");
        }

        [HttpGet("GetAuthors")]
        public async Task<List<Author>> GetAuthors()
        {
            await using (var pubs = await dbFactory.CreateDbContextAsync())
            {
                return await pubs.Authors.ToListAsync();
            }
        }
    }
}
