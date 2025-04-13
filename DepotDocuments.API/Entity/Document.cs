using DepotDocuments.API.Shared;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DepotDocuments.API.Entities

{
public class Document
{
[BsonId]
    public ObjectId Id { get; set; }
    public string Url { get; set; }
    public TypeDocument Type { get; set; }
    public string TextExtrait { get; set; }
    public DateTime DateAjout { get; set; }
}}
