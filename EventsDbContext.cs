using System;
using System.Collections.Generic;
using EventManagementSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.Data.Context;

public partial class EventsDbContext : DbContext
{
    public EventsDbContext()
    {
    }

    public EventsDbContext(DbContextOptions<EventsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventUser> EventUsers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:EventsConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventUser>(entity =>
        {
            entity.Property(e => e.Creation).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.EventRefNavigation).WithMany(p => p.EventUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventUser_Events");

            entity.HasOne(d => d.UserRefNavigation).WithMany(p => p.EventUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventUser_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
