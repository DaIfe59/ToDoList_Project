using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace TODOListClient.Models
{
    public class Category
    {
        [JsonPropertyName("id")]
        public int Id { get; set; } // Уникальный идентификатор категории
        [JsonPropertyName("name")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } // Название категории
    }
}