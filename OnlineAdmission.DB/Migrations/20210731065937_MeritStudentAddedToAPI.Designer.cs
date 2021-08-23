﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OnlineAdmission.DB;

namespace OnlineAdmission.DB.Migrations
{
    [DbContext(typeof(OnlineAdmissionDbContext))]
    [Migration("20210731065937_MeritStudentAddedToAPI")]
    partial class MeritStudentAddedToAPI
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.8")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("OnlineAdmission.Entity.MeritStudent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ApplicantName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Comments")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HSCGroup")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HSCRoll")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MeritPosition")
                        .HasColumnType("int");

                    b.Property<int>("NUAdmissionRoll")
                        .HasColumnType("int");

                    b.Property<double>("PaidAmaount")
                        .HasColumnType("float");

                    b.Property<bool>("PaymentStatus")
                        .HasColumnType("bit");

                    b.Property<int>("SubjectCode")
                        .HasColumnType("int");

                    b.Property<string>("SubjectName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("MeritStudents");
                });
#pragma warning restore 612, 618
        }
    }
}
