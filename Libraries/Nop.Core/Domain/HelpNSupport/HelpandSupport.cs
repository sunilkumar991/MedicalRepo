using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.HelpNSupport
{
    // Created by Alexandar Rajavel on 08-Feb-2019
    public class HelpandSupport : BaseEntity
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
    }
}
