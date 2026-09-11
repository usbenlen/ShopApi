using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shop.Application.DTOs.ProductFeedbackDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Infrastructure.Configuration;
using Shop.Infrastructure.Data.Mongo;

namespace Shop.Infrastructure.Services;

public class MongoProductFeedbackService : IProductFeedbackService
{
    private readonly IMongoCollection<ProductFeedbackDocument> _collection;

    public MongoProductFeedbackService(IOptions<MongoDbSettings> settings)
    {
        var mongoSettings = settings.Value;

        if (string.IsNullOrWhiteSpace(mongoSettings.ConnectionString))
            throw new InvalidOperationException("MongoDb:ConnectionString is not configured");

        if (string.IsNullOrWhiteSpace(mongoSettings.DatabaseName))
            throw new InvalidOperationException("MongoDb:DatabaseName is not configured");

        var client = new MongoClient(mongoSettings.ConnectionString);
        var database = client.GetDatabase(mongoSettings.DatabaseName);
        _collection = database.GetCollection<ProductFeedbackDocument>("ProductFeedback");
    }

    public async Task CreateAsync(int productId, Guid userId, ProductFeedbackCreateDTO dto, CancellationToken cancellationToken = default)
    {
        var document = new ProductFeedbackDocument
        {
            ProductId = productId,
            UserId = userId,
            Type = dto.Type,
            Message = dto.Message,
            Rating = dto.Rating,
            CreatedAt = DateTime.UtcNow
        };

        await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);
    }
}
