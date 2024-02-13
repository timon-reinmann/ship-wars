﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Yoo.Trainees.ShipWars.DataBase;

#nullable disable

namespace Yoo.Trainees.ShipWars.DataBase.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240115130813_AddStartShotHumanLogic")]
    partial class AddStartShotHumanLogic
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Yoo.Trainees.ShipWars.DataBase.Entities.Game", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("smalldatetime");

                    b.Property<int>("GameMode")
                        .HasColumnType("int");

                    b.Property<string>("GameStatus")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<bool>("IsBotGame")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid?>("NextPlayer")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Game");
                });

            modelBuilder.Entity("Yoo.Trainees.ShipWars.DataBase.Entities.GamePlayer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PlayerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("ScissorsRockPaperBet")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("PlayerId");

                    b.ToTable("GamePlayer");
                });

            modelBuilder.Entity("Yoo.Trainees.ShipWars.DataBase.Entities.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime");

                    b.Property<Guid>("GamePlayersId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GamePlayersId");

                    b.ToTable("Message");
                });

            modelBuilder.Entity("Yoo.Trainees.ShipWars.DataBase.Entities.Player", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Player");
                });

            modelBuilder.Entity("Yoo.Trainees.ShipWars.DataBase.Entities.Ship", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Length")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Ship");

                    b.HasData(
                        new
                        {
                            Id = new Guid("45652d4e-10be-4b26-a8a8-c6de9a4e49af"),
                            Length = 1,
                            Name = "Submarine"
                        },
                        new
                        {
                            Id = new Guid("a735b8ec-c868-45fa-81bc-942eda165a8f"),
                            Length = 2,
                            Name = "Destroyer"
                        },
                        new
                        {
                            Id = new Guid("985497b3-353c-49d3-97a0-79232a270da0"),
                            Length = 3,
                            Name = "Cruiser"
                        },
                        new
                        {
                            Id = new Guid("eee6167b-6588-42d9-b657-7698a2f5ca40"),
                            Length = 4,
                            Name = "Warship"
                        });
                });

            modelBuilder.Entity("Yoo.Trainees.ShipWars.DataBase.Entities.ShipPosition", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Direction")
                        .HasColumnType("int");

                    b.Property<Guid>("GamePlayerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsHuman")
                        .HasColumnType("bit");

                    b.Property<int>("Life")
                        .HasColumnType("int");

                    b.Property<Guid>("ShipId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("X")
                        .HasColumnType("int");

                    b.Property<int>("Y")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GamePlayerId");

                    b.HasIndex("ShipId");

                    b.ToTable("ShipPosition");
                });

            modelBuilder.Entity("Yoo.Trainees.ShipWars.DataBase.Entities.Shot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PlayerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("X")
                        .HasColumnType("int");

                    b.Property<int>("Y")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.ToTable("Shot");
                });

            modelBuilder.Entity("Yoo.Trainees.ShipWars.DataBase.Entities.StartShotHumanLogic", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Hit")
                        .HasColumnType("bit");

                    b.Property<int>("X")
                        .HasColumnType("int");

                    b.Property<int>("Y")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("StartShotHumanLogic");
                });

            modelBuilder.Entity("Yoo.Trainees.ShipWars.DataBase.Entities.GamePlayer", b =>
                {
                    b.HasOne("Yoo.Trainees.ShipWars.DataBase.Entities.Game", "Game")
                        .WithMany("GamePlayers")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Yoo.Trainees.ShipWars.DataBase.Entities.Player", "Player")
                        .WithMany("GamePlayers")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Yoo.Trainees.ShipWars.DataBase.Entities.Message", b =>
                {
                    b.HasOne("Yoo.Trainees.ShipWars.DataBase.Entities.GamePlayer", "GamePlayers")
                        .WithMany()
                        .HasForeignKey("GamePlayersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GamePlayers");
                });

            modelBuilder.Entity("Yoo.Trainees.ShipWars.DataBase.Entities.ShipPosition", b =>
                {
                    b.HasOne("Yoo.Trainees.ShipWars.DataBase.Entities.GamePlayer", "GamePlayer")
                        .WithMany()
                        .HasForeignKey("GamePlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Yoo.Trainees.ShipWars.DataBase.Entities.Ship", "Ship")
                        .WithMany("Positions")
                        .HasForeignKey("ShipId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GamePlayer");

                    b.Navigation("Ship");
                });

            modelBuilder.Entity("Yoo.Trainees.ShipWars.DataBase.Entities.Shot", b =>
                {
                    b.HasOne("Yoo.Trainees.ShipWars.DataBase.Entities.GamePlayer", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Yoo.Trainees.ShipWars.DataBase.Entities.Game", b =>
                {
                    b.Navigation("GamePlayers");
                });

            modelBuilder.Entity("Yoo.Trainees.ShipWars.DataBase.Entities.Player", b =>
                {
                    b.Navigation("GamePlayers");
                });

            modelBuilder.Entity("Yoo.Trainees.ShipWars.DataBase.Entities.Ship", b =>
                {
                    b.Navigation("Positions");
                });
#pragma warning restore 612, 618
        }
    }
}
