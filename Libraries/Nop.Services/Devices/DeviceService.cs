using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Devices;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Services.Devices
{
    public class DeviceService : IDeviceService
    {
        private readonly IRepository<BS_WebApi_Device> _deviceRepository;
        private readonly IEventPublisher _eventPublisher;

        public DeviceService(
            IRepository<BS_WebApi_Device> deviceRepository,
            IEventPublisher eventPublisher
            )
        {
            this._deviceRepository = deviceRepository;
            this._eventPublisher = eventPublisher;

        }


        public IList<BS_WebApi_Device> GetAllDevices()
        {
            var query = (from u in _deviceRepository.Table
                         orderby u.CustomerId
                         select u);
            var devices = query.ToList();
            return devices; ;
        }
        public IPagedList<BS_WebApi_Device> GetAllDevice(int pageIndex, int pageSize)
        {
            var query = (from u in _deviceRepository.Table
                         orderby u.CustomerId
                         select u);
            var devices = new PagedList<BS_WebApi_Device>(query, pageIndex, pageSize);
            return devices; ;
        }

        public IList<BS_WebApi_Device> GetDevicesByCustomerId(int customerId)
        {
            var query = (from u in _deviceRepository.Table
                         where u.CustomerId == customerId
                         orderby u.CreatedOnUtc descending
                         select u);
            var devices = query.ToList();
            return devices;
        }

        public BS_WebApi_Device InsertDevice(BS_WebApi_Device device)
        {
            if (device == null)
                throw new ArgumentNullException("device");

            _deviceRepository.Insert(device);
            //event device
            _eventPublisher.EntityInserted(device);

            return device; ;
        }

        public BS_WebApi_Device GetDeviceById(int id)
        {
            return _deviceRepository.GetById(id);
        }


        public BS_WebApi_Device GetDeviceByDeviceToken(string deviceToken)
        {

            BS_WebApi_Device device = null;
            var query = (from u in _deviceRepository.Table
                         where u.DeviceToken == deviceToken
                         select u);
            if (query.Any())
            {
                device = query.FirstOrDefault();
            }

            return device;
        }

        public void UpdateDevice(BS_WebApi_Device device)
        {

            if (device == null)
                throw new ArgumentNullException("device");
            _deviceRepository.Update(device);
            _eventPublisher.EntityUpdated(device);
        }

        public void DeleteDevice(BS_WebApi_Device device)
        {
            _deviceRepository.Delete(device);
            _eventPublisher.EntityDeleted(device);
        }
    }
}
