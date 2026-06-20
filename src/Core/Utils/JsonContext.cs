using System.Text.Json.Serialization;
using Core;

[JsonSerializable(typeof(Node))]
public partial class AppJsonContext : JsonSerializerContext { }
