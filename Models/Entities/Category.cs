using MongoDB.Bson.Serialization.Attributes;

namespace Models.Entities
{
    public class Category
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }
    }
}