﻿// <auto-generated />
using System;
using CJTasksHelperBot.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CJTasksHelperBot.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230704160916_ChangedRelationBetweenUsersAndChats2")]
    partial class ChangedRelationBetweenUsersAndChats2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CJTasksHelperBot.Domain.Entities.Chat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("CJTasksHelperBot.Domain.Entities.Homework", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CompletedAd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Deadline")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Subject")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Task")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Homework");
                });

            modelBuilder.Entity("CJTasksHelperBot.Domain.Entities.Task", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CompletedAd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Deadline")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("CJTasksHelperBot.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("TelegramId")
                        .HasColumnType("bigint");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CJTasksHelperBot.Domain.Entities.UserChat", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<Guid?>("ChatId1")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UserId1")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "ChatId");

                    b.HasIndex("ChatId");

                    b.HasIndex("ChatId1");

                    b.HasIndex("UserId1");

                    b.ToTable("UserChats");
                });

            modelBuilder.Entity("CJTasksHelperBot.Domain.Entities.Homework", b =>
                {
                    b.HasOne("CJTasksHelperBot.Domain.Entities.User", null)
                        .WithMany("Homeworks")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("CJTasksHelperBot.Domain.Entities.Task", b =>
                {
                    b.HasOne("CJTasksHelperBot.Domain.Entities.User", null)
                        .WithMany("Tasks")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("CJTasksHelperBot.Domain.Entities.UserChat", b =>
                {
                    b.HasOne("CJTasksHelperBot.Domain.Entities.Chat", null)
                        .WithMany()
                        .HasForeignKey("ChatId")
                        .HasPrincipalKey("TelegramId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("CJTasksHelperBot.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("ChatId")
                        .HasPrincipalKey("TelegramId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("CJTasksHelperBot.Domain.Entities.Chat", "Chat")
                        .WithMany()
                        .HasForeignKey("ChatId1");

                    b.HasOne("CJTasksHelperBot.Domain.Entities.Chat", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasPrincipalKey("TelegramId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("CJTasksHelperBot.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasPrincipalKey("TelegramId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("CJTasksHelperBot.Domain.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId1");

                    b.Navigation("Chat");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CJTasksHelperBot.Domain.Entities.User", b =>
                {
                    b.Navigation("Homeworks");

                    b.Navigation("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}