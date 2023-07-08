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
    [Migration("20230708141754_AddRelationshipToTaskAndHomeworkFromUserChat")]
    partial class AddRelationshipToTaskAndHomeworkFromUserChat
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
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

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

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.Property<string>("Subject")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Task")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("UserChatChatId")
                        .HasColumnType("bigint");

                    b.Property<long?>("UserChatUserId")
                        .HasColumnType("bigint");

                    b.Property<long?>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("StatusId");

                    b.HasIndex("UserId");

                    b.HasIndex("UserChatUserId", "UserChatChatId");

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

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("UserChatChatId")
                        .HasColumnType("bigint");

                    b.Property<long?>("UserChatUserId")
                        .HasColumnType("bigint");

                    b.Property<long?>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("StatusId");

                    b.HasIndex("UserId");

                    b.HasIndex("UserChatUserId", "UserChatChatId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("CJTasksHelperBot.Domain.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

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

                    b.HasKey("UserId", "ChatId");

                    b.HasIndex("ChatId");

                    b.ToTable("UserChats");
                });

            modelBuilder.Entity("CJTasksHelperBot.Domain.Enums.TaskStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TaskStatus");
                });

            modelBuilder.Entity("CJTasksHelperBot.Domain.Entities.Homework", b =>
                {
                    b.HasOne("CJTasksHelperBot.Domain.Enums.TaskStatus", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CJTasksHelperBot.Domain.Entities.User", null)
                        .WithMany("Homeworks")
                        .HasForeignKey("UserId");

                    b.HasOne("CJTasksHelperBot.Domain.Entities.UserChat", "UserChat")
                        .WithMany()
                        .HasForeignKey("UserChatUserId", "UserChatChatId");

                    b.Navigation("Status");

                    b.Navigation("UserChat");
                });

            modelBuilder.Entity("CJTasksHelperBot.Domain.Entities.Task", b =>
                {
                    b.HasOne("CJTasksHelperBot.Domain.Enums.TaskStatus", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CJTasksHelperBot.Domain.Entities.User", null)
                        .WithMany("Tasks")
                        .HasForeignKey("UserId");

                    b.HasOne("CJTasksHelperBot.Domain.Entities.UserChat", "UserChat")
                        .WithMany()
                        .HasForeignKey("UserChatUserId", "UserChatChatId");

                    b.Navigation("Status");

                    b.Navigation("UserChat");
                });

            modelBuilder.Entity("CJTasksHelperBot.Domain.Entities.UserChat", b =>
                {
                    b.HasOne("CJTasksHelperBot.Domain.Entities.Chat", "Chat")
                        .WithMany()
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CJTasksHelperBot.Domain.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

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