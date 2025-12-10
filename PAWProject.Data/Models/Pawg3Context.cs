using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PAWProject.Data.Models;

public partial class Pawg3Context : DbContext
{
    public Pawg3Context()
    {
    }

    public Pawg3Context(DbContextOptions<Pawg3Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Source> Sources { get; set; }

    public virtual DbSet<SourceItem> SourceItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(LocalDb)\\MSSQLLocalDB;Database=PAWG3;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Source>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sources__3214EC0788D4DDF0");

            entity.Property(e => e.ComponentType).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Url).HasMaxLength(500);
        });

        modelBuilder.Entity<SourceItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SourceIt__3214EC076936E668");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");

            entity.HasOne<Source>()
                  .WithMany()
                  .HasForeignKey(e => e.SourceId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
