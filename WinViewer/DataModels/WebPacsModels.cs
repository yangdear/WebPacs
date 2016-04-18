namespace WinViewer.DataModels
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class WebPacsModels : DbContext
    {
        public WebPacsModels()
            : base("name=WebPacsModels")
        {
        }

        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Deptments> Deptments { get; set; }
        public virtual DbSet<Hospitals> Hospitals { get; set; }
        public virtual DbSet<PacsReports> PacsReports { get; set; }
        public virtual DbSet<Patients> Patients { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoles>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.PacsReports)
                .WithOptional(e => e.AspNetUsers)
                .HasForeignKey(e => e.ReportUser_Id);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.PacsReports1)
                .WithOptional(e => e.AspNetUsers1)
                .HasForeignKey(e => e.Uploader_Id);

            modelBuilder.Entity<Hospitals>()
                .HasMany(e => e.AspNetUsers)
                .WithOptional(e => e.Hospitals)
                .HasForeignKey(e => e.Hospital_HospitalId);

            modelBuilder.Entity<Hospitals>()
                .HasMany(e => e.Deptments)
                .WithOptional(e => e.Hospitals)
                .HasForeignKey(e => e.Hospital_HospitalId);

            modelBuilder.Entity<Hospitals>()
                .HasMany(e => e.Patients)
                .WithOptional(e => e.Hospitals)
                .HasForeignKey(e => e.Hosptial_HospitalId);

            modelBuilder.Entity<Patients>()
                .HasMany(e => e.PacsReports)
                .WithOptional(e => e.Patients)
                .HasForeignKey(e => e.patient_PatientId);
        }
    }
}
