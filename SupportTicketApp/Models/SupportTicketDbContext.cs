using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using SupportTicketApp.ViewModels;
namespace SupportTicketApp.Models
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
        public DbSet<CommentImageJunction> CommentImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TicketInfoTab ile TicketImage ilişkisi
            modelBuilder.Entity<TicketInfoTab>()
                .HasOne(t => t.TicketImage)
                .WithMany()
                .HasForeignKey(t => t.TicketImageId)
                .OnDelete(DeleteBehavior.Restrict); // İlişki silindiğinde TicketImage silinmesin

            // TicketInfoCommentTab ile TicketInfoTab ilişkisi
            modelBuilder.Entity<TicketInfoCommentTab>()
                .HasOne(c => c.Ticket)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade); // Ticket silindiğinde ilgili yorumlar silinsin

            // TicketInfoCommentTab ile TicketCommentImage ilişkisi
            modelBuilder.Entity<TicketInfoCommentTab>()
                .HasOne(c => c.TicketCommentImage)
                .WithMany()
                .HasForeignKey(c => c.TicketCommentImageId)
                .OnDelete(DeleteBehavior.Restrict); // İlişki silindiğinde TicketCommentImage silinmesin

            // CommentImageJunction ile TicketInfoCommentTab ilişkisi
            modelBuilder.Entity<CommentImageJunction>()
                .HasOne(j => j.TicketComment)
                .WithMany(c => c.CommentImages)
                .HasForeignKey(j => j.CommentId)
                .OnDelete(DeleteBehavior.Cascade); // Yorum silindiğinde ilgili resimler silinsin

            // CommentImageJunction ile TicketCommentImage ilişkisi
            modelBuilder.Entity<CommentImageJunction>()
                .HasOne(j => j.TicketCommentImage)
                .WithMany()
                .HasForeignKey(j => j.ImageId)
                .OnDelete(DeleteBehavior.Restrict); // İlişki silindiğinde TicketCommentImage silinmesin


        }
    }
}
