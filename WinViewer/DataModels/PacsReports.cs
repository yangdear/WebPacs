namespace WinViewer.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PacsReports
    {
        [Key]
        public int PacsReportId { get; set; }

        public string PacsCode { get; set; }

        public DateTime CheckDate { get; set; }

        public string DicomFileName { get; set; }

        public string PreDiagnose { get; set; }

        public string CheckPoint { get; set; }

        public string ImageDesc { get; set; }

        public string ImagingDiagnosis { get; set; }

        public DateTime ReportDate { get; set; }

        public int? patient_PatientId { get; set; }

        [StringLength(128)]
        public string ReportUser_Id { get; set; }

        [StringLength(128)]
        public string Uploader_Id { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual AspNetUsers AspNetUsers1 { get; set; }

        public virtual Patients Patients { get; set; }
    }
}
