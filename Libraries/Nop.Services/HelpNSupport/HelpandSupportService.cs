using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Data;
using Nop.Core.Domain.HelpNSupport;
using Nop.Services.Events;
using System.Linq;

namespace Nop.Services.HelpNSupport
{
    /// <summary>
    /// Help and support service
    /// Created By Alexandar Rajavel on 08-Feb-2019
    /// </summary>
    public class HelpandSupportService : IHelpandSupportService
    {

        private readonly IRepository<HelpandSupport> _helpandSupportRepository;
        private readonly IEventPublisher _eventPublisher;

        public HelpandSupportService(IRepository<HelpandSupport> helpandSupportRepository, IEventPublisher eventPublisher)
        {
            _helpandSupportRepository = helpandSupportRepository;
            _eventPublisher = eventPublisher;
        }

        public IList<HelpandSupport> GetAll()
        {
            var query = from h in _helpandSupportRepository.Table
                        where h.IsActive == true
                        select h;
            var helpNsupport = query.ToList();
            return helpNsupport;
        }
    }
}
