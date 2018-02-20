using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DriverLicense.Models.Models
{
    public partial class DriverLicenseContext : DbContext
    {
        public virtual DbSet<Answers> Answers { get; set; }
        public virtual DbSet<Locations> Locations { get; set; }
        public virtual DbSet<Questions> Questions { get; set; }


        public DriverLicenseContext(DbContextOptions<DriverLicenseContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Answers>(entity =>
            {
                entity.HasKey(e => e.AnswerId);

                entity.Property(e => e.Description).IsRequired();

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Answers_Questions");
            });

            modelBuilder.Entity<Locations>(entity =>
            {
                entity.HasKey(e => e.LocationId);

                entity.Property(e => e.ImageUrl).IsRequired();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Questions>(entity =>
            {
                entity.HasKey(e => e.QuestionId);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Questions_Cities");
            });
        }
    }
}
