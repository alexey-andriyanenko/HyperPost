﻿// <auto-generated />
using System;
using HyperPost.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HyperPost.Migrations
{
    [DbContext(typeof(HyperPostDbContext))]
    [Migration("20230920183006_PackageEFModels")]
    partial class PackageEFModels
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.HasIndex("Number")
                        .IsUnique();

                    b.ToTable("Department");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            FullAddress = "HyperPost Department #1, 5331 Rexford Court, Montgomery AL 36116",
                            Number = 1
                        },
                        new
                        {
                            Id = 2,
                            FullAddress = "HyperPost Department #2, 6095 Terry Lane, Golden CO 80403",
                            Number = 2
                        },
                        new
                        {
                            Id = 3,
                            FullAddress = "HyperPost Department #3, 1002 Hilltop Drive, Dalton GA 30720",
                            Number = 3
                        },
                        new
                        {
                            Id = 4,
                            FullAddress = "HyperPost Department #4, 2325 Eastridge Circle, Moore OK 73160",
                            Number = 4
                        },
                        new
                        {
                            Id = 5,
                            FullAddress = "HyperPost Department #5, 100219141 Pine Ridge Circle, Anchorage AK 99516",
                            Number = 5
                        },
                        new
                        {
                            Id = 6,
                            FullAddress = "HyperPost Department #6, 5275 North 59th Avenue, Glendale AZ 85301",
                            Number = 6
                        },
                        new
                        {
                            Id = 7,
                            FullAddress = "HyperPost Department #7, 5985 Lamar Street, Arvada CO 80003",
                            Number = 7
                        },
                        new
                        {
                            Id = 8,
                            FullAddress = "HyperPost Department #8, 136 Acacia Drive, Blue Lake CA 95525",
                            Number = 8
                        },
                        new
                        {
                            Id = 9,
                            FullAddress = "HyperPost Department #9, 7701 Taylor Oaks Circle, Montgomery AL 36116",
                            Number = 9
                        },
                        new
                        {
                            Id = 10,
                            FullAddress = "HyperPost Department #10, 243 Kentucky Avenue, Pasadena MD 21122",
                            Number = 10
                        });
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

                    b.HasIndex("Name")
                        .IsUnique();

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

                    b.Property<DateTime?>("ArchivedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ArrivedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("DeliveryPrice")
                        .HasColumnType("decimal(8, 2)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("PackagePrice")
                        .HasColumnType("decimal(8, 2)");

                    b.Property<DateTime?>("ReceivedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ReceiverDepartmentId")
                        .HasColumnType("int");

                    b.Property<int>("ReceiverUserId")
                        .HasColumnType("int");

                    b.Property<int?>("SenderDepartmentId")
                        .HasColumnType("int");

                    b.Property<int>("SenderUserId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("SentAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.Property<decimal>("Weight")
                        .HasColumnType("decimal(4, 2)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ReceiverDepartmentId");

                    b.HasIndex("ReceiverUserId");

                    b.HasIndex("SenderDepartmentId");

                    b.HasIndex("SenderUserId");

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
                        },
                        new
                        {
                            Id = 5,
                            Name = "archived"
                        },
                        new
                        {
                            Id = 6,
                            Name = "modified"
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

            modelBuilder.Entity("HyperPost.Models.PackageModel", b =>
                {
                    b.HasOne("HyperPost.Models.PackageCategoryModel", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.HasOne("HyperPost.Models.DepartmentModel", "ReceiverDepartment")
                        .WithMany()
                        .HasForeignKey("ReceiverDepartmentId");

                    b.HasOne("HyperPost.Models.UserModel", "ReceiverUser")
                        .WithMany()
                        .HasForeignKey("ReceiverUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HyperPost.Models.DepartmentModel", "SenderDepartment")
                        .WithMany()
                        .HasForeignKey("SenderDepartmentId");

                    b.HasOne("HyperPost.Models.UserModel", "SenderUser")
                        .WithMany()
                        .HasForeignKey("SenderUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("ReceiverDepartment");

                    b.Navigation("ReceiverUser");

                    b.Navigation("SenderDepartment");

                    b.Navigation("SenderUser");
                });
#pragma warning restore 612, 618
        }
    }
}
