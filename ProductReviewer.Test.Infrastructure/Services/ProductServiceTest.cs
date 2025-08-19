using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProductReviewer.Application.Common.Dtos;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Domain.Entities;
using ProductReviewer.Domain.Enums;
using ProductReviewer.Infrastructure.Data;
using ProductReviewer.Infrastructure.Services;

namespace ProductReviewer.Test.Infrastructure.Services
{
    public class ProductServiceTest
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Mock<ICurrentUser> _currentUser;
        private readonly ProductService _productService;

        public ProductServiceTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDbContext(options);
            _dbContext.Database.EnsureCreated();

            _currentUser = new Mock<ICurrentUser>();
            _currentUser.Setup(x => x.UserId).Returns("test-user-id");

            _productService = new ProductService(_dbContext, _currentUser.Object);
        }

        [Trait("Category", "Infrastructure")]
        [Fact]
        public async Task CreateProduct_ReturnsSuccess()
        {
            var productDto = new CreateProductDto
            {
                Name = "Test Product",
                Description = "Test Description",
                SKU = "TEST-SKU",
                ProductType = "Electronics"
            };

            var result = await _productService.CreateProduct(productDto);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("Added Product successfully.");

            var savedProduct = await _dbContext.Products.FirstOrDefaultAsync();
            savedProduct.Should().NotBeNull();
            savedProduct!.Name.Should().Be(productDto.Name);
            savedProduct.Description.Should().Be(productDto.Description);
            savedProduct.SKU.Should().Be(productDto.SKU);
            savedProduct.ProductType.Should().Be(productDto.ProductType);
        }

        [Trait("Category", "Infrastructure")]
        [Fact]
        public async Task AddProductReview_ReturnsSuccess()
        {
            var productReview = new ReviewProductDto
            {
                ProductId = 1,
                Rating = 4.3,
                Comment = "This is a great product!"
            };
            var result = await _productService.AddProductReview(productReview);
            result.IsSuccess.Should().BeTrue();

            result.Value.Should().Be("Added Review Successfully.");

            var savedReview = await _dbContext.Reviews.FirstOrDefaultAsync();
            savedReview.Should().NotBeNull();
            savedReview!.ProductId.Should().Be(productReview.ProductId);
            savedReview.Rating.Should().Be(productReview.Rating);
            savedReview.Comment.Should().Be(productReview.Comment);
            savedReview.UserId.Should().Be(_currentUser.Object.UserId);
        }

        [Trait("Category", "Infrastructure")]
        [Fact]
        public async Task CheckUserExists_ReturnsSuccess()
        {
            int productid = 1;
            var productReview = new Review
            {
                ProductId = 1,
                Rating = 4.3,
                Comment = "This is a great product!",
                UserId = _currentUser.Object.UserId!
            };

            await _dbContext.Reviews.AddAsync(productReview);
            await _dbContext.SaveChangesAsync();

            var result = await _productService.CheckUserReviewExists(productid);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();
        }

        [Trait("Category", "Infrastructure")]
        [Fact]
        public async Task GetByid_ReturnsProduct_WhenValidId()
        {
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                SKU = "TEST-SKU",
                ProductType = "Electronics",
                Reviews = new List<Review>
                {
                    new Review { Rating = 4.3, Comment = "Great product", UserId = _currentUser.Object.UserId! }
                }
            };

            _dbContext.Products.Add(product);

            await _dbContext.SaveChangesAsync();
            var result = await _productService.GetById(product.Id);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be("Test Product");
            result.Value.AverageRating.Should().Be(4.3);
        }

        [Trait("Category", "Infrastructure")]
        [Fact]
        public async Task GetByid_ReturnsFailure_WhenInvalidId()
        {
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                SKU = "TEST-SKU",
                ProductType = "Electronics",
                Reviews = new List<Review>
                {
                    new Review { Rating = 4.3, Comment = "Great product", UserId = _currentUser.Object.UserId! }
                }
            };

            _dbContext.Products.Add(product);

            await _dbContext.SaveChangesAsync();
            var result = await _productService.GetById(product.Id + 1);

            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors[0].Message.Should().Be("Product not found.");
        }


        [Trait("Category", "Infrastructure")]
        [Fact]
        public async Task GetAllAsync_ReturnsWorstCategory_WhenWorstReviews()
        {
            _dbContext.Products.Add(new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                SKU = "TEST-SKU",
                ProductType = "Electronics",
                Reviews = new List<Review>
                {
                    new Review { Rating = 3.0, Comment = "Very bad product", UserId = _currentUser.Object.UserId! },
                    new Review { Rating = 4.9, Comment = "Not good", UserId = _currentUser.Object.UserId! }
                }
            });
            _dbContext.Products.Add(new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                SKU = "TEST-SKU",
                ProductType = "Electronics",
                Reviews = new List<Review>
                {
                    new Review { Rating = 1.0, Comment = "Very bad product", UserId = _currentUser.Object.UserId! },
                    new Review { Rating = 1.9, Comment = "Not good", UserId = _currentUser.Object.UserId! }
                }
            });

            await _dbContext.SaveChangesAsync();

            var result = await _productService.GetAllAsync(1, 10, ProductCategory.Worst);

            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().ContainSingle();

            var product = result.Value.Items.First();
            product.AverageRating.Should().BeLessThan(2.0);
            product.Category.Should().Be(ProductCategory.Worst);
        }

        [Trait("Category", "Infrastructure")]
        [Fact]
        public async Task GetAllAsync_ReturnsBadCategory_WhenBadReviews()
        {
            _dbContext.Products.Add(new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                SKU = "TEST-SKU",
                ProductType = "Electronics",
                Reviews = new List<Review>
                {
                    new Review { Rating = 4.3, Comment = "Good product 1", UserId = _currentUser.Object.UserId! },
                    new Review { Rating = 4.5, Comment = "Good product 2", UserId = _currentUser.Object.UserId! }
                }
            });

            _dbContext.Products.Add(new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                SKU = "TEST-SKU",
                ProductType = "Electronics",
                Reviews = new List<Review>
                {
                    new Review { Rating = 2.3, Comment = "Bad product 1", UserId = _currentUser.Object.UserId! },
                    new Review { Rating = 3.5, Comment = "Bad product 2", UserId = _currentUser.Object.UserId! }
                }
            });

            await _dbContext.SaveChangesAsync();

            var result = await _productService.GetAllAsync(1, 10, ProductCategory.Bad);

            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().ContainSingle();

            var product = result.Value.Items.First();
            product.AverageRating.Should().BeGreaterThanOrEqualTo(2.0).And.BeLessThan(4.0);
            product.Category.Should().Be(ProductCategory.Bad);
        }


        [Trait("Category", "Infrastructure")]
        [Fact]
        public async Task GetAllAsync_ReturnsGoodCategory_WhenGoodReviews()
        {
            _dbContext.Products.Add(new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                SKU = "TEST-SKU",
                ProductType = "Electronics",
                Reviews = new List<Review>
                {
                    new Review { Rating = 4.3, Comment = "Good product 1", UserId = _currentUser.Object.UserId! },
                    new Review { Rating = 4.5, Comment = "Good product 2", UserId = _currentUser.Object.UserId! }
                }
            });

            _dbContext.Products.Add(new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                SKU = "TEST-SKU",
                ProductType = "Electronics",
                Reviews = new List<Review>
                {
                    new Review { Rating = 2.3, Comment = "Its ok", UserId = _currentUser.Object.UserId! },
                    new Review { Rating = 1.5, Comment = "Bad", UserId = _currentUser.Object.UserId! }
                }
            });

            await _dbContext.SaveChangesAsync();

            var result = await _productService.GetAllAsync(1, 10, ProductCategory.Good);

            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().ContainSingle();

            var product = result.Value.Items.First();
            product.AverageRating.Should().BeGreaterThanOrEqualTo(4.0);
            product.Category.Should().Be(ProductCategory.Good);
        }


        [Trait("Category", "Infrastructure")]
        [Fact]
        public async Task GetAllAsync_ReturnsAllProduct()
        {
            _dbContext.Products.Add(new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                SKU = "TEST-SKU",
                ProductType = "Electronics",
                Reviews = new List<Review>
                {
                    new Review { Rating = 4.3, Comment = "Good product 1", UserId = _currentUser.Object.UserId! },
                    new Review { Rating = 4.5, Comment = "Good product 2", UserId = _currentUser.Object.UserId! }
                }
            });

            _dbContext.Products.Add(new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                SKU = "TEST-SKU",
                ProductType = "Electronics",
                Reviews = new List<Review>
                {
                    new Review { Rating = 3.3, Comment = "Ok product 1", UserId = _currentUser.Object.UserId! },
                    new Review { Rating = 2.5, Comment = "Ok product 2", UserId = _currentUser.Object.UserId! }
                }
            });

            _dbContext.Products.Add(new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                SKU = "TEST-SKU",
                ProductType = "Electronics",
                Reviews = new List<Review>
                {
                    new Review { Rating = 1.9, Comment = "Its ok", UserId = _currentUser.Object.UserId! },
                    new Review { Rating = 1.5, Comment = "Bad", UserId = _currentUser.Object.UserId! }
                }
            });

            await _dbContext.SaveChangesAsync();

            var result = await _productService.GetAllAsync(1, 10);

            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(3);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
