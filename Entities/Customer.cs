using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HotelCustomerAPI.Entities
{
    public class Customer
    {
        [BsonRepresentation(BsonType.ObjectId), BsonId]
        public string? Id { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }

        public int BookingId { get; set; }
    }
}
