using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using BS.Plugin.NopStation.MobileWebApi.Models.Medicine;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Medicine;
using Nop.Services.Medicine;
using System.Linq;
using System;
using System.Collections.Generic;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer;
using Nop.Services.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Services.Customers;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    // Created by Alexandar Rajavel on 11-July-2019
    public class MedicineController : BaseApiController
    {
        private readonly IMedicineRequestService _medicineRequestService;
        private readonly IWorkContext _workContext;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICustomerService _customerService;
        private readonly IStoreContext _storeContext;

        public MedicineController(IMedicineRequestService medicineRequestService, IWorkContext workContext,
            IPriceFormatter priceFormatter, ICustomerService customerService, IStoreContext storeContext)
        {
            _medicineRequestService = medicineRequestService;
            _workContext = workContext;
            _priceFormatter = priceFormatter;
            _customerService = customerService;
            _storeContext = storeContext;
        }


        [Route("api/medicine/medicinerequest")]
        [HttpPost]
        public IActionResult MedicineRequest([FromBody]MedicineRequestData model)
        {
            var responseModel = new BaseResponse();
            if (ModelState.IsValid)
            {
                if (model.Id <= 0)
                {
                    var request = new MedicineRequest()
                    {
                        Id = model.Id,
                        PatientName = model.PatientName,
                        DoctorName = model.DoctorName,
                        HospitalName = model.HospitalName,
                        MobileNumber = model.MobileNumber,
                        Remarks = model.Remarks,
                        CustomerId = _workContext.CurrentCustomer.Id,
                        RequestStatusId = (int)RequestStatus.Pending,
                        PrescriptionImageUrl = string.Join(",", model.PrescriptionImageUrl)
                    };
                    _medicineRequestService.InsertMedicineRequest(request);
                    responseModel.SuccessMessage = SUCCESS;
                }
                else
                {
                    var medicineRequest = _medicineRequestService.GetMedicineRequestById(model.Id);
                    if (medicineRequest == null)
                    {
                        responseModel.SuccessMessage = FAILURE;
                        responseModel.StatusCode = (int)ErrorType.NotFound;
                        responseModel.ErrorList.Add(ID_NOT_FOUND);
                    }
                    else
                    {
                        medicineRequest.PatientName = model.PatientName;
                        medicineRequest.MobileNumber = model.MobileNumber;
                        medicineRequest.Remarks = model.Remarks;
                        medicineRequest.PrescriptionImageUrl = string.Join(",", model.PrescriptionImageUrl);
                        _medicineRequestService.UpdateMedicineRequest(medicineRequest);
                        responseModel.SuccessMessage = SUCCESS;
                    }
                }
            }
            else
            {
                responseModel.SuccessMessage = FAILURE;
                responseModel.StatusCode = (int)ErrorType.NotOk;
                responseModel.ErrorList = GetModelStateErrors(ModelState);
            }
            return Ok(responseModel);
        }

        [Route("api/medicine/getmymedicinerequest")]
        [HttpGet]
        public IActionResult GetMyMedicineRequest()
        {
            var result = new NewRegisterResponseModel<List<MedicineResponse>>();
            var medicineReuest = _medicineRequestService.GetMedicineRequestByCustomerId(_workContext.CurrentCustomer.Id);
            if (medicineReuest != null && medicineReuest.Any())
            {
                var responseList = new List<MedicineResponse>();
                var decOderTotal = 0M;
                var strOrderTotal = string.Empty;
                foreach (var request in medicineReuest)
                {
                    decOderTotal = 0M;
                    strOrderTotal = string.Empty;
                    var medicineItem = new List<MedicineItem>();
                    if ((RequestStatus)request.RequestStatusId == RequestStatus.Approved)
                    {
                        if (request.MedicineRequestItems.Any())
                        {
                            request.MedicineRequestItems.ToList().ForEach(i => medicineItem.Add(new MedicineItem()
                            {
                                MedicineName = i.MedicineName,
                                Quantity = i.Quantity,
                                UnitPrice = _priceFormatter.FormatPrice(i.UnitPrice),
                                TotalAmount = _priceFormatter.FormatPrice(i.TotalAmount),
                                IsAvailable = i.IsAvailable
                            }));
                            decOderTotal = request.MedicineRequestItems.Where(s => s.IsAvailable).Sum(s => s.TotalAmount);
                            strOrderTotal = _priceFormatter.FormatPrice(decOderTotal);
                        }
                    }
                    responseList.Add(new MedicineResponse()
                    {
                        Id = request.Id,
                        PatientName = request.PatientName,
                        DoctorName = request.DoctorName,
                        HospitalName = request.HospitalName,
                        MobileNumber = request.MobileNumber,
                        Remarks = request.Remarks,
                        PrescriptionImageUrl = request.PrescriptionImageUrl.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries),
                        RequestStatusId = (int)(RequestStatus)request.RequestStatusId,
                        RequestStatusMessage = ((RequestStatus)request.RequestStatusId).ToString(),
                        RejectedReason = request.RejectedReason,
                        MedicineItems = medicineItem,
                        OrderTotal = (int)decOderTotal,
                        OrderTotalValue = strOrderTotal
                    });
                }
                result.Data = responseList;
                result.SuccessMessage = SUCCESS;
            }
            else
            {
                result.SuccessMessage = FAILURE;
                result.StatusCode = (int)ErrorType.NotOk;
                result.ErrorList = GetModelStateErrors(ModelState);
            }
            return Ok(result);
        }

        [Route("api/AddMedicineToCart/{requestId}/{shoppingCartTypeId}")]
        [HttpGet]
        public IActionResult AddMedicineToCart(int requestId, int shoppingCartTypeId)
        {
            var medicineRequest = _medicineRequestService.GetMedicineRequestById(requestId);
            var responseModel = new BaseResponse();
            if (medicineRequest == null)
            {
                responseModel.SuccessMessage = FAILURE;
                responseModel.StatusCode = (int)ErrorType.NotFound;
                responseModel.ErrorList.Add(ID_NOT_FOUND);
            }
            else
            {
                if ((RequestStatus)medicineRequest.RequestStatusId == RequestStatus.Approved)
                {
                    var now = DateTime.UtcNow;
                    var customer = _workContext.CurrentCustomer;

                    foreach (var item in medicineRequest.MedicineRequestItems.Where(x => x.IsAvailable))
                    {
                        switch ((ShoppingCartType)shoppingCartTypeId)
                        {

                            case ShoppingCartType.ShoppingCart:
                                {
                                    var isExist = customer.ShoppingCartItems.Any(x => x.ProductId == item.ProductId && x.ShoppingCartType == ShoppingCartType.ShoppingCart);
                                    if (!isExist)
                                    {
                                        var shoppingCartItem = new ShoppingCartItem
                                        {
                                            ShoppingCartType = (ShoppingCartType)shoppingCartTypeId,
                                            StoreId = _storeContext.CurrentStore.Id,
                                            CustomerEnteredPrice = 0M,
                                            Quantity = item.Quantity,
                                            ProductId = (int)item.ProductId,
                                            CreatedOnUtc = now,
                                            UpdatedOnUtc = now
                                        };
                                        customer.ShoppingCartItems.Add(shoppingCartItem);
                                        _customerService.UpdateCustomer(customer);
                                    }
                                    break;
                                }
                            case ShoppingCartType.Wishlist:
                                {
                                    var isExist = customer.ShoppingCartItems.Any(x => x.ProductId == item.ProductId && x.ShoppingCartType == ShoppingCartType.Wishlist);
                                    if (!isExist)
                                    {
                                        var shoppingCartItem = new ShoppingCartItem
                                        {
                                            ShoppingCartType = (ShoppingCartType)shoppingCartTypeId,
                                            StoreId = _storeContext.CurrentStore.Id,
                                            CustomerEnteredPrice = 0M,
                                            Quantity = item.Quantity,
                                            ProductId = (int)item.ProductId,
                                            CreatedOnUtc = now,
                                            UpdatedOnUtc = now
                                        };
                                        customer.ShoppingCartItems.Add(shoppingCartItem);
                                        _customerService.UpdateCustomer(customer);
                                    }
                                    break;
                                }
                        }
                    }
                    responseModel.SuccessMessage = SUCCESS;
                }
                else
                {
                    responseModel.SuccessMessage = FAILURE;
                    responseModel.StatusCode = (int)ErrorType.NotOk;
                    responseModel.ErrorList.Add(NOT_APPROVED);
                }
            }
            return Ok(responseModel);
        }

    }
}
