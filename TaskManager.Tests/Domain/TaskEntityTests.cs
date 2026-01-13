using FluentAssertions;
using TaskManager.Domain;
using Xunit;
using TaskStatus = TaskManager.Domain.TaskStatus;

namespace TaskManager.Tests.Domain;

public class TaskEntityTests
{
    [Fact]
    public void Task_WithValidData_ShouldBeValid()
    {
        var task = new TaskManager.Domain.Task
        {
            Titulo = "Tarefa de teste",
            Descricao = "Descrição da tarefa",
            DataCriacao = DateTime.Now,
            Status = TaskStatus.Pendente
        };

        var isValid = task.IsValid(out string errorMessage);

        isValid.Should().BeTrue();
        errorMessage.Should().BeEmpty();
    }

    [Fact]
    public void Task_WithCompletionDateBeforeCreationDate_ShouldBeInvalid()
    {
        var task = new TaskManager.Domain.Task
        {
            Titulo = "Tarefa de teste",
            DataCriacao = DateTime.Now,
            DataConclusao = DateTime.Now.AddDays(-1),
            Status = TaskStatus.Concluida
        };

        var isValid = task.IsValid(out var errorMessage);

        isValid.Should().BeFalse();
        errorMessage.Should().Be("A data e hora de conclusão não pode ser anterior à data e hora de criação");
    }

    [Fact]
    public void Task_WithCompletionTimeBeforeCreationTime_ShouldBeInvalid()
    {
        var creationDate = new DateTime(2026, 1, 13, 14, 30, 0);
        var completionDate = new DateTime(2026, 1, 13, 14, 15, 0);
        
        var task = new TaskManager.Domain.Task
        {
            Titulo = "Tarefa de teste",
            DataCriacao = creationDate,
            DataConclusao = completionDate,
            Status = TaskStatus.Concluida
        };

        var isValid = task.IsValid(out var errorMessage);

        isValid.Should().BeFalse();
        errorMessage.Should().Be("A data e hora de conclusão não pode ser anterior à data e hora de criação");
    }

    [Fact]
    public void Task_WithCompletionTimeAfterCreationTime_ShouldBeValid()
    {
        var creationDate = new DateTime(2026, 1, 13, 14, 30, 0); 
        var completionDate = new DateTime(2026, 1, 13, 14, 45, 0);
        
        var task = new TaskManager.Domain.Task
        {
            Titulo = "Tarefa de teste",
            DataCriacao = creationDate,
            DataConclusao = completionDate,
            Status = TaskStatus.Concluida
        };

        var isValid = task.IsValid(out var errorMessage);

        isValid.Should().BeTrue();
        errorMessage.Should().BeEmpty();
    }

    [Fact]
    public void Task_WithCompletionDateSameAsCreationDate_ShouldBeValid()
    {
        var dateTime = new DateTime(2026, 1, 13, 14, 30, 0);
        
        var task = new TaskManager.Domain.Task
        {
            Titulo = "Tarefa de teste",
            DataCriacao = dateTime,
            DataConclusao = dateTime,
            Status = TaskStatus.Concluida
        };

        var isValid = task.IsValid(out var errorMessage);

        isValid.Should().BeTrue();
        errorMessage.Should().BeEmpty();
    }

    [Fact]
    public void Task_CreatedAt11_00_CompletedAt11_01_ShouldBeValid()
    {
        var creationTime = new DateTime(2026, 1, 13, 11, 0, 0);
        var completionTime = new DateTime(2026, 1, 13, 11, 1, 0);
        
        var task = new TaskManager.Domain.Task
        {
            Titulo = "Tarefa de teste",
            DataCriacao = creationTime,
            DataConclusao = completionTime,
            Status = TaskStatus.Concluida
        };

        var isValid = task.IsValid(out var errorMessage);

        isValid.Should().BeTrue("tarefa concluída 1 minuto após a criação deve ser válida");
        errorMessage.Should().BeEmpty();
    }

    [Fact]
    public void Task_CreatedAt11_00_CompletedAt10_59_ShouldBeInvalid()
    {
        var creationTime = new DateTime(2026, 1, 13, 11, 0, 0);
        var completionTime = new DateTime(2026, 1, 13, 10, 59, 0);
        
        var task = new TaskManager.Domain.Task
        {
            Titulo = "Tarefa de teste",
            DataCriacao = creationTime,
            DataConclusao = completionTime,
            Status = TaskStatus.Concluida
        };

        var isValid = task.IsValid(out var errorMessage);

        isValid.Should().BeFalse("tarefa não pode ser concluída antes de ser criada");
        errorMessage.Should().Be("A data e hora de conclusão não pode ser anterior à data e hora de criação");
    }

    [Fact]
    public void Task_WithEmptyTitle_ShouldHaveValidationError()
    {
        var task = new TaskManager.Domain.Task
        {
            Titulo = "",
            DataCriacao = DateTime.Now,
            Status = TaskStatus.Pendente
        };

        task.Titulo.Should().BeEmpty();
    }

    [Fact]
    public void Task_WithTitleOver100Characters_ShouldHaveValidationError()
    {
        var titulo = new string('a', 101);
        var task = new TaskManager.Domain.Task
        {
            Titulo = titulo,
            DataCriacao = DateTime.Now,
            Status = TaskStatus.Pendente
        };

        task.Titulo.Length.Should().BeGreaterThan(100);
    }

    [Fact]
    public void Task_WithNullCompletionDate_ShouldBeValid()
    {
        var task = new TaskManager.Domain.Task
        {
            Titulo = "Tarefa pendente",
            DataCriacao = DateTime.Now,
            DataConclusao = null,
            Status = TaskStatus.Pendente
        };

        var isValid = task.IsValid(out string errorMessage);

        isValid.Should().BeTrue();
        errorMessage.Should().BeEmpty();
    }

    [Fact]
    public void Task_WithFutureCompletionDate_ShouldBeValid()
    {
        var task = new TaskManager.Domain.Task
        {
            Titulo = "Tarefa futura",
            DataCriacao = DateTime.Now,
            DataConclusao = DateTime.Now.AddDays(5),
            Status = TaskStatus.EmProgresso
        };

        var isValid = task.IsValid(out string errorMessage);

        isValid.Should().BeTrue();
        errorMessage.Should().BeEmpty();
    }

    [Theory]
    [InlineData(TaskStatus.Pendente)]
    [InlineData(TaskStatus.EmProgresso)]
    [InlineData(TaskStatus.Concluida)]
    public void Task_WithAllStatusTypes_ShouldBeValid(TaskStatus status)
    {
        var task = new TaskManager.Domain.Task
        {
            Titulo = "Tarefa com status",
            DataCriacao = DateTime.Now,
            Status = status
        };

        var isValid = task.IsValid(out string errorMessage);

        isValid.Should().BeTrue();
        errorMessage.Should().BeEmpty();
    }
}
