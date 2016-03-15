using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace TestInsights.Data
{
    public class InsightContext : DbContext
    {
        private static readonly string _defaultconnectionString = "" +
            new SqliteConnectionStringBuilder
            {
                DataSource = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\.testInsights")
            };

        private readonly string _connectionString;

        public InsightContext()
        {
        }

        public InsightContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Test> Tests { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public DbSet<TestRun> TestRuns { get; set; }

        public TEntity Find<TEntity>(Func<TEntity, bool> predicate)
            where TEntity : class
            => Find(ChangeTracker.Entries<TEntity>().Select(e => e.Entity), predicate)
                ?? Find(Set<TEntity>(), predicate);

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(_connectionString ?? _defaultconnectionString);

        private static TEntity Find<TEntity>(IEnumerable<TEntity> source, Func<TEntity, bool> predicate)
            where TEntity : class
            => source.FirstOrDefault(predicate);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestPass>();
            modelBuilder.Entity<TestFailed>();
            modelBuilder.Entity<TestSkipped>();
        }
    }
}
