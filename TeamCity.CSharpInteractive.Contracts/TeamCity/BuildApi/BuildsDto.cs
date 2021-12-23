// ReSharper disable CheckNamespace
namespace TeamCity.BuildApi
{
    using System.Text.Json.Serialization;

    public record BuildsDto
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}