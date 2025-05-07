namespace TODOListClient.Models
{
    public class TaskItem
    {
        public int Id { get; set; } // Уникальный идентификатор задачи
        public string Title { get; set; } // Название задачи
        public int? CategoryId { get; set; } // Идентификатор категории (может быть null)
        public Category Category { get; set; } // Связанная категория
    }
}