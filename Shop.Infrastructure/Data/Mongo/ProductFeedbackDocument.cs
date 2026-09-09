using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Shop.Domain.Enums;

namespace Shop.Infrastructure.Data.Mongo;

public class ProductFeedbackDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    public int ProductId { get; set; }

    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid UserId { get; set; }

    [BsonElement("type")]
    [BsonRepresentation(BsonType.String)]
    public ProductFeedbackType Type { get; set; }

    public string Message { get; set; } = string.Empty;
    public double? Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}
