﻿using System;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Models;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
    public partial class BlogCommentModel : BaseNopEntityModel
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string CustomerAvatarUrl { get; set; }

        public string CommentText { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool AllowViewingProfiles { get; set; }
    }
}