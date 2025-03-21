﻿// <auto-generated />
using System;
using Calia.Services.OrderAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Calia.Services.OrderAPI.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240924154251_addcanceltime")]
    partial class addcanceltime
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Calia.Services.OrderAPI.Models.AdminNames", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AdminName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AdminNames");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AdminName = "Hasan"
                        },
                        new
                        {
                            Id = 2,
                            AdminName = "Berke"
                        },
                        new
                        {
                            Id = 3,
                            AdminName = "Mehmet"
                        },
                        new
                        {
                            Id = 4,
                            AdminName = "Samet"
                        });
                });

            modelBuilder.Entity("Calia.Services.OrderAPI.Models.KisiselGider", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AdminId")
                        .HasColumnType("int");

                    b.Property<double?>("AlinanPara")
                        .HasColumnType("float");

                    b.Property<DateTime?>("GiderTarihi")
                        .HasColumnType("datetime2");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AdminId");

                    b.ToTable("KisiselGiders");
                });

            modelBuilder.Entity("Calia.Services.OrderAPI.Models.OrderDetails", b =>
                {
                    b.Property<int>("OrderDetailsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderDetailsId"));

                    b.Property<DateTime?>("CancelTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<int>("OrderHeaderId")
                        .HasColumnType("int");

                    b.Property<string>("PaymentStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("isPaid")
                        .HasColumnType("bit");

                    b.HasKey("OrderDetailsId");

                    b.HasIndex("OrderHeaderId");

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("Calia.Services.OrderAPI.Models.OrderExtra", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ExtraName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("OrderDetailsId")
                        .HasColumnType("int");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("OrderDetailsId");

                    b.ToTable("OrderExtras");
                });

            modelBuilder.Entity("Calia.Services.OrderAPI.Models.OrderHeader", b =>
                {
                    b.Property<int>("OrderHeaderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderHeaderId"));

                    b.Property<DateTime?>("CloseTime")
                        .HasColumnType("datetime2");

                    b.Property<double?>("Ikram")
                        .HasColumnType("float");

                    b.Property<double?>("Iskonto")
                        .HasColumnType("float");

                    b.Property<double?>("KrediKarti")
                        .HasColumnType("float");

                    b.Property<string>("MasaNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("Nakit")
                        .HasColumnType("float");

                    b.Property<string>("OrderStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("OrderTime")
                        .HasColumnType("datetime2");

                    b.Property<double>("OrderTotal")
                        .HasColumnType("float");

                    b.Property<string>("PaymentIntentId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StripeSessionId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TableDetailsId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("OrderHeaderId");

                    b.HasIndex("TableDetailsId");

                    b.ToTable("OrderHeaders");
                });

            modelBuilder.Entity("Calia.Services.OrderAPI.Models.TableDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double?>("AlinanFiyat")
                        .HasColumnType("float");

                    b.Property<DateTime?>("CloseTime")
                        .HasColumnType("datetime2");

                    b.Property<double?>("Ikram")
                        .HasColumnType("float");

                    b.Property<double?>("Iskonto")
                        .HasColumnType("float");

                    b.Property<double?>("KrediKarti")
                        .HasColumnType("float");

                    b.Property<double?>("Nakit")
                        .HasColumnType("float");

                    b.Property<DateTime?>("OpenTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("PaymentStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TableId")
                        .HasColumnType("int");

                    b.Property<double?>("TotalTable")
                        .HasColumnType("float");

                    b.Property<bool?>("isClosed")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("TableId");

                    b.ToTable("tableDetails");
                });

            modelBuilder.Entity("Calia.Services.OrderAPI.Models.TableNo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsOccupied")
                        .HasColumnType("bit");

                    b.Property<string>("MasaNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TableNos");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            IsOccupied = false,
                            MasaNo = "1"
                        },
                        new
                        {
                            Id = 2,
                            IsOccupied = false,
                            MasaNo = "2"
                        },
                        new
                        {
                            Id = 3,
                            IsOccupied = false,
                            MasaNo = "3"
                        },
                        new
                        {
                            Id = 4,
                            IsOccupied = false,
                            MasaNo = "4"
                        },
                        new
                        {
                            Id = 5,
                            IsOccupied = false,
                            MasaNo = "5"
                        },
                        new
                        {
                            Id = 6,
                            IsOccupied = false,
                            MasaNo = "6"
                        },
                        new
                        {
                            Id = 7,
                            IsOccupied = false,
                            MasaNo = "7"
                        },
                        new
                        {
                            Id = 8,
                            IsOccupied = false,
                            MasaNo = "8"
                        },
                        new
                        {
                            Id = 9,
                            IsOccupied = false,
                            MasaNo = "9"
                        },
                        new
                        {
                            Id = 10,
                            IsOccupied = false,
                            MasaNo = "10"
                        },
                        new
                        {
                            Id = 11,
                            IsOccupied = false,
                            MasaNo = "11"
                        },
                        new
                        {
                            Id = 12,
                            IsOccupied = false,
                            MasaNo = "12"
                        },
                        new
                        {
                            Id = 13,
                            IsOccupied = false,
                            MasaNo = "13"
                        },
                        new
                        {
                            Id = 14,
                            IsOccupied = false,
                            MasaNo = "14"
                        },
                        new
                        {
                            Id = 15,
                            IsOccupied = false,
                            MasaNo = "15"
                        },
                        new
                        {
                            Id = 16,
                            IsOccupied = false,
                            MasaNo = "16"
                        },
                        new
                        {
                            Id = 17,
                            IsOccupied = false,
                            MasaNo = "17"
                        },
                        new
                        {
                            Id = 18,
                            IsOccupied = false,
                            MasaNo = "18"
                        },
                        new
                        {
                            Id = 19,
                            IsOccupied = false,
                            MasaNo = "19"
                        },
                        new
                        {
                            Id = 20,
                            IsOccupied = false,
                            MasaNo = "20"
                        },
                        new
                        {
                            Id = 21,
                            IsOccupied = false,
                            MasaNo = "21"
                        },
                        new
                        {
                            Id = 22,
                            IsOccupied = false,
                            MasaNo = "22"
                        },
                        new
                        {
                            Id = 23,
                            IsOccupied = false,
                            MasaNo = "23"
                        },
                        new
                        {
                            Id = 24,
                            IsOccupied = false,
                            MasaNo = "24"
                        },
                        new
                        {
                            Id = 25,
                            IsOccupied = false,
                            MasaNo = "25"
                        },
                        new
                        {
                            Id = 26,
                            IsOccupied = false,
                            MasaNo = "26"
                        },
                        new
                        {
                            Id = 27,
                            IsOccupied = false,
                            MasaNo = "27"
                        },
                        new
                        {
                            Id = 28,
                            IsOccupied = false,
                            MasaNo = "28"
                        },
                        new
                        {
                            Id = 29,
                            IsOccupied = false,
                            MasaNo = "29"
                        },
                        new
                        {
                            Id = 30,
                            IsOccupied = false,
                            MasaNo = "30"
                        },
                        new
                        {
                            Id = 31,
                            IsOccupied = false,
                            MasaNo = "31"
                        });
                });

            modelBuilder.Entity("Calia.Services.OrderAPI.Models.Veriler", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AylikKrediKarti")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AylikNakit")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HaftalikKrediKarti")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HaftalikNakit")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("SonGuncellemeTarihi")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("SonGuncellemeTarihiYil")
                        .HasColumnType("datetime2");

                    b.Property<int>("ToplamKapananMasaSayisi")
                        .HasColumnType("int");

                    b.Property<int>("ToplamKazanç")
                        .HasColumnType("int");

                    b.Property<int>("ToplamSiparisSayisi")
                        .HasColumnType("int");

                    b.Property<int>("TotalProductNumber")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Veriler");
                });

            modelBuilder.Entity("Calia.Services.OrderAPI.Models.KisiselGider", b =>
                {
                    b.HasOne("Calia.Services.OrderAPI.Models.AdminNames", "AdminName")
                        .WithMany()
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AdminName");
                });

            modelBuilder.Entity("Calia.Services.OrderAPI.Models.OrderDetails", b =>
                {
                    b.HasOne("Calia.Services.OrderAPI.Models.OrderHeader", "OrderHeader")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderHeaderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OrderHeader");
                });

            modelBuilder.Entity("Calia.Services.OrderAPI.Models.OrderExtra", b =>
                {
                    b.HasOne("Calia.Services.OrderAPI.Models.OrderDetails", null)
                        .WithMany("ProductExtrasList")
                        .HasForeignKey("OrderDetailsId");
                });

            modelBuilder.Entity("Calia.Services.OrderAPI.Models.OrderHeader", b =>
                {
                    b.HasOne("Calia.Services.OrderAPI.Models.TableDetails", null)
                        .WithMany("OrderHeaders")
                        .HasForeignKey("TableDetailsId");
                });

            modelBuilder.Entity("Calia.Services.OrderAPI.Models.TableDetails", b =>
                {
                    b.HasOne("Calia.Services.OrderAPI.Models.TableNo", "tableNo")
                        .WithMany()
                        .HasForeignKey("TableId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("tableNo");
                });

            modelBuilder.Entity("Calia.Services.OrderAPI.Models.OrderDetails", b =>
                {
                    b.Navigation("ProductExtrasList");
                });

            modelBuilder.Entity("Calia.Services.OrderAPI.Models.OrderHeader", b =>
                {
                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("Calia.Services.OrderAPI.Models.TableDetails", b =>
                {
                    b.Navigation("OrderHeaders");
                });
#pragma warning restore 612, 618
        }
    }
}
