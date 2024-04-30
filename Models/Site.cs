using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace altsite.Models;

public class Site
{
    [BsonId]
    public ObjectId _id { get; set; } = ObjectId.GenerateNewId();
    
    [BsonElement("originalSiteURL")]
    public string OriginalSiteURL { get; set; }
    [BsonElement("lastRedirection")]
    public string LastRedirection { get; set; }
    [BsonElement("redirectableSites")]
    public List<string> RedirectableSites { get; set; }
}