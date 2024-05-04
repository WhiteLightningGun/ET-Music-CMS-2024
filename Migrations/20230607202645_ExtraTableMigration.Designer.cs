﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UpLoader_For_ET;

#nullable disable

namespace UpLoader_For_ET.Migrations
{
    [DbContext(typeof(UploadsDBContext))]
    [Migration("20230607202645_ExtraTableMigration")]
    partial class ExtraTableMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("UpLoader_For_ET.DBModels.FrontPageEntry", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("htmlEmbedLink")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("id");

                    b.ToTable("FrontPageEntries");
                });

            modelBuilder.Entity("UpLoader_For_ET.DBModels.UploadDBEntry", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("fileHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("userDescription")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("userFileTitle")
                        .IsRequired()
                        .HasMaxLength(28)
                        .HasColumnType("TEXT");

                    b.Property<string>("userHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("id");

                    b.ToTable("UploadDBEntries");
                });
#pragma warning restore 612, 618
        }
    }
}