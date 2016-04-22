using Microsoft.AspNet.Identity;
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
        [Display(Name ="医院编号")]
        public int HospitalId { get; set; }
        [Display(Name ="医院名称")]
        public string HospitalName { get; set; }
        public virtual ICollection<Patient> Patients { get; set; }
        
        public virtual ICollection<ApplicationUser> Operators { get; set; }
        public virtual ICollection<Deptment> Deptments { get; set; }

        public Hospital()
        {
            Operators = new HashSet<ApplicationUser>();
            Deptments = new HashSet<Deptment>();
            Patients = new HashSet<Patient>();
        }
    }
    
    public class Deptment
    {
        [Key]
        public int DeptmentId { get; set; }
        [Display(Name ="科室名称")]
        public string DeptmentName { get; set; }

        public int? HospitalId { get; set; }
        [Display(Name ="所属医院")]
        public virtual Hospital Hospital { get; set; }
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
        [Display(Name = "申请科室")]
        public string RequestDept { get; set; }
        [Display(Name = "Pacs报告")]
        public virtual ICollection<PacsReport> PacsReports { get; set; }
        public int? HospitalId { get; set; }
        [Display(Name ="所属医院")]
        public Hospital Hosptial { get; set; }


        public Patient()
        {
            PacsReports = new HashSet<PacsReport>();
        }
    }
    public class PacsReport
    {
        [Key]
        public int PacsReportId { get; set; }
        [Display(Name = "影像编号")]
        public string PacsCode { get; set; }
        [Display(Name = "检查时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss}")]
        [DataType(DataType.DateTime)]
        public DateTime? CheckDate { get; set; }
        [Display(Name = "影像文件")]
        public string DicomFileName { get; set; }
        [Display(Name = "初步诊断")]
        public string PreDiagnose { get; set; }
        [Display(Name = "检查部位")]
        public string CheckPoint { get; set; }

        [Display(Name = "影像描述")]
        public string ImageDesc { get; set; }
        [Display(Name = "影像诊断")]
        public string ImagingDiagnosis { get; set; }
        [Display(Name ="报告时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm:ss}")]
        [DataType(DataType.DateTime)]
        public DateTime? ReportDate { get; set; }
        public int? PatientId { get; set; }
        [Display(Name ="患者信息")]
        public virtual Patient Patient { get; set; }
        [Display(Name ="上传人")]
        public virtual ApplicationUser Uploader { get; set; }
        [Display(Name ="报告人")]
        public virtual ApplicationUser ReportUser { get; set; }
        [Display(Name ="已添加报告")]
        public ReportStateEnum ReportState { get; set; }
    }


    
    public enum ReportStateEnum
    {
        reUnknow = 0,
        rsUploadDicom = 1,
        rsFilledReport = 2
    }
    public class PacsDbInitializer: DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            string name = "Admin";
            string password = "123456";
            if (!RoleManager.RoleExists(name))
            {
                var roleresult = RoleManager.Create(new IdentityRole(name));
            }
            if (!RoleManager.RoleExists("operator"))
            {
                var roleresult = RoleManager.Create(new IdentityRole("operator"));
            }
           
            var op = new ApplicationUser();
            op.UserName = name;
            var result = UserManager.Create(op, password);
            if (result.Succeeded)
            {
                var r = UserManager.AddToRole(op.Id, name);
            }

            base.Seed(context);
        }
    }
}