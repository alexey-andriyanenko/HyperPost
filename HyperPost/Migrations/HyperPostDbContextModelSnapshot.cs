﻿// <auto-generated />
using System;
using HyperPost.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HyperPost.Migrations
{
    [DbContext(typeof(HyperPostDbContext))]
    partial class HyperPostDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HyperPost.Models.DepartmentModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("FullAddress")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Department");
                });

            modelBuilder.Entity("HyperPost.Models.PackageCategoryModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("PackageCategory");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Food"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Money"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Medicaments"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Accumulators"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Sports Products"
                        },
                        new
                        {
                            Id = 6,
                            Name = "Clothes"
                        },
                        new
                        {
                            Id = 7,
                            Name = "Shoes"
                        },
                        new
                        {
                            Id = 8,
                            Name = "Documents"
                        },
                        new
                        {
                            Id = 9,
                            Name = "Books"
                        },
                        new
                        {
                            Id = 10,
                            Name = "Computers"
                        },
                        new
                        {
                            Id = 11,
                            Name = "Accessories"
                        });
                });

            modelBuilder.Entity("HyperPost.Models.PackageModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ArrivedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("DeliveryPrice")
                        .HasColumnType("decimal(8, 2)");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("PackagePrice")
                        .HasColumnType("decimal(8, 2)");

                    b.Property<DateTime?>("ReceivedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("ReceiverDepartmentId")
                        .HasColumnType("int");

                    b.Property<int>("ReceiverUserId")
                        .HasColumnType("int");

                    b.Property<int>("SenderDepartmentId")
                        .HasColumnType("int");

                    b.Property<int>("SenderUserId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("SentAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.Property<decimal>("Weight")
                        .HasColumnType("decimal(2, 2)");

                    b.HasKey("Id");

                    b.ToTable("Package");
                });

            modelBuilder.Entity("HyperPost.Models.PackageStatusModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.ToTable("PackageStatus");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "created"
                        },
                        new
                        {
                            Id = 2,
                            Name = "sent"
                        },
                        new
                        {
                            Id = 3,
                            Name = "arrived"
                        },
                        new
                        {
                            Id = 4,
                            Name = "received"
                        });
                });

            modelBuilder.Entity("HyperPost.Models.UserModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Password")
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasFilter("[Email] IS NOT NULL");

                    b.HasIndex("PhoneNumber")
                        .IsUnique();

                    b.ToTable("User");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "admin@example.com",
                            FirstName = "Admin",
                            LastName = "User",
                            Password = "root",
                            PhoneNumber = "111111",
                            RoleId = 1
                        },
                        new
                        {
                            Id = 2,
                            Email = "manager@example.com",
                            FirstName = "Manager",
                            LastName = "User",
                            Password = "manager_password",
                            PhoneNumber = "222222",
                            RoleId = 2
                        },
                        new
                        {
                            Id = 3,
                            Email = "client@example.com",
                            FirstName = "Client",
                            LastName = "User",
                            Password = "client_password",
                            PhoneNumber = "333333",
                            RoleId = 3
                        });
                });

            modelBuilder.Entity("HyperPost.Models.UserRoleModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.ToTable("UserRole");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "admin"
                        },
                        new
                        {
                            Id = 2,
                            Name = "manager"
                        },
                        new
                        {
                            Id = 3,
                            Name = "client"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
