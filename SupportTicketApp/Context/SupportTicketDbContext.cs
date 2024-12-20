using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using SupportTicketApp.ViewModels;
using SupportTicketApp.Models;

namespace SupportTicketApp.Context
{
    public class SupportTicketDbContext : DbContext
    {

        public SupportTicketDbContext(DbContextOptions<SupportTicketDbContext> options) : base(options) { }

        public DbSet<UserTab> UserTabs { get; set; }
        public DbSet<UserLogTab> UserLogTabs { get; set; }
        public DbSet<TicketInfoTab> TicketInfoTabs { get; set; }
        public DbSet<TicketInfoCommentTab> TicketInfoCommentTabs { get; set; }
        public DbSet<TicketImage> TicketImages { get; set; }
        public DbSet<TicketCommentImage> TicketCommentImages { get; set; }
        public DbSet<TicketAssignment> TicketAssignments { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UserTab ile TicketInfoTab arasındaki bire çok ilişki
            modelBuilder.Entity<TicketInfoTab>()
                .HasOne(t => t.UserTab)
                .WithMany(u => u.TicketInfoTabs)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // TicketInfoTab ile TicketImage arasındaki bire çok ilişkiyi tanımlıyoruz
            modelBuilder.Entity<TicketInfoTab>()
                .HasMany(t => t.TicketImages) // TicketInfoTab'ın birden fazla resmi olabilir
                .WithOne(ti => ti.TicketInfoTab) // Ters ilişkiyi kuruyoruz: Her TicketImage, bir TicketInfoTab'a ait olacak
                .HasForeignKey(ti => ti.TicketId) // TicketImage tablosunda TicketId, TicketInfoTab'ın foreign key'i olacaktır.
                .OnDelete(DeleteBehavior.Cascade);


            // TicketInfoTab ile TicketInfoCommentTab arasındaki bire çok ilişki
            modelBuilder.Entity<TicketInfoCommentTab>()
                .HasOne(tc => tc.TicketInfoTab)
                .WithMany(t => t.TicketInfoCommentTabs)
                .HasForeignKey(tc => tc.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserTab ile TicketInfoCommentTab arasındaki bire çok ilişki
            modelBuilder.Entity<TicketInfoCommentTab>()
                .HasOne(tc => tc.UserTab)
                .WithMany(t => t.TicketInfoCommentTabs)
                .HasForeignKey(tc => tc.UserId)
                .OnDelete(DeleteBehavior.NoAction);


            // TicketInfoCommentTab ile TicketCommentImage arasındaki bire çok ilişkiyi tanımlıyoruz
            modelBuilder.Entity<TicketInfoCommentTab>()
                .HasMany(t => t.TicketCommentImages) // TicketInfoCommentTab'ın birden fazla resmi olabilir
                .WithOne(i => i.TicketInfoCommentTab) // TicketCommentImage'in bir TicketInfoCommentTab'ı olacak (ters ilişki)
                .HasForeignKey(i => i.CommentId) // TicketCommentImage tablosunda CommentId, TicketInfoCommentTab'ın foreign key'i olacaktır
                .OnDelete(DeleteBehavior.Cascade);

            // UserTab ile UserLogTab arasındaki bire çok ilişki
            modelBuilder.Entity<UserLogTab>()
                .HasOne(ul => ul.UserTab)
                .WithMany(u => u.UserLogTabs)
                .HasForeignKey(ul => ul.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // TicketAssignment (ara tablo) ile UserTab ve TicketInfoTab arasındaki çoka çok ilişki
            modelBuilder.Entity<TicketAssignment>()
                .HasKey(ta => new { ta.TicketId, ta.UserId });

            modelBuilder.Entity<TicketAssignment>()
                .HasOne(ta => ta.TicketInfoTab)
                .WithMany(t => t.TicketAssignments)
                .HasForeignKey(ta => ta.TicketId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TicketAssignment>()
                .HasOne(ta => ta.UserTab)
                .WithMany(u => u.TicketAssignments)
                .HasForeignKey(ta => ta.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
