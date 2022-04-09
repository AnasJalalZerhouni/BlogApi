using BlogApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace BlogApi.Infrastructure
{
    public class BlogContext : DbContext
    {
        private IDbContextTransaction? _currentTransaction;
        public BlogContext(DbContextOptions<BlogContext> opts) : base(opts)
        {
        }

        public DbSet<Article> Articles { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Person> Persons { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<ArticleTag> ArticleTags { get; set; } = null!;
        public DbSet<ArticleFavorite> ArticleFavorites { get; set; } = null!;
        public DbSet<FollowedPeople> FollowedPeople { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticleTag>(b =>
            {
                b.HasKey(t => new { t.ArticleId, t.TagId });

                b.HasOne(pt => pt.Article)
                .WithMany(p => p!.ArticleTags)
                .HasForeignKey(pt => pt.ArticleId);

                b.HasOne(pt => pt.Tag)
                .WithMany(p => p!.ArticleTags)
                .HasForeignKey(pt => pt.TagId);
            });

            modelBuilder.Entity<ArticleFavorite>(b =>
            {
                b.HasKey(t => new {t.ArticleId,t.PersonId });

                b.HasOne(pt => pt.Person)
                .WithMany(t => t!.ArticleFavorites)
                .HasForeignKey(t => t.PersonId);

                b.HasOne(t => t.Article)
                .WithMany(pt => pt!.ArticleFavorites)
                .HasForeignKey(t=> t.ArticleId);
            });

            modelBuilder.Entity<FollowedPeople>(b=>
            {
                b.HasKey(t => new { t.ObserverId, t.TargetId });

                b.HasOne(t => t.Observer)
                .WithMany(pt => pt.Followers)
                .HasForeignKey(t => t.ObserverId)
                .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(t=> t.Target)
                .WithMany(pt => pt.Following)
                .HasForeignKey(t => t.TargetId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    
        public void BeginTransaction()
        {
            if (_currentTransaction != null)
            {
                return;
            }
            if (!Database.IsInMemory())
            {
                _currentTransaction = Database.BeginTransaction(IsolationLevel.ReadCommitted);
            }
        }

        public void CommitTransaction()
        {
            try
            {
                _currentTransaction?.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}
