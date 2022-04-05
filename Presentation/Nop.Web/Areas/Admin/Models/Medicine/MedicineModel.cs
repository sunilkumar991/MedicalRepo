using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Medicine
{
    public class MedicineModel : BaseNopEntityModel
    {
        public string RejectedReason { get; set; }

        [Required(ErrorMessage = "Request status id is required")]
        public int RequestStatusId { get; set; }
    }
}
