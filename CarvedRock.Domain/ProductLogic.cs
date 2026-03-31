using System.Diagnostics;
using CarvedRock.Core;
using CarvedRock.Data;
using CarvedRock.Data.Entities;
using CarvedRock.Domain.Mapping;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CarvedRock.Domain;

public class ProductLogic(ICarvedRockRepository repo,
            IValidator<NewProductModel> newProductValidator,
            ActivitySource activitySource,
            ILogger<ProductLogic> logger) : IProductLogic
{
    public async Task<IEnumerable<Product>> GetProductsForCategoryAsync(string category)
    {
        using var scope = logger.BeginScope(
            new Dictionary<string, object> { ["category"] = category });

        FastLog.CallingRepository(logger); // high-performance situations
        //logger.LogInformation("Calling repository.");
        return await repo.GetProductsAsync(category);
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        // custom trace example
        // basics: https://opentelemetry.io/docs/zero-code/dotnet/custom/#traces
        // more info: https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing-instrumentation-walkthroughs

        // activitysource added as singleton in servicedefaults
        // keep in mind that http calls, grpc calls, and aspire integrations should trace out of the box
        //   e.g. redis, mongo, sqlserver, file storage, lots more
        //var activitySource = new ActivitySource("CarvedRock.Inventory");
        using (var activity = activitySource.StartActivity("INVENTORY SingleProduct"))
        {
            activity?.SetTag("product", id.ToString());                                    
            await Task.Delay(250); // quarter of a second -- do some logic that's interesting
        } // activity will stop when using statement completes

        return await repo.GetProductByIdAsync(id);
    }

    public async Task<ProductModel> CreateProductAsync(NewProductModel newProduct)
    {
        await newProductValidator.ValidateAndThrowAsync(newProduct);

        var productMapper = new ProductMapper();

        var productToCreate = productMapper.NewProductModelToProduct(newProduct);
        var createdProduct = await repo.CreateProductAsync(productToCreate);
        return productMapper.ProductToProductModel(createdProduct);
    }

    public async Task<ProductModel> UpdateProductAsync(int id, NewProductModel updatedProduct)
    {
        var productMapper = new ProductMapper();
        var productToUpdate = productMapper.NewProductModelToProduct(updatedProduct);
        productToUpdate.Id = id;

        var updatedProductEntity = await repo.UpdateProductAsync(productToUpdate);
        return productMapper.ProductToProductModel(updatedProductEntity);
    }

    public async Task DeleteProductAsync(int id)
    {
        await repo.DeleteProductAsync(id);
    }
}