﻿using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Services.Events;

namespace BS.Plugin.NopStation.MobileWebApi.Infrastructure.Cache
{
    public partial class ModelCacheEventConsumer :
       //languages
       IConsumer<EntityInsertedEvent<Language>>,
       IConsumer<EntityUpdatedEvent<Language>>,
       IConsumer<EntityDeletedEvent<Language>>,
       //currencies
       IConsumer<EntityInsertedEvent<Currency>>,
       IConsumer<EntityUpdatedEvent<Currency>>,
       IConsumer<EntityDeletedEvent<Currency>>,
       //settings
       IConsumer<EntityUpdatedEvent<Setting>>,
       //manufacturers
       IConsumer<EntityInsertedEvent<Manufacturer>>,
       IConsumer<EntityUpdatedEvent<Manufacturer>>,
       IConsumer<EntityDeletedEvent<Manufacturer>>,
       //vendors
       IConsumer<EntityInsertedEvent<Vendor>>,
       IConsumer<EntityUpdatedEvent<Vendor>>,
       IConsumer<EntityDeletedEvent<Vendor>>,
       //product manufacturers
       IConsumer<EntityInsertedEvent<ProductManufacturer>>,
       IConsumer<EntityUpdatedEvent<ProductManufacturer>>,
       IConsumer<EntityDeletedEvent<ProductManufacturer>>,
       //categories
       IConsumer<EntityInsertedEvent<Category>>,
       IConsumer<EntityUpdatedEvent<Category>>,
       IConsumer<EntityDeletedEvent<Category>>,
       //product categories
       IConsumer<EntityInsertedEvent<ProductCategory>>,
       IConsumer<EntityUpdatedEvent<ProductCategory>>,
       IConsumer<EntityDeletedEvent<ProductCategory>>,
       //products
       IConsumer<EntityInsertedEvent<Product>>,
       IConsumer<EntityUpdatedEvent<Product>>,
       IConsumer<EntityDeletedEvent<Product>>,
       //related product
       IConsumer<EntityInsertedEvent<RelatedProduct>>,
       IConsumer<EntityUpdatedEvent<RelatedProduct>>,
       IConsumer<EntityDeletedEvent<RelatedProduct>>,
       //product tags
       IConsumer<EntityInsertedEvent<ProductTag>>,
       IConsumer<EntityUpdatedEvent<ProductTag>>,
       IConsumer<EntityDeletedEvent<ProductTag>>,
       //specification attributes
       IConsumer<EntityUpdatedEvent<SpecificationAttribute>>,
       IConsumer<EntityDeletedEvent<SpecificationAttribute>>,
       //specification attribute options
       IConsumer<EntityUpdatedEvent<SpecificationAttributeOption>>,
       IConsumer<EntityDeletedEvent<SpecificationAttributeOption>>,
       //Product specification attribute
       IConsumer<EntityInsertedEvent<ProductSpecificationAttribute>>,
       IConsumer<EntityUpdatedEvent<ProductSpecificationAttribute>>,
       IConsumer<EntityDeletedEvent<ProductSpecificationAttribute>>,
       //Product attributes
       IConsumer<EntityDeletedEvent<ProductAttribute>>,
       //Product attributes
       IConsumer<EntityInsertedEvent<ProductAttributeMapping>>,
       IConsumer<EntityDeletedEvent<ProductAttributeMapping>>,
       //Product attribute values
       IConsumer<EntityUpdatedEvent<ProductAttributeValue>>,
       //Topics
       IConsumer<EntityInsertedEvent<Topic>>,
       IConsumer<EntityUpdatedEvent<Topic>>,
       IConsumer<EntityDeletedEvent<Topic>>,
       //Orders
       IConsumer<EntityInsertedEvent<Order>>,
       IConsumer<EntityUpdatedEvent<Order>>,
       IConsumer<EntityDeletedEvent<Order>>,
       //Picture
       IConsumer<EntityInsertedEvent<Picture>>,
       IConsumer<EntityUpdatedEvent<Picture>>,
       IConsumer<EntityDeletedEvent<Picture>>,
       //Product picture mapping
       IConsumer<EntityInsertedEvent<ProductPicture>>,
       IConsumer<EntityUpdatedEvent<ProductPicture>>,
       IConsumer<EntityDeletedEvent<ProductPicture>>,
       //polls
       IConsumer<EntityInsertedEvent<Poll>>,
       IConsumer<EntityUpdatedEvent<Poll>>,
       IConsumer<EntityDeletedEvent<Poll>>,
       //blog posts
       IConsumer<EntityInsertedEvent<BlogPost>>,
       IConsumer<EntityUpdatedEvent<BlogPost>>,
       IConsumer<EntityDeletedEvent<BlogPost>>,
       //news items
       IConsumer<EntityInsertedEvent<NewsItem>>,
       IConsumer<EntityUpdatedEvent<NewsItem>>,
       IConsumer<EntityDeletedEvent<NewsItem>>,
       //states/province
       IConsumer<EntityInsertedEvent<StateProvince>>,
       IConsumer<EntityUpdatedEvent<StateProvince>>,
       IConsumer<EntityDeletedEvent<StateProvince>>,
       //templates
       IConsumer<EntityInsertedEvent<CategoryTemplate>>,
       IConsumer<EntityUpdatedEvent<CategoryTemplate>>,
       IConsumer<EntityDeletedEvent<CategoryTemplate>>,
       IConsumer<EntityInsertedEvent<ManufacturerTemplate>>,
       IConsumer<EntityUpdatedEvent<ManufacturerTemplate>>,
       IConsumer<EntityDeletedEvent<ManufacturerTemplate>>,
       IConsumer<EntityInsertedEvent<ProductTemplate>>,
       IConsumer<EntityUpdatedEvent<ProductTemplate>>,
       IConsumer<EntityDeletedEvent<ProductTemplate>>,
       IConsumer<EntityInsertedEvent<TopicTemplate>>,
       IConsumer<EntityUpdatedEvent<TopicTemplate>>,
       IConsumer<EntityDeletedEvent<TopicTemplate>>,
       //checkout attributes
       IConsumer<EntityInsertedEvent<CheckoutAttribute>>,
       IConsumer<EntityUpdatedEvent<CheckoutAttribute>>,
       IConsumer<EntityDeletedEvent<CheckoutAttribute>>,
       //shopping cart items
       IConsumer<EntityUpdatedEvent<ShoppingCartItem>>
    {

        #region
        public const string CATEGORY_HOMEPAGE_KEY_PRODUCT_CATEGORY = "Nop.pres.category.homepage-Bswebapi-ProductPerCategory-{0}-{1}-{2}-{3}";
        #endregion



        /// <summary>
        /// Key for categories on the search page
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string SEARCH_CATEGORIES_MODEL_KEY = "Nop.pres.search.categories-Bswebapi-{0}-{1}-{2}";
        public const string SEARCH_CATEGORIES_PATTERN_KEY = "Nop.pres.search.categories";

        /// <summary>
        /// Key for ManufacturerNavigationModel caching
        /// </summary>
        /// <remarks>
        /// {0} : current manufacturer id
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public const string MANUFACTURER_NAVIGATION_MODEL_KEY = "Nop.pres.manufacturer.navigation-Bswebapi-{0}-{1}-{2}-{3}";
        public const string MANUFACTURER_NAVIGATION_PATTERN_KEY = "Nop.pres.manufacturer.navigation";

        /// <summary>
        /// Key for VendorNavigationModel caching
        /// </summary>
        public const string VENDOR_NAVIGATION_MODEL_KEY = "Nop.pres.vendor.navigation-Bswebapi";
        public const string VENDOR_NAVIGATION_PATTERN_KEY = "Nop.pres.vendor.navigation";

        /// <summary>
        /// Key for caching of a value indicating whether a manufacturer has featured products
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string MANUFACTURER_HAS_FEATURED_PRODUCTS_KEY = "Nop.pres.manufacturer.hasfeaturedproducts-Bswebapi-{0}-{1}-{2}";
        public const string MANUFACTURER_HAS_FEATURED_PRODUCTS_PATTERN_KEY = "Nop.pres.manufacturer.hasfeaturedproducts";

        /// <summary>
        /// Key for CategoryNavigationModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : comma separated list of customer roles
        /// {2} : current store ID
        /// </remarks>
        public const string CATEGORY_NAVIGATION_MODEL_KEY = "Nop.pres.category.navigation-Bswebapi-{0}-{1}-{2}";
        public const string CATEGORY_NAVIGATION_PATTERN_KEY = "Nop.pres.category.navigation";

        /// <summary>
        /// Key for list of CategorySimpleModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : comma separated list of customer roles
        /// {2} : current store ID
        /// </remarks>
        public const string CATEGORY_ALL_MODEL_KEY = "Nop.pres.category.all-Bswebapi-{0}-{1}-{2}";
        public const string CATEGORY_ALL_PATTERN_KEY = "Nop.pres.category.all";

        /// <summary>
        /// Key for TopMenuModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : comma separated list of customer roles
        /// {2} : current store ID
        /// </remarks>
        public const string CATEGORY_MENU_MODEL_KEY = "Nop.pres.category.menu-Bswebapi-{0}-{1}-{2}";
        public const string CATEGORY_MENU_PATTERN_KEY = "Nop.pres.category.menu";

        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : comma separated list of customer roles
        /// {1} : current store ID
        /// {2} : category ID
        /// </remarks>
        public const string CATEGORY_NUMBER_OF_PRODUCTS_MODEL_KEY = "Nop.pres.category.numberofproducts-Bswebapi-{0}-{1}-{2}";
        public const string CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY = "Nop.pres.category.numberofproducts";

        /// <summary>
        /// Key for caching of a value indicating whether a category has featured products
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string CATEGORY_HAS_FEATURED_PRODUCTS_KEY = "Nop.pres.category.hasfeaturedproducts-Bswebapi-{0}-{1}-{2}";
        public const string CATEGORY_HAS_FEATURED_PRODUCTS_PATTERN_KEY = "Nop.pres.category.hasfeaturedproducts";

        /// <summary>
        /// Key for caching of category breadcrumb
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : language ID
        /// </remarks>
        public const string CATEGORY_BREADCRUMB_KEY = "Nop.pres.category.breadcrumb-Bswebapi-{0}-{1}-{2}-{3}";
        public const string CATEGORY_BREADCRUMB_PATTERN_KEY = "Nop.pres.category.breadcrumb";

        /// <summary>
        /// Key for caching of subcategories of certain category
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// {3} : language ID
        /// {4} : is connection SSL secured (included in a category picture URL)
        /// </remarks>
        public const string CATEGORY_SUBCATEGORIES_KEY = "Nop.pres.category.subcategories-Bswebapi-{0}-{1}-{2}-{3}-{4}";
        public const string CATEGORY_SUBCATEGORIES_PATTERN_KEY = "Nop.pres.category.subcategories";

        /// <summary>
        /// Key for caching of categories displayed on home page
        /// </summary>
        /// <remarks>
        /// {0} : roles of the current user
        /// {1} : current store ID
        /// {2} : language ID
        /// {3} : is connection SSL secured (included in a category picture URL)
        /// </remarks>

        public const string CATEGORY_HOMEPAGE_KEY = "Nop.pres.category.homepage-Bswebapi-{0}-{1}-{2}-{3}";
        public const string CATEGORY_HOMEPAGE_PATTERN_KEY = "Nop.pres.category.homepage";

        /// <summary>
        /// Key for GetChildCategoryIds method results caching
        /// </summary>
        /// <remarks>
        /// {0} : parent category id
        /// {1} : comma separated list of customer roles
        /// {2} : current store ID
        /// </remarks>
        public const string CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY = "Nop.pres.category.childidentifiers-Bswebapi-{0}-{1}-{2}";
        public const string CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY = "Nop.pres.category.childidentifiers";

        /// <summary>
        /// Key for ProductBreadcrumbModel caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// {2} : comma separated list of customer roles
        /// {3} : current store ID
        /// </remarks>
        public const string PRODUCT_BREADCRUMB_MODEL_KEY = "Nop.pres.product.breadcrumb-Bswebapi-{0}-{1}-{2}-{3}";
        public const string PRODUCT_BREADCRUMB_PATTERN_KEY = "Nop.pres.product.breadcrumb";


        /// <summary>
        /// Key for ProductTagModel caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// {2} : current store ID
        /// </remarks>
        public const string PRODUCTTAG_BY_PRODUCT_MODEL_KEY = "Nop.pres.producttag.byproduct-Bswebapi-{0}-{1}-{2}";
        public const string PRODUCTTAG_BY_PRODUCT_PATTERN_KEY = "Nop.pres.producttag.byproduct";

        /// <summary>
        /// Key for PopularProductTagsModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current store ID
        /// </remarks>
        public const string PRODUCTTAG_POPULAR_MODEL_KEY = "Nop.pres.producttag.popular-Bswebapi-{0}-{1}";
        public const string PRODUCTTAG_POPULAR_PATTERN_KEY = "Nop.pres.producttag.popular";

        /// <summary>
        /// Key for ProductManufacturers model caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// {2} : roles of the current user
        /// {3} : current store ID
        /// </remarks>
        public const string PRODUCT_MANUFACTURERS_MODEL_KEY = "Nop.pres.product.manufacturers-Bswebapi-{0}-{1}-{2}-{3}";
        public const string PRODUCT_MANUFACTURERS_PATTERN_KEY = "Nop.pres.product.manufacturers";

        /// <summary>
        /// Key for ProductSpecificationModel caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : language id
        /// </remarks>
        public const string PRODUCT_SPECS_MODEL_KEY = "Nop.pres.product.specs-Bswebapi-{0}-{1}";
        public const string PRODUCT_SPECS_PATTERN_KEY = "Nop.pres.product.specs";

        /// <summary>
        /// Key for caching of a value indicating whether a product has product attributes
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// </remarks>
        public const string PRODUCT_HAS_PRODUCT_ATTRIBUTES_KEY = "Nop.pres.product.hasproductattributes-Bswebapi-{0}";
        public const string PRODUCT_HAS_PRODUCT_ATTRIBUTES_PATTERN_KEY = "Nop.pres.product.hasproductattributes";

        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// {1} : language id
        /// {2} : store id
        /// </remarks>
        public const string TOPIC_MODEL_BY_SYSTEMNAME_KEY = "Nop.pres.topic.details.bysystemname-{0}-{1}-{2}";
        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic id
        /// {1} : language id
        /// {2} : store id
        /// </remarks>
        public const string TOPIC_MODEL_BY_ID_KEY = "Nop.pres.topic.details.byid-{0}-{1}-{2}";
        /// <summary>
        /// Key for TopicModel caching
        /// </summary>
        /// <remarks>
        /// {0} : topic system name
        /// {1} : language id
        /// {2} : store id
        /// </remarks>
        public const string TOPIC_SENAME_BY_SYSTEMNAME = "Nop.pres.topic.sename.bysystemname-{0}-{1}-{2}";
        /// <summary>
        /// Key for TopMenuModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current store ID
        /// </remarks>
        public const string TOPIC_TOP_MENU_MODEL_KEY = "Nop.pres.topic.topmenu-{0}-{1}";
        /// <summary>
        /// Key for TopMenuModel caching
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : current store ID
        /// </remarks>
        public const string TOPIC_FOOTER_MODEL_KEY = "Nop.pres.topic.footer-{0}-{1}";
        public const string TOPIC_PATTERN_KEY = "Nop.pres.topic";

        /// <summary>
        /// Key for CategoryTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : category template id
        /// </remarks>
        public const string CATEGORY_TEMPLATE_MODEL_KEY = "Nop.pres.categorytemplate-{0}";
        public const string CATEGORY_TEMPLATE_PATTERN_KEY = "Nop.pres.categorytemplate";

        /// <summary>
        /// Key for ManufacturerTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer template id
        /// </remarks>
        public const string MANUFACTURER_TEMPLATE_MODEL_KEY = "Nop.pres.manufacturertemplate-{0}";
        public const string MANUFACTURER_TEMPLATE_PATTERN_KEY = "Nop.pres.manufacturertemplate";

        /// <summary>
        /// Key for ProductTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : product template id
        /// </remarks>
        public const string PRODUCT_TEMPLATE_MODEL_KEY = "Nop.pres.producttemplate-{0}";
        public const string PRODUCT_TEMPLATE_PATTERN_KEY = "Nop.pres.producttemplate";

        /// <summary>
        /// Key for TopicTemplate caching
        /// </summary>
        /// <remarks>
        /// {0} : topic template id
        /// </remarks>
        public const string TOPIC_TEMPLATE_MODEL_KEY = "Nop.pres.topictemplate-{0}";
        public const string TOPIC_TEMPLATE_PATTERN_KEY = "Nop.pres.topictemplate";

        /// <summary>
        /// Key for bestsellers identifiers displayed on the home page
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// </remarks>
        public const string HOMEPAGE_BESTSELLERS_IDS_KEY = "Nop.pres.bestsellers.homepage-Bswebapi-{0}";
        public const string HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY = "Nop.pres.bestsellers.homepage";

        /// <summary>
        /// Key for "also purchased" product identifiers displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// {1} : current store ID
        /// </remarks>
        public const string PRODUCTS_ALSO_PURCHASED_IDS_KEY = "Nop.pres.alsopuchased-{0}-{1}";
        public const string PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY = "Nop.pres.alsopuchased";

        /// <summary>
        /// Key for "related" product identifiers displayed on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : current product id
        /// {1} : current store ID
        /// </remarks>
        public const string PRODUCTS_RELATED_IDS_KEY = "Nop.pres.related-Bswebapi-{0}-{1}";
        public const string PRODUCTS_RELATED_IDS_PATTERN_KEY = "Nop.pres.related";

        /// <summary>
        /// Key for default product picture caching (all pictures)
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : picture size
        /// {2} : isAssociatedProduct?
        /// {3} : language ID ("alt" and "title" can depend on localized product name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string PRODUCT_DEFAULTPICTURE_MODEL_KEY = "Nop.pres.product.detailspictures-Bswebapi-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string PRODUCT_DEFAULTPICTURE_PATTERN_KEY = "Nop.pres.product.detailspictures";

        /// <summary>
        /// Key for product picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized product name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string PRODUCT_DETAILS_PICTURES_MODEL_KEY = "Nop.pres.product.picture-Bswebapi-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string PRODUCT_DETAILS_TPICTURES_PATTERN_KEY = "Nop.pres.product.picture";

        /// <summary>
        /// Key for product reviews caching
        /// </summary>
        /// <remarks>
        /// {0} : product id
        /// {1} : current store ID
        /// </remarks>
        public const string PRODUCT_REVIEWS_MODEL_KEY = "Nop.pres.product.reviews-Bswebapi-{0}-{1}";
        public const string PRODUCT_REVIEWS_PATTERN_KEY = "Nop.pres.product.reviews";
        public const string PRODUCT_REVIEWS_PATTERN_KEY_BY_ID = "Nop.pres.product.reviews-{0}-";

        /// <summary>
        /// Key for product attribute picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : is connection SSL secured?
        /// {2} : current store ID
        /// </remarks>
        public const string PRODUCTATTRIBUTE_PICTURE_MODEL_KEY = "Nop.pres.productattribute.picture-Bswebapi-{0}-{1}-{2}";
        public const string PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY = "Nop.pres.productattribute.picture";

        /// <summary>
        /// Key for product attribute picture caching on the product details page
        /// </summary>
        /// <remarks>
        /// {0} : picture id
        /// {1} : is connection SSL secured?
        /// {2} : current store ID
        /// </remarks>
        public const string PRODUCTATTRIBUTE_IMAGESQUARE_PICTURE_MODEL_KEY = "Nop.pres.productattribute.imagesquare.picture-{0}-{1}-{2}";
        public const string PRODUCTATTRIBUTE_IMAGESQUARE_PICTURE_PATTERN_KEY = "Nop.pres.productattribute.imagesquare.picture";

        /// <summary>
        /// Key for category picture caching
        /// </summary>
        /// <remarks>
        /// {0} : category id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized category name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string CATEGORY_PICTURE_MODEL_KEY = "Nop.pres.category.picture-Bswebapi-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string CATEGORY_PICTURE_PATTERN_KEY = "Nop.pres.category.picture";

        /// <summary>
        /// Key for manufacturer picture caching
        /// </summary>
        /// <remarks>
        /// {0} : manufacturer id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized manufacturer name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string MANUFACTURER_PICTURE_MODEL_KEY = "Nop.pres.manufacturer.picture-Bswebapi-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string MANUFACTURER_PICTURE_PATTERN_KEY = "Nop.pres.manufacturer.picture";

        /// <summary>
        /// Key for vendor picture caching
        /// </summary>
        /// <remarks>
        /// {0} : vendor id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized category name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string VENDOR_PICTURE_MODEL_KEY = "Nop.pres.vendor.picture-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string VENDOR_PICTURE_PATTERN_KEY = "Nop.pres.vendor.picture";

        /// <summary>
        /// Key for cart picture caching
        /// </summary>
        /// <remarks>
        /// {0} : shopping cart item id
        /// P.S. we could cache by product ID. it could increase performance.
        /// but it won't work for product attributes with custom images
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized product name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public const string CART_PICTURE_MODEL_KEY = "Nop.pres.cart.picture-Bswebapi-{0}-{1}-{2}-{3}-{4}-{5}";
        public const string CART_PICTURE_PATTERN_KEY = "Nop.pres.cart.picture";

        /// <summary>
        /// Key for home page polls
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// </remarks>
        public const string HOMEPAGE_POLLS_MODEL_KEY = "Nop.pres.poll.homepage-{0}";
        /// <summary>
        /// Key for polls by system name
        /// </summary>
        /// <remarks>
        /// {0} : poll system name
        /// {1} : language ID
        /// </remarks>
        public const string POLL_BY_SYSTEMNAME__MODEL_KEY = "Nop.pres.poll.systemname-Bswebapi-{0}-{1}";
        public const string POLLS_PATTERN_KEY = "Nop.pres.poll";

        /// <summary>
        /// Key for blog tag list model
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public const string BLOG_TAGS_MODEL_KEY = "Nop.pres.blog.tags-{0}-{1}";
        /// <summary>
        /// Key for blog archive (years, months) block model
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public const string BLOG_MONTHS_MODEL_KEY = "Nop.pres.blog.months-{0}-{1}";
        public const string BLOG_PATTERN_KEY = "Nop.pres.blog";
        /// <summary>
        /// Key for number of blog comments
        /// </summary>
        /// <remarks>
        /// {0} : blog post ID
        /// {1} : store ID
        /// {2} : are only approved comments?
        /// </remarks>
        public const string BLOG_COMMENTS_NUMBER_KEY = "Nop.pres.blog.comments.number-Bswebapi-{0}-{1}-{2}";
        public const string BLOG_COMMENTS_PATTERN_KEY = "Nop.pres.blog.comments";

        /// <summary>
        /// Key for home page news
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {1} : current store ID
        /// </remarks>
        public const string HOMEPAGE_NEWSMODEL_KEY = "Nop.pres.news.homepage-Bswebapi-{0}-{1}";
        public const string NEWS_PATTERN_KEY = "Nop.pres.news";

        /// <summary>
        /// Key for states by country id
        /// </summary>
        /// <remarks>
        /// {0} : country ID
        /// {0} : "empty" or "select" item
        /// {0} : language ID
        /// </remarks>
        public const string STATEPROVINCES_BY_COUNTRY_MODEL_KEY = "Nop.pres.stateprovinces.bycountry-Bswebapi-{0}-{1}-{2}";
        public const string STATEPROVINCES_PATTERN_KEY = "Nop.pres.stateprovinces";

        /// <summary>
        /// Key for cities by state id
        /// </summary>
        /// <remarks>
        /// {0} : state ID
        /// {0} : "empty" or "select" item
        /// {0} : language ID
        /// </remarks>
        public const string CITIES_BY_STATE_MODEL_KEY = "Nop.pres.cities.bystate-Bswebapi-{0}-{1}-{2}";
        public const string CITIES_PATTERN_KEY = "Nop.pres.cities";

        /// <summary>
        /// Key for available languages
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// </remarks>
        public const string AVAILABLE_LANGUAGES_MODEL_KEY = "Nop.pres.languages.all-{0}";
        public const string AVAILABLE_LANGUAGES_PATTERN_KEY = "Nop.pres.languages";

        /// <summary>
        /// Key for available currencies
        /// </summary>
        /// <remarks>
        /// {0} : language ID
        /// {0} : current store ID
        /// </remarks>
        public const string AVAILABLE_CURRENCIES_MODEL_KEY = "Nop.pres.currencies.all-{0}-{1}";
        public const string AVAILABLE_CURRENCIES_PATTERN_KEY = "Nop.pres.currencies";

        /// <summary>
        /// Key for caching of a value indicating whether we have checkout attributes
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : true - all attributes, false - only shippable attributes
        /// </remarks>
        public const string CHECKOUTATTRIBUTES_EXIST_KEY = "Nop.pres.checkoutattributes.exist-Bswebapi-{0}-{1}";
        public const string CHECKOUTATTRIBUTES_PATTERN_KEY = "Nop.pres.checkoutattributes";

        /// <summary>
        /// Key for sitemap on the sitemap page
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string SITEMAP_PAGE_MODEL_KEY = "Nop.pres.sitemap.page-{0}-{1}-{2}";
        /// <summary>
        /// Key for sitemap on the sitemap SEO page
        /// </summary>
        /// <remarks>
        /// {0} : language id
        /// {1} : roles of the current user
        /// {2} : current store ID
        /// </remarks>
        public const string SITEMAP_SEO_MODEL_KEY = "Nop.pres.sitemap.seo-{0}-{1}-{2}";
        public const string SITEMAP_PATTERN_KEY = "Nop.pres.sitemap";

        /// <summary>
        /// Key for widget info
        /// </summary>
        /// <remarks>
        /// {0} : current store ID
        /// {1} : widget zone
        /// </remarks>
        public const string WIDGET_MODEL_KEY = "Nop.pres.widget-{0}-{1}";
        public const string WIDGET_PATTERN_KEY = "Nop.pres.widget";

        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IStaticCacheManager _cacheManager;

        #endregion

        public ModelCacheEventConsumer(CatalogSettings catalogSettings, IStaticCacheManager cacheManager)
        {
            _catalogSettings = catalogSettings;
            _cacheManager = cacheManager;
        }

        //languages
        public void HandleEvent(EntityInsertedEvent<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_LANGUAGES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_LANGUAGES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<Language> eventMessage)
        {
            //clear all localizable models
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_LANGUAGES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }

        //currencies
        public void HandleEvent(EntityInsertedEvent<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<Currency> eventMessage)
        {
            _cacheManager.RemoveByPattern(AVAILABLE_CURRENCIES_PATTERN_KEY);
        }

        public void HandleEvent(EntityUpdatedEvent<Setting> eventMessage)
        {
            //clear models which depend on settings
            _cacheManager.RemoveByPattern(PRODUCTTAG_POPULAR_PATTERN_KEY); //depends on CatalogSettings.NumberOfProductTags
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY); //depends on CatalogSettings.ManufacturersBlockItemsToDisplay
            _cacheManager.RemoveByPattern(VENDOR_NAVIGATION_PATTERN_KEY); //depends on VendorSettings.VendorBlockItemsToDisplay
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY); //depends on CatalogSettings.ShowCategoryProductNumber and CatalogSettings.ShowCategoryProductNumberIncludingSubcategories
            _cacheManager.RemoveByPattern(CATEGORY_MENU_PATTERN_KEY); //depends on CatalogSettings.ShowCategoryProductNumber and CatalogSettings.ShowCategoryProductNumberIncludingSubcategories
            _cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY); //depends on CatalogSettings.ShowCategoryProductNumberIncludingSubcategories
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY); //depends on CatalogSettings.NumberOfBestsellersOnHomepage
            _cacheManager.RemoveByPattern(PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY); //depends on CatalogSettings.ProductsAlsoPurchasedNumber
            _cacheManager.RemoveByPattern(PRODUCTS_RELATED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(BLOG_PATTERN_KEY); //depends on BlogSettings.NumberOfTags
            _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY); //depends on NewsSettings.MainPageNewsCount
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY); //depends on distinct sitemap settings
            _cacheManager.RemoveByPattern(WIDGET_PATTERN_KEY); //depends on WidgetSettings and certain settings of widgets

        }

        //vendors
        public void HandleEvent(EntityInsertedEvent<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPattern(VENDOR_NAVIGATION_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPattern(VENDOR_NAVIGATION_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<Vendor> eventMessage)
        {
            _cacheManager.RemoveByPattern(VENDOR_NAVIGATION_PATTERN_KEY);
        }

        //manufacturers
        public void HandleEvent(EntityInsertedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<Manufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }

        //product manufacturers
        public void HandleEvent(EntityInsertedEvent<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_HAS_FEATURED_PRODUCTS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_HAS_FEATURED_PRODUCTS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<ProductManufacturer> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_MANUFACTURERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_HAS_FEATURED_PRODUCTS_PATTERN_KEY);
        }

        //categories
        public void HandleEvent(EntityInsertedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_MENU_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_SUBCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HOMEPAGE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_MENU_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_SUBCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HOMEPAGE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<Category> eventMessage)
        {
            _cacheManager.RemoveByPattern(SEARCH_CATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_MENU_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_CHILD_IDENTIFIERS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_SUBCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HOMEPAGE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }

        //product categories
        public void HandleEvent(EntityInsertedEvent<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_MENU_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HAS_FEATURED_PRODUCTS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_MENU_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HAS_FEATURED_PRODUCTS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<ProductCategory> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_MENU_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HAS_FEATURED_PRODUCTS_PATTERN_KEY);
        }

        //products
        public void HandleEvent(EntityInsertedEvent<Product> eventMessage)
        {
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<Product> eventMessage)
        {
            //_cacheManager.RemoveByPattern(PRODUCT_BREADCRUMB_PATTERN_KEY);
            //_cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            //_cacheManager.RemoveByPattern(CATEGORY_MENU_PATTERN_KEY);
            //_cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_RELATED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<Product> eventMessage)
        {
            //_cacheManager.RemoveByPattern(CATEGORY_NAVIGATION_PATTERN_KEY);
            //_cacheManager.RemoveByPattern(CATEGORY_MENU_PATTERN_KEY);
            //_cacheManager.RemoveByPattern(CATEGORY_NUMBER_OF_PRODUCTS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_RELATED_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }

        //product tags
        public void HandleEvent(EntityInsertedEvent<ProductTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTTAG_POPULAR_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTTAG_BY_PRODUCT_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<ProductTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTTAG_POPULAR_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTTAG_BY_PRODUCT_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<ProductTag> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTTAG_POPULAR_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTTAG_BY_PRODUCT_PATTERN_KEY);
        }

        //related products
        public void HandleEvent(EntityInsertedEvent<RelatedProduct> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTS_RELATED_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<RelatedProduct> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTS_RELATED_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<RelatedProduct> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTS_RELATED_IDS_PATTERN_KEY);
        }

        //specification attributes
        public void HandleEvent(EntityUpdatedEvent<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<SpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
        }

        //specification attribute options
        public void HandleEvent(EntityUpdatedEvent<SpecificationAttributeOption> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<SpecificationAttributeOption> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
        }

        //Product specification attribute
        public void HandleEvent(EntityInsertedEvent<ProductSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<ProductSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<ProductSpecificationAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_SPECS_PATTERN_KEY);
        }

        //Product attributes
        public void HandleEvent(EntityDeletedEvent<ProductAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_HAS_PRODUCT_ATTRIBUTES_PATTERN_KEY);
        }
        //Product attributes
        public void HandleEvent(EntityInsertedEvent<ProductAttributeMapping> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_HAS_PRODUCT_ATTRIBUTES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<ProductAttributeMapping> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_HAS_PRODUCT_ATTRIBUTES_PATTERN_KEY);
        }
        //Product attributes
        public void HandleEvent(EntityUpdatedEvent<ProductAttributeValue> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
        }

        //Topics
        public void HandleEvent(EntityInsertedEvent<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<Topic> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_PATTERN_KEY);
            _cacheManager.RemoveByPattern(SITEMAP_PATTERN_KEY);
        }

        //Orders
        public void HandleEvent(EntityInsertedEvent<Order> eventMessage)
        {
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<Order> eventMessage)
        {
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<Order> eventMessage)
        {
            _cacheManager.RemoveByPattern(HOMEPAGE_BESTSELLERS_IDS_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTS_ALSO_PURCHASED_IDS_PATTERN_KEY);
        }


        //Pictures
        public void HandleEvent(EntityInsertedEvent<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_DEFAULTPICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_DETAILS_TPICTURES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HOMEPAGE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_SUBCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_DEFAULTPICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_DETAILS_TPICTURES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HOMEPAGE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_SUBCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<Picture> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_DEFAULTPICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_DETAILS_TPICTURES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_HOMEPAGE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CATEGORY_SUBCATEGORIES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(MANUFACTURER_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }

        //Product picture mappings
        public void HandleEvent(EntityInsertedEvent<ProductPicture> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_DEFAULTPICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_DETAILS_TPICTURES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<ProductPicture> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_DEFAULTPICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_DETAILS_TPICTURES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<ProductPicture> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_DEFAULTPICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCT_DETAILS_TPICTURES_PATTERN_KEY);
            _cacheManager.RemoveByPattern(PRODUCTATTRIBUTE_PICTURE_PATTERN_KEY);
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }

        //Polls
        public void HandleEvent(EntityInsertedEvent<Poll> eventMessage)
        {
            _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<Poll> eventMessage)
        {
            _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<Poll> eventMessage)
        {
            _cacheManager.RemoveByPattern(POLLS_PATTERN_KEY);
        }

        //Blog posts
        public void HandleEvent(EntityInsertedEvent<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPattern(BLOG_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPattern(BLOG_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<BlogPost> eventMessage)
        {
            _cacheManager.RemoveByPattern(BLOG_PATTERN_KEY);
        }

        //News items
        public void HandleEvent(EntityInsertedEvent<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<NewsItem> eventMessage)
        {
            _cacheManager.RemoveByPattern(NEWS_PATTERN_KEY);
        }

        //State/province
        public void HandleEvent(EntityInsertedEvent<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<StateProvince> eventMessage)
        {
            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }

        //templates
        public void HandleEvent(EntityInsertedEvent<CategoryTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORY_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<CategoryTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORY_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<CategoryTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(CATEGORY_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityInsertedEvent<ManufacturerTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<ManufacturerTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<ManufacturerTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(MANUFACTURER_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityInsertedEvent<ProductTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<ProductTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<ProductTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(PRODUCT_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityInsertedEvent<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_TEMPLATE_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<TopicTemplate> eventMessage)
        {
            _cacheManager.RemoveByPattern(TOPIC_TEMPLATE_PATTERN_KEY);
        }

        //checkout attributes
        public void HandleEvent(EntityInsertedEvent<CheckoutAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdatedEvent<CheckoutAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeletedEvent<CheckoutAttribute> eventMessage)
        {
            _cacheManager.RemoveByPattern(CHECKOUTATTRIBUTES_PATTERN_KEY);
        }

        //shopping cart items
        public void HandleEvent(EntityUpdatedEvent<ShoppingCartItem> eventMessage)
        {
            _cacheManager.RemoveByPattern(CART_PICTURE_PATTERN_KEY);
        }

    }
}

