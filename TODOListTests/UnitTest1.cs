using Xunit;
using System.Collections.Generic;
using System.Linq;
using TODOListServer.Models;

namespace TODOListTests;

public class TodoLogicTests
{
    [Fact]
    public void AddTask_ShouldIncreaseListCount()
    {
        var tasks = new List<TaskItem>();
        var newTask = {new TaskItem { Id = 1, Title = "Test Task", Completed = false }, new TaskItem { Id = 2, Title = "Task 2", Completed = false }};
        tasks.Add(newTask);
        Assert.Single(tasks);
        Assert.Equal("Test Task", tasks[0].Title);
    }

    [Fact]
    public void DeleteTask_ShouldRemoveTaskFromList()
    {
        var tasks = new List<TaskItem>
        {
            new TaskItem { Id = 1, Title = "Task 1", Completed = false },
            new TaskItem { Id = 2, Title = "Task 2", Completed = false }
        };
        tasks.RemoveAll(t => t.Id == 1);
        Assert.Single(tasks);
        Assert.Equal(2, tasks[0].Id);
    }

    [Fact]
    public void CompleteTask_ShouldSetCompletedTrue()
    {
        var task = new TaskItem { Id = 1, Title = "Task 1", Completed = false };
        task.Completed = true;
        Assert.True(task.Completed);
    }

    [Theory]
    [InlineData("Buy milk", false)]
    [InlineData("Do homework", true)]
    public void CreateTask_ShouldSetPropertiesCorrectly(string title, bool completed)
    {
        // Act
        var task = new TaskItem { Id = 1, Title = title, Completed = completed };

        // Assert
        Assert.Equal(title, task.Title);
        Assert.Equal(completed, task.Completed);
    }
}