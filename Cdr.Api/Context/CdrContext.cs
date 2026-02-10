using Cdr.Api.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cdr.Api.Context;

public class CdrContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public CdrContext(DbContextOptions<CdrContext> options) : base(options)
    {
    }

    // Email Reporting Entities (Feature: 002-automated-cdr-email-reports)
    public DbSet<EmailDeliveryAudit> EmailDeliveryAudits { get; set; }
    public DbSet<HolidayCalendar> HolidayCalendars { get; set; }
    public DbSet<ReportExecutionLog> ReportExecutionLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        // Email Reporting Entity Configurations
        modelBuilder.Entity<EmailDeliveryAudit>(entity =>
        {
            entity.ToTable("EmailDeliveryAudits");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ReportExecutionId);
            entity.HasIndex(e => e.RecipientEmail);
            entity.HasIndex(e => e.DeliveryStatus);
            entity.HasIndex(e => e.CreatedAt);
        });

        modelBuilder.Entity<HolidayCalendar>(entity =>
        {
            entity.ToTable("HolidayCalendars");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.HolidayDate);
            entity.HasIndex(e => new { e.Year, e.HolidayDate });
            entity.HasIndex(e => e.IsActive);
        });

        modelBuilder.Entity<ReportExecutionLog>(entity =>
        {
            entity.ToTable("ReportExecutionLogs");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.HangfireJobId);
            entity.HasIndex(e => e.ReportType);
            entity.HasIndex(e => e.ExecutionStatus);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasMany(e => e.EmailDeliveries)
                  .WithOne(e => e.ReportExecution)
                  .HasForeignKey(e => e.ReportExecutionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}