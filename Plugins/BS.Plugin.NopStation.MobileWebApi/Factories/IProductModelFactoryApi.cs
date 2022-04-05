﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Models.Product;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Product;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;

namespace BS.Plugin.NopStation.MobileWebApi.Factories
{
    /// <summary>
    /// Represents the interface of the product model factory
    /// </summary>
    public partial interface IProductModelFactoryApi
    {
        /// <summary>
        /// Get the product template view path
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>View path</returns>
        string PrepareProductTemplateViewPath(Product product);

        /// <summary>
        /// Prepare the product overview models
        /// </summary>
        /// <param name="products">Collection of products</param>
        /// <param name="preparePriceModel">Whether to prepare the price model</param>
        /// <param name="preparePictureModel">Whether to prepare the picture model</param>
        /// <param name="productThumbPictureSize">Product thumb picture size (longest side); pass null to use the default value of media settings</param>
        /// <param name="prepareSpecificationAttributes">Whether to prepare the specification attribute models</param>
        /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
        /// <returns>Collection of product overview model</returns>
        IEnumerable<ProductOverViewModelApi> PrepareProductOverviewModels(IEnumerable<Product> products,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false, bool isHomePageCategoriesProduct = false);

        /// <summary>
        /// Prepare the product details model
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <param name="isAssociatedProduct">Whether the product is associated</param>
        /// <returns>Product details model</returns>
        ProductDetailsModelApi PrepareProductDetailsModel(Product product, ShoppingCartItem updatecartitem = null, bool isAssociatedProduct = false);

        /// <summary>
        /// Prepare the product reviews model
        /// </summary>
        /// <param name="model">Product reviews model</param>
        /// <param name="product">Product</param>
        /// <returns>Product reviews model</returns>
        ProductReviewsResponseModel PrepareProductReviewsModel(ProductReviewsResponseModel model, Product product);

        /// <summary>
        /// Created By : Ankur Shriastava
        /// </summary>
        /// <param name="model"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        ProductReviewsResponseModel PrepareCustomerProductReviewsModel(ProductReviewsResponseModel model, Product product);
        /// <summary>
        /// Prepare the customer product reviews model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>Customer product reviews model</returns>
        //CustomerProductReviewsModel PrepareCustomerProductReviewsModel(int? page);

        /// <summary>
        /// Prepare the product email a friend model
        /// </summary>
        /// <param name="model">Product email a friend model</param>
        /// <param name="product">Product</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>product email a friend model</returns>
        //ProductEmailAFriendModel PrepareProductEmailAFriendModel(ProductEmailAFriendModel model, Product product, bool excludeProperties);

        /// <summary>
        /// Prepare the product specification models
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>List of product specification model</returns>
        IList<ProductSpecificationModel> PrepareProductSpecificationModel(Product product);

        /// <summary>
        /// Prepare filter items
        /// Created by Alexandar Rajavel on 11-Nov-2018
        /// </summary>
        /// <returns>List of filter items</returns>
        List<SpecificationFilterItem> GetFilterItems(IList<Product> products);

        /// <summary>
        /// Prepare sort items
        /// Created by Alexandar Rajavel on 13-Nov-2018
        /// </summary>
        /// <returns>List of sort items</returns>
        dynamic GetAvailableSortOptions();
    }
}
