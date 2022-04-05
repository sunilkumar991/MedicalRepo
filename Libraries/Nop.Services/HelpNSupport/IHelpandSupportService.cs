using Nop.Core.Domain.HelpNSupport;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.HelpNSupport
{
    /// <summary>
    /// Help and support service interface
    /// Created By Alexandar Rajavel on 08-Feb-2019
    /// </summary>
    public interface IHelpandSupportService
    {
        /// <summary>
        /// Get all details
        /// </summary>
        /// <returns>HelpandSupportDetail</returns>
        IList<HelpandSupport> GetAll();
    }
}
