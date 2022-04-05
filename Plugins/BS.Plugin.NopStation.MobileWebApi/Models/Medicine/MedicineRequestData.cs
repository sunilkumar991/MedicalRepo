using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Medicine
{
    public class MedicineRequestData
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Patient name is required")]
        public string PatientName { get; set; }

        public string DoctorName { get; set; }

        public string HospitalName { get; set; }

        [Required(ErrorMessage = "MobileNumber is required")]
        public string MobileNumber { get; set; }

        public string Remarks { get; set; }

        [Required(ErrorMessage = "At least one prescription image url is required")]
        public string[] PrescriptionImageUrl { get; set; }
    }
}
