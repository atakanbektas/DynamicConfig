using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Config.Domain
{
    public sealed class ConfigItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string Id { get; set; } = default!;
        public string ApplicationName { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Value { get; set; } = default!;
        public bool IsActive { get; set; } = true;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
