using System.Text.Json.Serialization;

namespace BlogApi.Features.Profiles
{
    public class Profile
    {
        public string? Username { get; set; }

        public string? Bio { get; set; }

        public string? Image { get; set; }

        [JsonPropertyName("following")]
        public bool IsFollowed { get; set; }
    }
}
