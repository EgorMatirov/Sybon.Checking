﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Sybon.Checking;
using Sybon.Checking.Repositories.SubmitsRepository;
using System;

namespace Sybon.Checking.Migrations
{
    [DbContext(typeof(CheckingContext))]
    [Migration("20171017190549_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Sybon.Checking.Repositories.CompilersRepository.Compiler", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Args");

                    b.Property<long>("ResourceLimitsId");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.HasIndex("ResourceLimitsId");

                    b.ToTable("Compilers");
                });

            modelBuilder.Entity("Sybon.Checking.Repositories.CompilersRepository.ResourceLimits", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("MemoryLimitBytes");

                    b.Property<long>("NumberOfProcesses");

                    b.Property<long>("OutputLimitBytes");

                    b.Property<long>("RealTimeLimitMillis");

                    b.Property<long>("TimeLimitMillis");

                    b.HasKey("Id");

                    b.ToTable("ResourceLimits");
                });

            modelBuilder.Entity("Sybon.Checking.Repositories.SubmitsRepository.BuildResult", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Output");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.ToTable("BuildResult");
                });

            modelBuilder.Entity("Sybon.Checking.Repositories.SubmitsRepository.ResourceUsage", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("MemoryUsageBytes");

                    b.Property<long>("TimeUsageMillis");

                    b.HasKey("Id");

                    b.ToTable("ResourceUsage");
                });

            modelBuilder.Entity("Sybon.Checking.Repositories.SubmitsRepository.Solution", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Data");

                    b.Property<int>("FileType");

                    b.HasKey("Id");

                    b.ToTable("Solution");
                });

            modelBuilder.Entity("Sybon.Checking.Repositories.SubmitsRepository.Submit", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("CompilerId");

                    b.Property<DateTime>("Created");

                    b.Property<bool>("PretestsOnly");

                    b.Property<long>("ProblemId");

                    b.Property<long?>("ResultId");

                    b.Property<long>("SolutionId");

                    b.Property<long?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CompilerId");

                    b.HasIndex("ResultId")
                        .IsUnique()
                        .HasFilter("[ResultId] IS NOT NULL");

                    b.HasIndex("SolutionId")
                        .IsUnique();

                    b.ToTable("Submits");
                });

            modelBuilder.Entity("Sybon.Checking.Repositories.SubmitsRepository.SubmitResult", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("BuildResultId");

                    b.HasKey("Id");

                    b.HasIndex("BuildResultId");

                    b.ToTable("SubmitResult");
                });

            modelBuilder.Entity("Sybon.Checking.Repositories.SubmitsRepository.TestGroupResult", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Executed");

                    b.Property<string>("InternalId");

                    b.Property<long>("SubmitResultId");

                    b.HasKey("Id");

                    b.HasIndex("SubmitResultId");

                    b.ToTable("TestGroupResult");
                });

            modelBuilder.Entity("Sybon.Checking.Repositories.SubmitsRepository.TestResult", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ActualResult");

                    b.Property<string>("ExpectedResult");

                    b.Property<string>("Input");

                    b.Property<string>("JudgeMessage");

                    b.Property<long?>("ResourceUsageId");

                    b.Property<int>("Status");

                    b.Property<long>("TestGroupResultId");

                    b.HasKey("Id");

                    b.HasIndex("ResourceUsageId");

                    b.HasIndex("TestGroupResultId");

                    b.ToTable("TestResult");
                });

            modelBuilder.Entity("Sybon.Checking.Repositories.CompilersRepository.Compiler", b =>
                {
                    b.HasOne("Sybon.Checking.Repositories.CompilersRepository.ResourceLimits", "ResourceLimits")
                        .WithMany()
                        .HasForeignKey("ResourceLimitsId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sybon.Checking.Repositories.SubmitsRepository.Submit", b =>
                {
                    b.HasOne("Sybon.Checking.Repositories.CompilersRepository.Compiler", "Compiler")
                        .WithMany()
                        .HasForeignKey("CompilerId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Sybon.Checking.Repositories.SubmitsRepository.SubmitResult", "Result")
                        .WithOne("Submit")
                        .HasForeignKey("Sybon.Checking.Repositories.SubmitsRepository.Submit", "ResultId");

                    b.HasOne("Sybon.Checking.Repositories.SubmitsRepository.Solution", "Solution")
                        .WithOne("Submit")
                        .HasForeignKey("Sybon.Checking.Repositories.SubmitsRepository.Submit", "SolutionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sybon.Checking.Repositories.SubmitsRepository.SubmitResult", b =>
                {
                    b.HasOne("Sybon.Checking.Repositories.SubmitsRepository.BuildResult", "BuildResult")
                        .WithMany()
                        .HasForeignKey("BuildResultId");
                });

            modelBuilder.Entity("Sybon.Checking.Repositories.SubmitsRepository.TestGroupResult", b =>
                {
                    b.HasOne("Sybon.Checking.Repositories.SubmitsRepository.SubmitResult", "SubmitResult")
                        .WithMany("TestResults")
                        .HasForeignKey("SubmitResultId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Sybon.Checking.Repositories.SubmitsRepository.TestResult", b =>
                {
                    b.HasOne("Sybon.Checking.Repositories.SubmitsRepository.ResourceUsage", "ResourceUsage")
                        .WithMany()
                        .HasForeignKey("ResourceUsageId");

                    b.HasOne("Sybon.Checking.Repositories.SubmitsRepository.TestGroupResult", "TestGroupResult")
                        .WithMany("TestResults")
                        .HasForeignKey("TestGroupResultId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
