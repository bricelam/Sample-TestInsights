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

        public DbSet<TestAssembly> Assemblies { get; set; }
        public DbSet<TestClass> Classes { get; set; }
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
            modelBuilder.Entity<TestClass>(
                x =>
                {
                    x.Property<string>("AssemblyName");
                    x.HasKey("AssemblyName", nameof(TestClass.Name));
                    x.HasOne(c => c.Assembly).WithMany(a => a.Classes).HasForeignKey("AssemblyName");
                });
            modelBuilder.Entity<Test>(
                x =>
                {
                    x.Property<string>("AssemblyName");
                    x.HasKey("AssemblyName", "ClassName", nameof(Test.Name));
                    x.HasOne(t => t.Class).WithMany(c => c.Tests).HasForeignKey("AssemblyName", "ClassName");
                });
            modelBuilder.Entity<TestResult>(
                x =>
                {
                    x.Property<string>("AssemblyName");
                    x.Property<string>("ClassName");
                    x.HasKey("AssemblyName", "ClassName", "TestName", nameof(TestResult.StartTime));
                    x.HasDiscriminator<string>("Result")
                        .HasValue<TestPassedResult>("Pass")
                        .HasValue<TestFailedResult>("Fail")
                        .HasValue<TestSkippedResult>("Skip");
                    x.HasOne(r => r.Test).WithMany(t => t.Results).HasForeignKey("AssemblyName", "ClassName", "TestName");
                });
        }
    }
}
