namespace WinViewer.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Patients
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Patients()
        {
            PacsReports = new HashSet<PacsReports>();
        }

        [Key]
        public int PatientId { get; set; }

        public string PatientCode { get; set; }

        public string PatientName { get; set; }

        public string Sex { get; set; }

        public DateTime Birthday { get; set; }

        public string RequestDept { get; set; }

        public int? Hosptial_HospitalId { get; set; }

        public virtual Hospitals Hospitals { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PacsReports> PacsReports { get; set; }
    }
}
