using BS.Plugin.NopStation.MobileWebApi.Domain;
using Nop.Core;
using System.Collections.Generic;

namespace BS.Plugin.NopStation.MobileApp.Services
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
        IPagedList<Device> GetAllDevice(int pageIndex, int pageSize);

        /// <summary>
        /// Search Device
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="deviceType"></param>
        /// <param name="deviceToken"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IPagedList<Device> SearchDevice(int customerId = 0, int deviceType = 0, string deviceToken = null, int pageIndex = 0, int pageSize = int.MaxValue);


        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets all devices by customerId.
        /// </summary>
        /// <param name="CustomerId">CustomerId</param>
        /// <returns></returns>
        IList<Device> GetDevicesByCustomerId(int customerId);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Inserts the device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <returns></returns>
        Device InsertDevice(Device device);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the device by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        Device GetDeviceById(int id);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the device by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        Device GetDeviceByDeviceToken(string deviceToken);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Updates the device.
        /// </summary>
        /// <param name="device">The device.</param>
        void UpdateDevice(Device device);

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Deletes the device.
        /// </summary>
        /// <param name="device">The device.</param>
        void DeleteDevice(Device device);
    }
}