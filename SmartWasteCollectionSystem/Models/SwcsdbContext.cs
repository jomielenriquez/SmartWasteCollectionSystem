using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SmartWasteCollectionSystem.Models;

public partial class SwcsdbContext : DbContext
{
    public SwcsdbContext()
    {
    }

    public SwcsdbContext(DbContextOptions<SwcsdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Announcement> Announcements { get; set; }

    public virtual DbSet<DayOfWeek> DayOfWeeks { get; set; }

    public virtual DbSet<Email> Emails { get; set; }

    public virtual DbSet<FrequencyType> FrequencyTypes { get; set; }

    public virtual DbSet<GarbageCollectionSchedule> GarbageCollectionSchedules { get; set; }

    public virtual DbSet<MonthlyDue> MonthlyDues { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasKey(e => e.AnnouncementId);//.HasName("PK__Announce__9DE44574D64ADA81");

            entity.Property(e => e.AnnouncementId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<DayOfWeek>(entity =>
        {
            entity.HasKey(e => e.DayOfWeekId);//.HasName("PK__DayOfWee__01AA8DDFF2F0D8FA");

            entity.ToTable("DayOfWeek");

            entity.Property(e => e.DayOfWeekId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("DayOfWeekID");
            entity.Property(e => e.Day).HasMaxLength(50);
        });

        modelBuilder.Entity<Email>(entity =>
        {
            entity.HasKey(e => e.EmailId);//.HasName("PK__Emails__7ED91ACF5C6D758F");

            entity.Property(e => e.EmailId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SentDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<FrequencyType>(entity =>
        {
            entity.HasKey(e => e.FrequencyTypeId);//.HasName("PK__Frequenc__829BB4DCD8D30891");

            entity.ToTable("FrequencyType");

            entity.Property(e => e.FrequencyTypeId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("FrequencyTypeID");
            entity.Property(e => e.FrequencyName).HasMaxLength(50);
        });

        modelBuilder.Entity<GarbageCollectionSchedule>(entity =>
        {
            entity.HasKey(e => e.GarbageCollectionScheduleId);//.HasName("PK__GarbageC__CF2C7CEF0240A813");

            entity.ToTable("GarbageCollectionSchedule");

            entity.Property(e => e.GarbageCollectionScheduleId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("GarbageCollectionScheduleID");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DayOfWeekId).HasColumnName("DayOfWeekID");
            entity.Property(e => e.FrequencyTypeId).HasColumnName("FrequencyTypeID");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(d => d.DayOfWeek).WithMany(p => p.GarbageCollectionSchedules)
                .HasForeignKey(d => d.DayOfWeekId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GarbageCollectionSchedule_DayOfWeekId");

            entity.HasOne(d => d.FrequencyType).WithMany(p => p.GarbageCollectionSchedules)
                .HasForeignKey(d => d.FrequencyTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GarbageCollectionSchedule_FrequencyTypeID");
        });

        modelBuilder.Entity<MonthlyDue>(entity =>
        {
            entity.HasKey(e => e.MonthlyDueId);//.HasName("PK__MonthlyD__F025985FB8301311");

            entity.Property(e => e.MonthlyDueId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("MonthlyDueID");
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaidDate).HasColumnType("datetime");
            entity.Property(e => e.StarDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.MonthlyDues)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MonthlyDu__UserI__5535A963");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);//.HasName("PK__Users__1788CCACE57243DF");

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("UserID");
            entity.Property(e => e.BlockNumber).HasMaxLength(50);
            entity.Property(e => e.ContactNumber).HasMaxLength(50);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.HomeOwnerApikey).HasColumnName("HomeOwnerAPIKey");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.LotNumber).HasMaxLength(50);
            entity.Property(e => e.MoveInDate).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.StreetName).HasMaxLength(50);

            entity.HasOne(d => d.UserRole).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_UserRoleID");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId);//.HasName("PK__UserRole__3D978A552FCAB013");

            entity.ToTable("UserRole");

            entity.Property(e => e.UserRoleId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("UserRoleID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
