using AuthJwtSolution.Model;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace AuthJwtSolution.Context;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(User))]
public partial class UserContext : JsonSerializerContext
{

}
