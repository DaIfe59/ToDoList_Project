namespace TODOListServer.Models
{
    public class TaskItem
    {
        public int Id { get; set; } // Уникальный идентификатор
        public string Title { get; set; } // Название задачи
        public int? CategoryId { get; set; } // Идентификатор категории
        public Category Category { get; set; } // Связанная категория

    }
}