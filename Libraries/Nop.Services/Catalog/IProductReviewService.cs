using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Created By : Alexandar Rajavel on 17-Nov-2018
    /// Product review service interface
    /// </summary>
    public interface IProductReviewService
    {
        /// <summary>
        /// Gets an product review by address identifier
        /// </summary>
        /// <param name="productReviewId">ProductReview identifier</param>
        /// <returns>ProductReview</returns>
        ProductReview GetProductReviewById(int productReviewId);

        /// <summary>
        /// Updates the productReview
        /// </summary>
        /// <param name="productReview">productReview</param>
        void UpdateProductReview(ProductReview productReview);
    }
}
