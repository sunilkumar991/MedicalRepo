using System;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Created By : Alexandar Rajavel on 17-Nov-2018
    /// Product review service
    /// </summary>
    public class ProductReviewService : IProductReviewService
    {
        private readonly IRepository<ProductReview> _productReviewRepository;
        private readonly IEventPublisher _eventPublisher;

        public ProductReviewService(IRepository<ProductReview> productReviewRepository, IEventPublisher eventPublisher)
        {
            _productReviewRepository = productReviewRepository;
            _eventPublisher = eventPublisher;
        }

        /// <summary>
        /// Gets a product review
        /// </summary>
        /// <param name="productReviewId">ProductReview identifier</param>
        /// <returns>ProductReview</returns>
        public ProductReview GetProductReviewById(int productReviewId)
        {
            if (productReviewId == 0)
                return null;

            return _productReviewRepository.GetById(productReviewId);
        }

        /// <summary>
        /// Updates the productReview
        /// </summary>
        /// <param name="productReview">productReview</param>
        public void UpdateProductReview(ProductReview productReview)
        {
            if (productReview == null)
                throw new ArgumentNullException(nameof(productReview));

            _productReviewRepository.Update(productReview);

            //event notification
            _eventPublisher.EntityUpdated(productReview);
        }
    }
}
