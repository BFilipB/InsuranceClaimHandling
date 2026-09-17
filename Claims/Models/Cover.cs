using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Models
{
    /// <summary>
    /// Represents an insurance cover (policy) for a specific object type over a date range.
    /// </summary>
    public class Cover
    {
        [BsonId]
        public string Id { get; set; } = string.Empty;

        [BsonElement("startDate")]
        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime StartDate { get; set; }

        [BsonElement("endDate")]
        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime EndDate { get; set; }

        [BsonElement("claimType")]
        public CoverType Type { get; set; }

        [BsonElement("premium")]
        public decimal Premium { get; set; }
    }

    public enum CoverType
    {
        Yacht = 0,
        PassengerShip = 1,
        ContainerShip = 2,
        BulkCarrier = 3,
        Tanker = 4
    }
}
