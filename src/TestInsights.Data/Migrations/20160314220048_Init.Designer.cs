using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestInsights.Data.Migrations
{
    [DbContext(typeof(InsightContext))]
    [Migration("20160314220048_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rc2-20207");

            modelBuilder.Entity("TestInsights.Data.Test", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Assembly");

                    b.Property<string>("Class");

                    b.Property<string>("Collection");

                    b.Property<string>("DisplayName");

                    b.Property<string>("Method");

                    b.HasKey("Id");

                    b.ToTable("Tests");
                });

            modelBuilder.Entity("TestInsights.Data.TestResult", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<decimal>("ExecutionTime");

                    b.Property<int?>("TestId");

                    b.Property<int?>("TestRunId");

                    b.HasKey("Id");

                    b.HasIndex("TestId");

                    b.HasIndex("TestRunId");

                    b.ToTable("TestResults");

                    b.HasDiscriminator<string>("Discriminator").HasValue("TestResult");
                });

            modelBuilder.Entity("TestInsights.Data.TestRun", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("StartTime");

                    b.Property<string>("TestEnvironment");

                    b.HasKey("Id");

                    b.ToTable("TestRuns");
                });

            modelBuilder.Entity("TestInsights.Data.TestFailed", b =>
                {
                    b.HasBaseType("TestInsights.Data.TestResult");

                    b.Property<string>("ExceptionType");

                    b.Property<string>("Message");

                    b.Property<string>("StackTrace");

                    b.ToTable("TestFailed");

                    b.HasDiscriminator().HasValue("TestFailed");
                });

            modelBuilder.Entity("TestInsights.Data.TestPass", b =>
                {
                    b.HasBaseType("TestInsights.Data.TestResult");


                    b.ToTable("TestPass");

                    b.HasDiscriminator().HasValue("TestPass");
                });

            modelBuilder.Entity("TestInsights.Data.TestSkipped", b =>
                {
                    b.HasBaseType("TestInsights.Data.TestResult");

                    b.Property<string>("Reason");

                    b.ToTable("TestSkipped");

                    b.HasDiscriminator().HasValue("TestSkipped");
                });

            modelBuilder.Entity("TestInsights.Data.TestResult", b =>
                {
                    b.HasOne("TestInsights.Data.Test")
                        .WithMany()
                        .HasForeignKey("TestId");

                    b.HasOne("TestInsights.Data.TestRun")
                        .WithMany()
                        .HasForeignKey("TestRunId");
                });
        }
    }
}
