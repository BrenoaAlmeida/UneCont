﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Model;

namespace Model.Migrations
{
    [DbContext(typeof(UneContexto))]
    [Migration("20250116195943_VersaoInicial")]
    partial class VersaoInicial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Model.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DataDeInsercao");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("Log");
                });

            modelBuilder.Entity("Model.LogAgora", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CacheStatus")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<DateTime>("DataHoraInsercao");

                    b.Property<string>("HttpMethod")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<int>("LogId");

                    b.Property<string>("ResponseSize")
                        .IsRequired();

                    b.Property<string>("StatusCode")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<int>("TimeTaken");

                    b.Property<string>("UriPath")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("LogId");

                    b.ToTable("LogAgora");
                });

            modelBuilder.Entity("Model.LogArquivo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("Arquivo");

                    b.Property<DateTime>("DataHoraInsercao");

                    b.Property<int>("LogId");

                    b.Property<string>("NomeArquivo")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("TipoLog");

                    b.HasKey("Id");

                    b.HasIndex("LogId");

                    b.ToTable("LogArquivo");
                });

            modelBuilder.Entity("Model.LogMinhaCdn", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CacheStatus")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<int>("LogId");

                    b.Property<string>("Request")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("ResponseSize")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("StatusCode")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<string>("TimeTaken")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("LogId");

                    b.ToTable("LogMinhaCdn");
                });

            modelBuilder.Entity("Model.LogAgora", b =>
                {
                    b.HasOne("Model.Log", "Log")
                        .WithMany("LogAgora")
                        .HasForeignKey("LogId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Model.LogArquivo", b =>
                {
                    b.HasOne("Model.Log")
                        .WithMany("LogArquivo")
                        .HasForeignKey("LogId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Model.LogMinhaCdn", b =>
                {
                    b.HasOne("Model.Log", "Log")
                        .WithMany("LogMinhaCdn")
                        .HasForeignKey("LogId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
