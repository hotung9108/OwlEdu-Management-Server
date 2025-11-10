using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OwlEdu_Manager_Server.Models;

public partial class EnglishCenterManagementContext : DbContext
{
    public EnglishCenterManagementContext()
    {
    }

    public EnglishCenterManagementContext(DbContextOptions<EnglishCenterManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<ClassAssignment> ClassAssignments { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Score> Scores { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__account__3213E83FACB48C97");

            entity.ToTable("account");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("id");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("avatar");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("update_at");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.ScheduleId }).HasName("PK__attendan__C675AE3CA28D8E75");

            entity.ToTable("attendance");

            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .HasColumnName("student_id");
            entity.Property(e => e.ScheduleId)
                .HasMaxLength(50)
                .HasColumnName("schedule_id");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.TeacherId)
                .HasMaxLength(50)
                .HasColumnName("teacher_id");

            entity.HasOne(d => d.Schedule).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__attendanc__sched__74AE54BC");

            entity.HasOne(d => d.Student).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__attendanc__stude__73BA3083");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK__attendanc__teach__76969D2E");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__class__3213E83F41DEDAC7");

            entity.ToTable("class");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("id");
            entity.Property(e => e.CourseId)
                .HasMaxLength(50)
                .HasColumnName("course_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.MaxStudents).HasColumnName("max_students");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Require)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("require");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.Target)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("target");
            entity.Property(e => e.TeacherId)
                .HasMaxLength(50)
                .HasColumnName("teacher_id");

            entity.HasOne(d => d.Course).WithMany(p => p.Classes)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__class__course_id__5812160E");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Classes)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK__class__teacher_i__59FA5E80");
        });

        modelBuilder.Entity<ClassAssignment>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.ClassId }).HasName("PK__classAss__55EC41025E46FD7B");

            entity.ToTable("classAssignment");

            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .HasColumnName("student_id");
            entity.Property(e => e.ClassId)
                .HasMaxLength(50)
                .HasColumnName("class_id");
            entity.Property(e => e.AssignedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("assigned_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.Class).WithMany(p => p.ClassAssignments)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__classAssi__class__6B24EA82");

            entity.HasOne(d => d.Student).WithMany(p => p.ClassAssignments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__classAssi__stude__6A30C649");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__course__3213E83F5438A2D0");

            entity.ToTable("course");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Fee)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("fee");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.NumberOfLessons).HasColumnName("number_of_lessons");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__enrollme__3213E83F7716011B");

            entity.ToTable("enrollment");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("id");
            entity.Property(e => e.CourseId)
                .HasMaxLength(50)
                .HasColumnName("course_id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.EnrollmentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("enrollment_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .HasColumnName("student_id");

            entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__enrollmen__cours__5DCAEF64");

            entity.HasOne(d => d.Student).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__enrollmen__stude__5CD6CB2B");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__payment__3213E83F65A85BC1");

            entity.ToTable("payment");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.EnrollmentId)
                .HasMaxLength(50)
                .HasColumnName("enrollment_id");
            entity.Property(e => e.FeeCollectorId)
                .HasMaxLength(50)
                .HasColumnName("fee_collector_id");
            entity.Property(e => e.Method)
                .HasMaxLength(30)
                .HasColumnName("method");
            entity.Property(e => e.PayerId)
                .HasMaxLength(50)
                .HasColumnName("payer_id");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("payment_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.Enrollment).WithMany(p => p.Payments)
                .HasForeignKey(d => d.EnrollmentId)
                .HasConstraintName("FK__payment__enrollm__628FA481");

            entity.HasOne(d => d.FeeCollector).WithMany(p => p.PaymentFeeCollectors)
                .HasForeignKey(d => d.FeeCollectorId)
                .HasConstraintName("FK__payment__fee_col__6477ECF3");

            entity.HasOne(d => d.Payer).WithMany(p => p.PaymentPayers)
                .HasForeignKey(d => d.PayerId)
                .HasConstraintName("FK__payment__payer_i__656C112C");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__schedule__3213E83FDE5E740C");

            entity.ToTable("schedule");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("id");
            entity.Property(e => e.ClassId)
                .HasMaxLength(50)
                .HasColumnName("class_id");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.Room)
                .HasMaxLength(50)
                .HasColumnName("room");
            entity.Property(e => e.SessionDate).HasColumnName("session_date");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.TeacherId)
                .HasMaxLength(50)
                .HasColumnName("teacher_id");

            entity.HasOne(d => d.Class).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__schedule__class___6FE99F9F");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK__schedule__teache__70DDC3D8");
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.ClassId, e.Type }).HasName("PK__score__1D0FB950132B0CFF");

            entity.ToTable("score");

            entity.Property(e => e.StudentId)
                .HasMaxLength(50)
                .HasColumnName("student_id");
            entity.Property(e => e.ClassId)
                .HasMaxLength(50)
                .HasColumnName("class_id");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Lisening)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("lisening");
            entity.Property(e => e.Reading)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("reading");
            entity.Property(e => e.Speaking)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("speaking");
            entity.Property(e => e.TeacherId)
                .HasMaxLength(50)
                .HasColumnName("teacher_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Writing)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("writing");

            entity.HasOne(d => d.Class).WithMany(p => p.Scores)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__score__class_id__7A672E12");

            entity.HasOne(d => d.Student).WithMany(p => p.Scores)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__score__student_i__797309D9");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Scores)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK__score__teacher_i__7B5B524B");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__student__3213E83F6BF91732");

            entity.ToTable("student");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("id");
            entity.Property(e => e.AccountId)
                .HasMaxLength(50)
                .HasColumnName("account_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone_number");

            entity.HasOne(d => d.Account).WithMany(p => p.Students)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__student__account__4F7CD00D");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__teacher__3213E83F889D1449");

            entity.ToTable("teacher");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .HasColumnName("id");
            entity.Property(e => e.AccountId)
                .HasMaxLength(50)
                .HasColumnName("account_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.Qualification)
                .HasMaxLength(255)
                .HasColumnName("qualification");
            entity.Property(e => e.Specialization)
                .HasMaxLength(255)
                .HasColumnName("specialization");

            entity.HasOne(d => d.Account).WithMany(p => p.Teachers)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__teacher__account__52593CB8");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
