namespace WinViewer.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Deptments
    {
        [Key]
        public int DeptmentId { get; set; }

        public string DeptmentName { get; set; }

        public int? Hospital_HospitalId { get; set; }

        public virtual Hospitals Hospitals { get; set; }
    }
}
