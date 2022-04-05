using Nop.Core.Domain.Localization;
using System.Collections.Generic;

namespace Nop.Core.Domain.Directory
{
    /// <summary>
    /// Represents a city
    /// </summary>
    public partial class City : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the state identifier
        /// </summary>
        public int StateId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the abbreviation
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Shipping Allwed or not to City flag
        /// </summary>
        public bool IsShippingAllowed { get; set; }

        /// <summary>
        /// Created By : Ankur Shrivastava
        /// Cretaed On : 19-OCT-2018
        /// Is Delivery Allowed or not to City flag
        /// </summary>
        public bool IsDeliveryAllowed { get; set; }

        // <summary>
        ///  Gets or sets the ShippingCharge cost, Added by Alexandar Rajavel on 10-June-2019
        /// </summary>
        public decimal? ShippingCharge { get; set; }

        //Added by Alexandar Rajavel on 20-June-2019
        public decimal? ShippingChargeUptoOneKg { get; set; }
        public decimal? ShippingChargeUptoTwoKg { get; set; }
        public decimal? ShippingChargeUptoThreeKg { get; set; }
        public decimal? ShippingChargeUptoFourKg { get; set; }
        public decimal? ShippingChargeUptoFiveKg { get; set; }
        public decimal? ShippingChargeAboveFiveKg { get; set; }

        /// <summary>
        /// Gets or sets the state
        /// </summary>
        public virtual StateProvince StateProvince { get; set; }

        // <summary>
        ///  Gets or sets the MinOrderValue, Added by Sunil Kumar on 30-04-2020
        /// </summary>
        public decimal? MinOrderValue { get; set; }

        
    }
}
