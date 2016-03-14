using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace TestInsights.Data
{
    public class InsightContext : DbContext
    {
        private static readonly string _connectionString = "" +
            new SqliteConnectionStringBuilder
            {
                DataSource = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\.testInsights")
            };

        public DbSet<Test> Tests { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public DbSet<TestRun> TestRuns { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(_connectionString);
    }
}
