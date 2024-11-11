﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SupportTicketApp.Models;

#nullable disable

namespace SupportTicketApp.Migrations
{
    [DbContext(typeof(SupportTicketDbContext))]
    partial class SupportTicketDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SupportTicketApp.Models.CommentImageJunction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CommentId")
                        .HasColumnType("int");

                    b.Property<int>("ImageId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CommentId");

                    b.HasIndex("ImageId");

                    b.ToTable("CommentImages");
                });

            modelBuilder.Entity("SupportTicketApp.Models.TicketCommentImage", b =>
                {
                    b.Property<int>("ImageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ImageId"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.HasKey("ImageId");

                    b.ToTable("TicketCommentImages");
                });

            modelBuilder.Entity("SupportTicketApp.Models.TicketImage", b =>
                {
                    b.Property<int>("ImageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ImageId"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.HasKey("ImageId");

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

                    b.Property<int?>("TicketCommentImageId")
                        .HasColumnType("int");

                    b.Property<int?>("TicketCommentImageImageId")
                        .HasColumnType("int");

                    b.Property<int>("TicketId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("CommentId");

                    b.HasIndex("TicketCommentImageId");

                    b.HasIndex("TicketCommentImageImageId")
                        .IsUnique()
                        .HasFilter("[TicketCommentImageImageId] IS NOT NULL");

                    b.HasIndex("TicketId");

                    b.ToTable("TicketInfoCommentTabs");
                });

            modelBuilder.Entity("SupportTicketApp.Models.TicketInfoTab", b =>
                {
                    b.Property<int>("TicketId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TicketId"));

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

                    b.Property<int?>("TicketImageId")
                        .HasColumnType("int");

                    b.Property<int?>("TicketImageImageId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Urgency")
                        .HasMaxLength(20)
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("UserTabUserId")
                        .HasColumnType("int");

                    b.HasKey("TicketId");

                    b.HasIndex("TicketImageId");

                    b.HasIndex("TicketImageImageId")
                        .IsUnique()
                        .HasFilter("[TicketImageImageId] IS NOT NULL");

                    b.HasIndex("UserTabUserId");

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

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("LogId");

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

            modelBuilder.Entity("SupportTicketApp.Models.CommentImageJunction", b =>
                {
                    b.HasOne("SupportTicketApp.Models.TicketInfoCommentTab", "TicketComment")
                        .WithMany("CommentImages")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SupportTicketApp.Models.TicketCommentImage", "TicketCommentImage")
                        .WithMany()
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("TicketComment");

                    b.Navigation("TicketCommentImage");
                });

            modelBuilder.Entity("SupportTicketApp.Models.TicketInfoCommentTab", b =>
                {
                    b.HasOne("SupportTicketApp.Models.TicketCommentImage", "TicketCommentImage")
                        .WithMany()
                        .HasForeignKey("TicketCommentImageId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SupportTicketApp.Models.TicketCommentImage", null)
                        .WithOne("Comment")
                        .HasForeignKey("SupportTicketApp.Models.TicketInfoCommentTab", "TicketCommentImageImageId");

                    b.HasOne("SupportTicketApp.Models.TicketInfoTab", "Ticket")
                        .WithMany("Comments")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ticket");

                    b.Navigation("TicketCommentImage");
                });

            modelBuilder.Entity("SupportTicketApp.Models.TicketInfoTab", b =>
                {
                    b.HasOne("SupportTicketApp.Models.TicketImage", "TicketImage")
                        .WithMany()
                        .HasForeignKey("TicketImageId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SupportTicketApp.Models.TicketImage", null)
                        .WithOne("Ticket")
                        .HasForeignKey("SupportTicketApp.Models.TicketInfoTab", "TicketImageImageId");

                    b.HasOne("SupportTicketApp.Models.UserTab", "UserTab")
                        .WithMany()
                        .HasForeignKey("UserTabUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TicketImage");

                    b.Navigation("UserTab");
                });

            modelBuilder.Entity("SupportTicketApp.Models.TicketCommentImage", b =>
                {
                    b.Navigation("Comment")
                        .IsRequired();
                });

            modelBuilder.Entity("SupportTicketApp.Models.TicketImage", b =>
                {
                    b.Navigation("Ticket")
                        .IsRequired();
                });

            modelBuilder.Entity("SupportTicketApp.Models.TicketInfoCommentTab", b =>
                {
                    b.Navigation("CommentImages");
                });

            modelBuilder.Entity("SupportTicketApp.Models.TicketInfoTab", b =>
                {
                    b.Navigation("Comments");
                });
#pragma warning restore 612, 618
        }
    }
}
