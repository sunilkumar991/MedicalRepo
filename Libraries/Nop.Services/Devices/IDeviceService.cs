using Nop.Core;
using Nop.Core.Domain.Devices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Devices
{
    public interface IDeviceService
    {
        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets all device.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        IList<BS_WebApi_Device> GetAllDevices();
        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets all device.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        IPagedList<BS_WebApi_Device> GetAllDevice(int pageIndex, int pageSize);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets all devices by customerId.
        /// </summary>
        /// <param name="CustomerId">CustomerId</param>
        /// <returns></returns>
        IList<BS_WebApi_Device> GetDevicesByCustomerId(int customerId);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Inserts the device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <returns></returns>
        BS_WebApi_Device InsertDevice(BS_WebApi_Device device);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the device by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        BS_WebApi_Device GetDeviceById(int id);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the device by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        BS_WebApi_Device GetDeviceByDeviceToken(string deviceToken);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Updates the device.
        /// </summary>
        /// <param name="device">The device.</param>
        void UpdateDevice(BS_WebApi_Device device);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Deletes the device.
        /// </summary>
        /// <param name="device">The device.</param>
        void DeleteDevice(BS_WebApi_Device device);
    }
}
