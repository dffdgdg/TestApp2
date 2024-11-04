using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TestApp.Model;

public partial class TestDbContext : DbContext
{
    public TestDbContext()
    {
    }

    public TestDbContext(DbContextOptions<TestDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<HistoricalSite> HistoricalSites { get; set; }

    public virtual DbSet<HistoricalSitesInfo> HistoricalSitesInfos { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<SiteHistory> SiteHistories { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<TestQuestionsAnswer> TestQuestionsAnswers { get; set; }

    public virtual DbSet<TestSummary> TestSummaries { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserDetail> UserDetails { get; set; }

    public virtual DbSet<Userresult> Userresults { get; set; }

    public virtual DbSet<Usertype> Usertypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=TestDB;Username=postgres;Password=qwaszxedc1");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("answers_pkey");

            entity.ToTable("answers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnswerText).HasColumnName("answer_text");
            entity.Property(e => e.IsCorrect)
                .HasDefaultValue(false)
                .HasColumnName("is_correct");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");

            entity.HasOne(d => d.Question).WithMany(p => p.Answers)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("answers_question_id_fkey");
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("districts_pkey");

            entity.ToTable("districts");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FoundingDate).HasColumnName("founding_date");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<HistoricalSite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("historical_sites_pkey");

            entity.ToTable("historical_sites");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConstructionDate).HasColumnName("construction_date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.District).HasColumnName("district");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timestamp");

            entity.HasOne(d => d.DistrictNavigation).WithMany(p => p.HistoricalSites)
                .HasForeignKey(d => d.District)
                .HasConstraintName("historical_sites_district_fkey");
        });

        modelBuilder.Entity<HistoricalSitesInfo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("historical_sites_info");

            entity.Property(e => e.ConstructionDate).HasColumnName("construction_date");
            entity.Property(e => e.DistrictDescription).HasColumnName("district_description");
            entity.Property(e => e.DistrictName)
                .HasMaxLength(255)
                .HasColumnName("district_name");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.SiteDescription).HasColumnName("site_description");
            entity.Property(e => e.SiteName)
                .HasMaxLength(255)
                .HasColumnName("site_name");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("questions_pkey");

            entity.ToTable("questions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.QuestionNumber).HasColumnName("question_number");
            entity.Property(e => e.QuestionText).HasColumnName("question_text");
            entity.Property(e => e.QuestionType)
                .HasMaxLength(50)
                .HasColumnName("question_type");
            entity.Property(e => e.TestId).HasColumnName("test_id");

            entity.HasOne(d => d.Test).WithMany(p => p.Questions)
                .HasForeignKey(d => d.TestId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("questions_test_id_fkey");
        });

        modelBuilder.Entity<SiteHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("site_history_pkey");

            entity.ToTable("site_history");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EventDate).HasColumnName("event_date");
            entity.Property(e => e.EventDescription).HasColumnName("event_description");
            entity.Property(e => e.Site).HasColumnName("site");

            entity.HasOne(d => d.SiteNavigation).WithMany(p => p.SiteHistories)
                .HasForeignKey(d => d.Site)
                .HasConstraintName("site_history_site_fkey");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tests_pkey");

            entity.ToTable("tests");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.District).HasColumnName("district");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.DistrictNavigation).WithMany(p => p.Tests)
                .HasForeignKey(d => d.District)
                .HasConstraintName("fk_district");
        });

        modelBuilder.Entity<TestQuestionsAnswer>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("test_questions_answers");

            entity.Property(e => e.AnswerId).HasColumnName("answer_id");
            entity.Property(e => e.AnswerText).HasColumnName("answer_text");
            entity.Property(e => e.IsCorrect).HasColumnName("is_correct");
            entity.Property(e => e.QuestionId).HasColumnName("question_id");
            entity.Property(e => e.QuestionNumber).HasColumnName("question_number");
            entity.Property(e => e.QuestionText).HasColumnName("question_text");
            entity.Property(e => e.TestId).HasColumnName("test_id");
            entity.Property(e => e.TestTitle)
                .HasMaxLength(255)
                .HasColumnName("test_title");
        });

        modelBuilder.Entity<TestSummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("test_summary");

            entity.Property(e => e.DistrictName)
                .HasMaxLength(255)
                .HasColumnName("district_name");
            entity.Property(e => e.QuestionCount).HasColumnName("question_count");
            entity.Property(e => e.TestDescription).HasColumnName("test_description");
            entity.Property(e => e.TestId).HasColumnName("test_id");
            entity.Property(e => e.TestTitle)
                .HasMaxLength(255)
                .HasColumnName("test_title");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Login, "users_username_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Birthdate).HasColumnName("birthdate");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.Midname)
                .HasMaxLength(255)
                .HasColumnName("midname");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .HasColumnName("surname");
            entity.Property(e => e.Usertype).HasColumnName("usertype");

            entity.HasOne(d => d.UsertypeNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Usertype)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usertype");
        });

        modelBuilder.Entity<UserDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("user_details");

            entity.Property(e => e.Birthdate).HasColumnName("birthdate");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.Midname)
                .HasMaxLength(255)
                .HasColumnName("midname");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .HasColumnName("surname");
            entity.Property(e => e.UserType)
                .HasMaxLength(50)
                .HasColumnName("user_type");
        });

        modelBuilder.Entity<Userresult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("userresult_pkey");

            entity.ToTable("userresult");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Testid).HasColumnName("testid");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Test).WithMany(p => p.Userresults)
                .HasForeignKey(d => d.Testid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userresult_testid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Userresults)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userresult_userid_fkey");
        });

        modelBuilder.Entity<Usertype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usertype_pkey");

            entity.ToTable("usertype");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
