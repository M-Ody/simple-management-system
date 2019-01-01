﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SysGuiApi;

namespace SysGuiApi.Migrations
{
    [DbContext(typeof(BrillDbContext))]
    [Migration("20181110170659_AddCityAndUpdateClient")]
    partial class AddCityAndUpdateClient
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.3-rtm-32065")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("SysGuiApi.Models.City", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.ToTable("City");
                });

            modelBuilder.Entity("SysGuiApi.Models.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .HasMaxLength(100);

                    b.Property<int?>("CityId");

                    b.Property<string>("Cpf")
                        .HasMaxLength(15);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("Phone")
                        .HasMaxLength(20);

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.HasIndex("UserId");

                    b.ToTable("Client");
                });

            modelBuilder.Entity("SysGuiApi.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ClientId");

                    b.Property<bool>("CloseningPaid");

                    b.Property<double>("CloseningPayment");

                    b.Property<double>("CloseningPaymentFiscal");

                    b.Property<DateTime>("DateCreation");

                    b.Property<DateTime>("DateFirstInstallmentPayment");

                    b.Property<string>("Description")
                        .HasMaxLength(150);

                    b.Property<int>("Factory");

                    b.Property<double>("FirstPayment");

                    b.Property<double>("FirstPaymentFiscal");

                    b.Property<bool>("FirstPaymentPaid");

                    b.Property<int>("InstallmentPaymentDay");

                    b.Property<double>("InstallmentValue");

                    b.Property<double>("InstallmentValueFiscal");

                    b.Property<int>("NumberInstallments");

                    b.Property<int>("NumberInstallmentsPaid");

                    b.Property<int>("PaymentMode");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("UserId");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("SysGuiApi.Models.PermissionGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<int[]>("Permissions");

                    b.HasKey("Id");

                    b.ToTable("PermissionGroup");
                });

            modelBuilder.Entity("SysGuiApi.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("PermissionGroupId");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("PermissionGroupId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SysGuiApi.Models.Client", b =>
                {
                    b.HasOne("SysGuiApi.Models.City", "City")
                        .WithMany()
                        .HasForeignKey("CityId");

                    b.HasOne("SysGuiApi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SysGuiApi.Models.Order", b =>
                {
                    b.HasOne("SysGuiApi.Models.Client", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SysGuiApi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SysGuiApi.Models.User", b =>
                {
                    b.HasOne("SysGuiApi.Models.PermissionGroup", "PermissionGroup")
                        .WithMany()
                        .HasForeignKey("PermissionGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
