using System.Text.Json.Serialization;

namespace AuthSolution.Model;

public class User
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; } = "email@teste.com";

    [JsonPropertyName("password")]
    public string Password { get; set; } = Random.Shared.Next().ToString();

    [JsonPropertyName("roles")]
    public string[] Role { get; set; }

}