using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebViewer.Models
{
    public class FillReportViewModel
    {
        public int PacsReportId { get; set; }
        [Display(Name = "影像描述")]
        public string ImageDesc { get; set; }
        [Display(Name = "影像诊断")]
        public string ImagingDiagnosis { get; set; }
    }
}