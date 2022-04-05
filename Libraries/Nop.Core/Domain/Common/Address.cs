﻿using System;
using Nop.Core.Domain.Directory;

namespace Nop.Core.Domain.Common
{
    /// <summary>
    /// Address
    /// </summary>
    public partial class Address : BaseEntity, ICloneable
    {
        /// <summary>
        /// Gets or sets the first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the company
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets the country identifier
        /// </summary>
        public int? CountryId { get; set; }

        /// <summary>
        /// Gets or sets the state/province identifier
        /// </summary>
        public int? StateProvinceId { get; set; }

        /// <summary>
        /// Gets or sets the county
        /// </summary>
        public string County { get; set; }

        /// <summary>
        /// Gets or sets the city
        /// </summary>
        //public string City { get; set; }

        /// <summary>
        /// Gets or sets the CityId
        /// </summary>
        public int? CityId { get; set; }

        /// <summary>
        /// Gets or sets the address 1
        /// </summary>
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the address 2
        /// </summary>
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the zip/postal code
        /// </summary>
        public string ZipPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the phone number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the fax number
        /// </summary>
        public string FaxNumber { get; set; }

        /// <summary>
        /// Gets or sets the custom attributes (see "AddressAttribute" entity for more info)
        /// </summary>
        public string CustomAttributes { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the house number
        /// </summary>
        public string HouseNo { get; set; }

        /// <summary>
        /// Gets or sets the floor number
        /// </summary>
        public string FloorNo { get; set; }

        /// <summary>
        /// Gets or sets the room number
        /// </summary>
        public string RoomNo { get; set; }

        /// <summary>
        /// Gets or sets the lift option
        /// </summary>
        public bool IsLiftOption { get; set; }

        /// <summary>
        /// Gets or sets the rope color
        /// </summary>
        public string RopeColor { get; set; }

        /// <summary>
        /// Gets or sets the country
        /// </summary>
        public virtual Country Country { get; set; }

        /// <summary>
        /// Gets or sets the state/province
        /// </summary>
        public virtual StateProvince StateProvince { get; set; }

        /// <summary>
        /// Gets or sets the state/province
        /// </summary>
        public virtual City City { get; set; }

        /// <summary>
        /// Added by Sunil Kumar on 19-Dec-2018
        /// Gets or sets the latitude
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// Added by Sunil Kumar on 19-Dec-2018
        /// Gets or sets the longitude
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// Added by Alexandar Rajavel on 08-Mar-2019
        /// Gets or sets the Country Code
        /// </summary>
        public string CountryCode { get; set; }


        /// <summary>
        /// Clone
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var addr = new Address
            {
                FirstName = FirstName,
                Latitude = Latitude,
                Longitude = Longitude,
                LastName = LastName,
                Email = Email,
                Company = Company,
                Country = Country,
                CountryId = CountryId,
                StateProvince = StateProvince,
                StateProvinceId = StateProvinceId,
                County = County,
                City = City,
                CityId = CityId,
                Address1 = Address1,
                Address2 = Address2,
                ZipPostalCode = ZipPostalCode,
                PhoneNumber = PhoneNumber,
                FaxNumber = FaxNumber,
                CustomAttributes = CustomAttributes,
                CreatedOnUtc = CreatedOnUtc,
                HouseNo = HouseNo,
                FloorNo = FloorNo,
                RoomNo = RoomNo,
                IsLiftOption = IsLiftOption,
                RopeColor = RopeColor,
                CountryCode = CountryCode
            };

            return addr;
        }
    }
}