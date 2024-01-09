﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProductManager.Data;

#nullable disable

namespace ProductManager.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    partial class ApplicationDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ProductManager.Models.Company", b =>
                {
                    b.Property<int>("CompanyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CompanyId"));

                    b.Property<string>("CompanyEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("CompanyPhoneNumber")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("CompanyId");

                    b.ToTable("Company", (string)null);
                });

            modelBuilder.Entity("ProductManager.Models.Licence", b =>
                {
                    b.Property<int>("LicenceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LicenceId"));

                    b.Property<decimal>("Cost")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("ValidityMonths")
                        .HasColumnType("int");

                    b.HasKey("LicenceId");

                    b.ToTable("Licence", (string)null);
                });

            modelBuilder.Entity("ProductManager.Models.LicencePurchase", b =>
                {
                    b.Property<int>("PurchaseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PurchaseId"));

                    b.Property<int>("CompanyId")
                        .HasColumnType("int");

                    b.Property<int>("LicenceId")
                        .HasColumnType("int");

                    b.Property<DateTime>("PurchaseDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalCost")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("PurchaseId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("LicenceId");

                    b.ToTable("LicencePurchase", (string)null);
                });

            modelBuilder.Entity("ProductManager.Models.Project", b =>
                {
                    b.Property<int>("ProjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProjectId"));

                    b.Property<int>("CompanyId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProjectId");

                    b.HasIndex("CompanyId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("ProductManager.Models.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoleId"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RoleId");

                    b.ToTable("Role", (string)null);
                });

            modelBuilder.Entity("ProductManager.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<int>("CompanyId")
                        .HasColumnType("int");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.HasIndex("CompanyId");

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("ProductManager.Models.UserProjectAssignment", b =>
                {
                    b.Property<int>("AssignmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AssignmentId"));

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("AssignmentId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UserId");

                    b.ToTable("UserProjectsAssignments");
                });

            modelBuilder.Entity("ProductManager.Models.UserProjectRole", b =>
                {
                    b.Property<int>("ProjectRoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProjectRoleId"));

                    b.Property<int>("AssignmentId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("ProjectRoleId");

                    b.HasIndex("AssignmentId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserProjectRole", (string)null);
                });

            modelBuilder.Entity("ProductManager.Models.UserRole", b =>
                {
                    b.Property<int>("UserRoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserRoleId"));

                    b.Property<int>("RoleId")
                        .HasMaxLength(50)
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("UserRoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRole", (string)null);
                });

            modelBuilder.Entity("ProductManager.Models.LicencePurchase", b =>
                {
                    b.HasOne("ProductManager.Models.Company", "Company")
                        .WithMany("LicencePurchases")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ProductManager.Models.Licence", "Licence")
                        .WithMany("LicencePurchases")
                        .HasForeignKey("LicenceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");

                    b.Navigation("Licence");
                });

            modelBuilder.Entity("ProductManager.Models.Project", b =>
                {
                    b.HasOne("ProductManager.Models.Company", "Company")
                        .WithMany("Projects")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");
                });

            modelBuilder.Entity("ProductManager.Models.User", b =>
                {
                    b.HasOne("ProductManager.Models.Company", "Company")
                        .WithMany("Users")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");
                });

            modelBuilder.Entity("ProductManager.Models.UserProjectAssignment", b =>
                {
                    b.HasOne("ProductManager.Models.Project", "Project")
                        .WithMany("UserProjectAssignments")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ProductManager.Models.User", "User")
                        .WithMany("UserProjectAssignments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ProductManager.Models.UserProjectRole", b =>
                {
                    b.HasOne("ProductManager.Models.UserProjectAssignment", "UserProjectAssignment")
                        .WithMany("UserProjectRoles")
                        .HasForeignKey("AssignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ProductManager.Models.Role", "Role")
                        .WithMany("UserProjectRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("UserProjectAssignment");
                });

            modelBuilder.Entity("ProductManager.Models.UserRole", b =>
                {
                    b.HasOne("ProductManager.Models.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ProductManager.Models.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ProductManager.Models.Company", b =>
                {
                    b.Navigation("LicencePurchases");

                    b.Navigation("Projects");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("ProductManager.Models.Licence", b =>
                {
                    b.Navigation("LicencePurchases");
                });

            modelBuilder.Entity("ProductManager.Models.Project", b =>
                {
                    b.Navigation("UserProjectAssignments");
                });

            modelBuilder.Entity("ProductManager.Models.Role", b =>
                {
                    b.Navigation("UserProjectRoles");

                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("ProductManager.Models.User", b =>
                {
                    b.Navigation("UserProjectAssignments");

                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("ProductManager.Models.UserProjectAssignment", b =>
                {
                    b.Navigation("UserProjectRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
