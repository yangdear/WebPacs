using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebViewer.Models
{
    public class Hospital
    {
        [Key]
        public int HospitalId { get; set; }

        public string HospitalName { get; set; }
        public virtual ICollection<Patient> Patients { get; set; }
    }

    public class Patient
    {
        [Key]
        [Display(Name ="患者编码")]
        public int PatientId { get; set; }
        [Display(Name ="患者编号")]
        public string PatientCode { get; set; }

        [Display(Name = "姓名")]
        public string PatientName { get; set; }

        [Display(Name = "性别")]
        public string Sex { get; set; }
        [Display(Name = "出生日期")]
        public DateTime Birthday { get; set; }
        public virtual ICollection<PacsReport> PacsReports { get; set; }
    }
    public class PacsReport
    {
        [Key]
        public int PacsReportId { get; set; }
        [Display(Name = "检查日期")]
        public DateTime CheckDate { get; set; }
        [Display(Name ="影像文件")]
        public string DicomFileName { get; set; }
        [Display(Name = "初步诊断")]
        public string PreDiagnose { get; set; }
        [Display(Name = "检查部位")]
        public string CheckPoint { get; set; }

        [Display(Name = "影像描述")]
        public string ImageDesc { get; set; }
        [Display(Name = "影像诊断")]
        public string ImagingDiagnosis  { get; set; }

    }
    public class PacsDBContext : IdentityDbContext
    {
        public DbSet<Hospital> Hospital { get; set; }
        public DbSet<Patient> Patient { get; set; }
        public DbSet<PacsReport> PacsReport { get; set; }


    }
}