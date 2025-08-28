using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace TODOListServer.Models
{
    public class TaskItem
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [JsonPropertyName("category_id")]
        public int? CategoryId { get; set; }

        public Category? Category { get; set; }
    }
}