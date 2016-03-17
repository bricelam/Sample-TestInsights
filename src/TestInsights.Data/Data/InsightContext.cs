using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestInsights.Models;

namespace TestInsights.Data
{
    public class InsightContext : DbContext
    {
        private readonly string _connectionString;

        public InsightContext()
        {
            _connectionString = "" +
                new SqliteConnectionStringBuilder
                {
                    DataSource = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\.testInsights")
                };
        }

        public InsightContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Test> Tests { get; set; }
        public DbSet<TestResult> Results { get; set; }

        public TEntity Find<TEntity>(Func<TEntity, bool> predicate)
            where TEntity : class
            => Find(ChangeTracker.Entries<TEntity>().Select(e => e.Entity), predicate)
                ?? Find(Set<TEntity>(), predicate);

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(_connectionString).UseLoggerFactory(new LoggerFactory().AddConsole());

        private static TEntity Find<TEntity>(IEnumerable<TEntity> source, Func<TEntity, bool> predicate)
            where TEntity : class
            => source.FirstOrDefault(predicate);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Test>().HasKey(t => new { t.Assembly, t.Class, t.Name });
            modelBuilder.Entity<TestResult>().HasKey("TestAssembly", "TestClass", "TestName", "StartTime");
            modelBuilder.Entity<TestPassedResult>();
            modelBuilder.Entity<TestFailedResult>();
            modelBuilder.Entity<TestSkippedResult>();
        }
    }
}
