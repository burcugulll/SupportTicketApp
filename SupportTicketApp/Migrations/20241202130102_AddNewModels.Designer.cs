﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SupportTicketApp.Models;

#nullable disable

namespace SupportTicketApp.Migrations
{
    [DbContext(typeof(SupportTicketDbContext))]
    [Migration("20241202130102_AddNewModels")]
    partial class AddNewModels
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SupportTicketApp.Models.TicketAssignment", b =>
                {
                    b.Property<int>("TicketId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("AssignedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("bit");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.HasKey("TicketId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("TicketAssignments");
                });

            modelBuilder.Entity("SupportTicketApp.Models.TicketCommentImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CommentId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("TicketInfoTabTicketId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CommentId");

                    b.HasIndex("TicketInfoTabTicketId");

                    b.ToTable("TicketCommentImages");
                });

            modelBuilder.Entity("SupportTicketApp.Models.TicketImage", b =>
                {
                    b.Property<int>("TicketImageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TicketImageId"));

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TicketId")
                        .HasColumnType("int");

                    b.HasKey("TicketImageId");

                    b.HasIndex("TicketId");

                    b.ToTable("TicketImages");
                });

            modelBuilder.Entity("SupportTicketApp.Models.TicketInfoCommentTab", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CommentId"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<int>("TicketId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("CommentId");

                    b.HasIndex("TicketId");

                    b.ToTable("TicketInfoCommentTabs");
                });

            modelBuilder.Entity("SupportTicketApp.Models.TicketInfoTab", b =>
                {
                    b.Property<int>("TicketId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TicketId"));

                    b.Property<int>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<int?>("TicketImageId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Urgency")
                        .HasColumnType("int");

                    b.HasKey("TicketId");

                    b.HasIndex("CreatedByUserId");

                    b.ToTable("TicketInfoTabs");
                });

            modelBuilder.Entity("SupportTicketApp.Models.UserLogTab", b =>
                {
                    b.Property<int>("LogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LogId"));

                    b.Property<string>("IPAdress")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Log")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LogTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("LogId");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogTabs");
                });

            modelBuilder.Entity("SupportTicketApp.Models.UserTab", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("LockoutEndTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("LoginAttempts")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<byte[]>("ProfilePhoto")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("UserType")
                        .HasColumnType("int");

                    b.HasKey("UserId");

                    b.ToTable("UserTabs");
                });

            modelBuilder.Entity("SupportTicketApp.Models.TicketAssignment", b =>
                {
                    b.HasOne("SupportTicketApp.Models.TicketInfoTab", "TicketInfoTab")
                        .WithMany("TicketAssignments")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SupportTicketApp.Models.UserTab", "UserTab")
                        .WithMany("TicketAssignments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TicketInfoTab");

                    b.Navigation("UserTab");
                });

            modelBuilder.Entity("SupportTicketApp.Models.TicketCommentImage", b =>
                {
                    b.HasOne("SupportTicketApp.Models.TicketInfoCommentTab", "TicketInfoCommentTab")
                        .WithMany("TicketCommentImages")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SupportTicketApp.Models.TicketInfoTab", null)
                        .WithMany("TicketCommentImages")
                        .HasForeignKey("TicketInfoTabTicketId");

                    b.Navigation("TicketInfoCommentTab");
                });

            modelBuilder.Entity("SupportTicketApp.Models.TicketImage", b =>
                {
                    b.HasOne("SupportTicketApp.Models.TicketInfoTab", "TicketInfoTab")
                        .WithMany("TicketImages")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TicketInfoTab");
                });

            modelBuilder.Entity("SupportTicketApp.Models.TicketInfoCommentTab", b =>
                {
                    b.HasOne("SupportTicketApp.Models.TicketInfoTab", "TicketInfoTab")
                        .WithMany("TicketInfoCommentTabs")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TicketInfoTab");
                });

            modelBuilder.Entity("SupportTicketApp.Models.TicketInfoTab", b =>
                {
                    b.HasOne("SupportTicketApp.Models.UserTab", "CreatedByUser")
                        .WithMany("TicketInfoTabs")
                        .HasForeignKey("CreatedByUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedByUser");
                });

            modelBuilder.Entity("SupportTicketApp.Models.UserLogTab", b =>
                {
                    b.HasOne("SupportTicketApp.Models.UserTab", "UserTab")
                        .WithMany("UserLogTabs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserTab");
                });

            modelBuilder.Entity("SupportTicketApp.Models.TicketInfoCommentTab", b =>
                {
                    b.Navigation("TicketCommentImages");
                });

            modelBuilder.Entity("SupportTicketApp.Models.TicketInfoTab", b =>
                {
                    b.Navigation("TicketAssignments");

                    b.Navigation("TicketCommentImages");

                    b.Navigation("TicketImages");

                    b.Navigation("TicketInfoCommentTabs");
                });

            modelBuilder.Entity("SupportTicketApp.Models.UserTab", b =>
                {
                    b.Navigation("TicketAssignments");

                    b.Navigation("TicketInfoTabs");

                    b.Navigation("UserLogTabs");
                });
#pragma warning restore 612, 618
        }
    }
}
