﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NZ_Walk.Server.Data;

#nullable disable

namespace NZ_Walk2.Migrations
{
    [DbContext(typeof(NzWalksDbContext))]
    [Migration("20250413143654_Adding Image Table")]
    partial class AddingImageTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("NZ_Walk.Server.Models.Domain.Difficulty", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Difficulties");

                    b.HasData(
                        new
                        {
                            Id = new Guid("70f5a843-15c3-42d9-ac99-a4343b56ca92"),
                            Name = "Easy"
                        },
                        new
                        {
                            Id = new Guid("0c8bac97-5527-4b0c-8e8a-e88b06bbc409"),
                            Name = "Medium"
                        },
                        new
                        {
                            Id = new Guid("31996f53-0d32-4892-b51c-622f6b7948f8"),
                            Name = "Hard"
                        });
                });

            modelBuilder.Entity("NZ_Walk.Server.Models.Domain.Region", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegionImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Regions");

                    b.HasData(
                        new
                        {
                            Id = new Guid("d4ebc14b-5c40-41da-ae8f-9e8d666f99f3"),
                            Code = "AKL",
                            Name = "Auckland",
                            RegionImageUrl = "https://images.pexels.com/photos/31439461/pexels-photo-31439461.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2"
                        },
                        new
                        {
                            Id = new Guid("c068d38a-2d3a-43a4-99ec-25303f74acf1"),
                            Code = "NTL",
                            Name = "Northland"
                        },
                        new
                        {
                            Id = new Guid("f9a601d4-3fd5-4d6e-9b74-2209aaccb352"),
                            Code = "BOP",
                            Name = "Bay Of Plenty"
                        },
                        new
                        {
                            Id = new Guid("978ec598-fac3-4f8e-98d3-0282a20369f8"),
                            Code = "WGN",
                            Name = "Wellington",
                            RegionImageUrl = "https://images.pexels.com/photos/3538656/pexels-photo-3538656.jpeg?auto=compress&cs=tinysrgb&w=600"
                        },
                        new
                        {
                            Id = new Guid("c054fafb-b830-4d77-a46d-9abcbe4f2997"),
                            Code = "NSN",
                            Name = "Nelson",
                            RegionImageUrl = "https://images.pexels.com/photos/11130685/pexels-photo-11130685.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2"
                        },
                        new
                        {
                            Id = new Guid("5d89cc85-e9b3-4388-8598-71a25e659440"),
                            Code = "STL",
                            Name = "Southland"
                        });
                });

            modelBuilder.Entity("NZ_Walk.Server.Models.Domain.Walk", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("DifficultyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("LengthInKm")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RegionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("WalkImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DifficultyId");

                    b.HasIndex("RegionId");

                    b.ToTable("Walks");
                });

            modelBuilder.Entity("NZ_Walk2.Models.Domain.Image", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FileDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileExtension")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("FileSizeInBytes")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("NZ_Walk.Server.Models.Domain.Walk", b =>
                {
                    b.HasOne("NZ_Walk.Server.Models.Domain.Difficulty", "Difficulty")
                        .WithMany()
                        .HasForeignKey("DifficultyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("NZ_Walk.Server.Models.Domain.Region", "Region")
                        .WithMany()
                        .HasForeignKey("RegionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Difficulty");

                    b.Navigation("Region");
                });
#pragma warning restore 612, 618
        }
    }
}
