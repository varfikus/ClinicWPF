using System;
using System.Collections.Generic;
using ClinicWPF.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicWPF;

public partial class ClinicdbContext : DbContext
{
    public ClinicdbContext()
    {
    }

    public ClinicdbContext(DbContextOptions<ClinicdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Clinic> Clinics { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Doctorclinic> Doctorclinics { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=127.0.0.1;Port=5432;Database=clinicdb;Username=postgres;Password=admin");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("appointments_pkey");

            entity.ToTable("appointments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Appointmentdatetime).HasColumnName("appointmentdatetime");
            entity.Property(e => e.Clinicid).HasColumnName("clinicid");
            entity.Property(e => e.Doctorid).HasColumnName("doctorid");
            entity.Property(e => e.Patientid).HasColumnName("patientid");
            entity.Property(e => e.Reasonforvisit)
                .HasMaxLength(255)
                .HasColumnName("reasonforvisit");

            entity.HasOne(d => d.Clinic).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Clinicid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("appointments_clinicid_fkey");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Doctorid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("appointments_doctorid_fkey");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Patientid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("appointments_patientid_fkey");
        });

        modelBuilder.Entity<Clinic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("clinics_pkey");

            entity.ToTable("clinics");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Clinicname)
                .HasMaxLength(100)
                .HasColumnName("clinicname");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("doctors_pkey");

            entity.ToTable("doctors");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            entity.Property(e => e.Ismaindoctor)
                .HasDefaultValue(false)
                .HasColumnName("ismaindoctor");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.Specialty)
                .HasMaxLength(100)
                .HasColumnName("specialty");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
        });

        modelBuilder.Entity<Doctorclinic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("doctorclinics_pkey");

            entity.ToTable("doctorclinics");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Clinicid).HasColumnName("clinicid");
            entity.Property(e => e.Doctorid).HasColumnName("doctorid");
            entity.Property(e => e.Ismaindoctor)
                .HasDefaultValue(false)
                .HasColumnName("ismaindoctor");

            entity.HasOne(d => d.Clinic).WithMany(p => p.Doctorclinics)
                .HasForeignKey(d => d.Clinicid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("doctorclinics_clinicid_fkey");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Doctorclinics)
                .HasForeignKey(d => d.Doctorid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("doctorclinics_doctorid_fkey");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("patients_pkey");

            entity.ToTable("patients");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Dateofbirth).HasColumnName("dateofbirth");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
