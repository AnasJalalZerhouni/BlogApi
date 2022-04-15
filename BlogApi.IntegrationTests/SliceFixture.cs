using BlogApi.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApi.IntegrationTests
{
    public class SliceFixture : IDisposable
    {
        static readonly IConfiguration Config;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IServiceProvider _provider;
        private readonly string DbName = Guid.NewGuid() + ".db";

        static SliceFixture()
        {
            Config = new ConfigurationBuilder()
               .AddEnvironmentVariables()
               .Build();
        }

        public SliceFixture()
        {

            var Program = new WebApplicationFactory<Program>();

            var wb = Program.WithWebHostBuilder(builder =>
            {
                DbContextOptionsBuilder b = new DbContextOptionsBuilder();
                b.UseInMemoryDatabase(DbName);
                builder.ConfigureServices(x=>
                {
                    x.AddSingleton(new BlogContext(b.Options));
                });
            });
            _provider = wb.Services.CreateScope().ServiceProvider;

            GetDbContext().Database.EnsureCreated();
            _scopeFactory = _provider.GetService<IServiceScopeFactory>();
        }

        public BlogContext GetDbContext()
        {
            return _provider.GetService<BlogContext>() ?? throw new InvalidOperationException();
        }

        public void Dispose()
        {
            File.Delete(DbName);
        }

        public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                await action(scope.ServiceProvider);
            }
        }

        public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                return await action(scope.ServiceProvider);
            }
        }

        public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            return ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetService<IMediator>();

                return mediator.Send(request);
            });
        }

        public Task SendAsync(IRequest request)
        {
            return ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetService<IMediator>();

                return mediator.Send(request);
            });
        }

        public Task ExecuteDbContextAsync(Func<BlogContext, Task> action)
        {
            return ExecuteScopeAsync(sp => action(sp.GetService<BlogContext>()));
        }

        public Task<T> ExecuteDbContextAsync<T>(Func<BlogContext, Task<T>> action)
        {
            return ExecuteScopeAsync(sp =>
            {
                var context = sp.GetService<BlogContext>();
                var ka = action(context).Result;
                return action(context);
            });
        }

        public Task InsertAsync(params object[] entities)
        {
            return ExecuteDbContextAsync(db =>
            {
                foreach (var entity in entities)
                {
                    db.Add(entity);
                }
                return db.SaveChangesAsync();
            });
        }
    }
}
