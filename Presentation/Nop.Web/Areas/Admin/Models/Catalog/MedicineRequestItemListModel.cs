using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public class MedicineRequestItemListModel
    {
            public int MedicineRequestID { get; set; }
            public string MedicineName { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public bool IsAvailable { get; set; }
            public string ProductID { get; set; }
    }
}
