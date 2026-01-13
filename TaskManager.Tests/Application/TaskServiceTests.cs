using FluentAssertions;
using Moq;
using TaskManager.Application;
using TaskManager.Domain;
using TaskManager.Infrastructure;
using Xunit;
using TaskStatus = TaskManager.Domain.TaskStatus;

namespace TaskManager.Tests.Application;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _service = new TaskService(_repositoryMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAllTasksAsync_ShouldReturnAllTasks()
    {
        var tasks = new List<TaskManager.Domain.Task>
        {
            new() { Id = 1, Titulo = "Tarefa 1", DataCriacao = DateTime.Now, Status = TaskStatus.Pendente },
            new() { Id = 2, Titulo = "Tarefa 2", DataCriacao = DateTime.Now, Status = TaskStatus.EmProgresso }
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

        var result = await _service.GetAllTasksAsync();

        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(tasks);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTaskByIdAsync_WithValidId_ShouldReturnTask()
    {
        var task = new TaskManager.Domain.Task
        {
            Id = 1,
            Titulo = "Tarefa de teste",
            DataCriacao = DateTime.Now,
            Status = TaskStatus.Pendente
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);

        var result = await _service.GetTaskByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Titulo.Should().Be("Tarefa de teste");
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTaskByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((TaskManager.Domain.Task?)null);

        var result = await _service.GetTaskByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTaskAsync_WithValidTask_ShouldSetCreationDateAndReturnTask()
    {
        var task = new TaskManager.Domain.Task
        {
            Titulo = "Nova tarefa",
            Descricao = "Descrição da tarefa",
            Status = TaskStatus.Pendente
        };
        _repositoryMock.Setup(r => r.ExistsByTitleAsync("Nova tarefa")).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<TaskManager.Domain.Task>()))
            .ReturnsAsync((TaskManager.Domain.Task t) => { t.Id = 1; return t; });

        var result = await _service.CreateTaskAsync(task);

        result.Should().NotBeNull();
        result.DataCriacao.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskManager.Domain.Task>()), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTaskAsync_WithInvalidTask_ShouldThrowException()
    {
        var task = new TaskManager.Domain.Task
        {
            Titulo = "",  // Título vazio (inválido)
            Status = TaskStatus.Pendente
        };

        Func<System.Threading.Tasks.Task> act = async () => await _service.CreateTaskAsync(task);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("O título é obrigatório");
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTaskAsync_WithCompletionDateBeforeCreationDate_ShouldThrowException()
    {
        var task = new TaskManager.Domain.Task
        {
            Titulo = "Tarefa inválida",
            DataCriacao = DateTime.Now,
            DataConclusao = DateTime.Now.AddDays(-1),
            Status = TaskStatus.Concluida
        };

        Func<System.Threading.Tasks.Task> act = async () => await _service.CreateTaskAsync(task);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("A data e hora de conclusão não pode ser anterior à data e hora de criação");
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTaskAsync_WithDuplicateTitle_ShouldThrowException()
    {
        var task = new TaskManager.Domain.Task
        {
            Titulo = "Tarefa existente",
            Descricao = "Descrição da tarefa",
            Status = TaskStatus.Pendente
        };

        // Simular que já existe uma tarefa com o mesmo título
        _repositoryMock.Setup(r => r.ExistsByTitleAsync("Tarefa existente")).ReturnsAsync(true);

        Func<System.Threading.Tasks.Task> act = async () => await _service.CreateTaskAsync(task);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Já existe uma tarefa com este título");
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTaskAsync_WithValidTask_ShouldUpdateTask()
    {
        var existingTask = new TaskManager.Domain.Task
        {
            Id = 1,
            Titulo = "Tarefa original",
            DataCriacao = DateTime.Now.AddDays(-1),
            Status = TaskStatus.Pendente
        };
        var updatedTask = new TaskManager.Domain.Task
        {
            Id = 1,
            Titulo = "Tarefa atualizada",
            DataCriacao = existingTask.DataCriacao,
            Status = TaskStatus.EmProgresso
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingTask);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TaskManager.Domain.Task>())).Returns(System.Threading.Tasks.Task.CompletedTask);

        await _service.UpdateTaskAsync(updatedTask);

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TaskManager.Domain.Task>()), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTaskAsync_WithNonExistentTask_ShouldThrowException()
    {
        var task = new TaskManager.Domain.Task
        {
            Id = 999,
            Titulo = "Tarefa inexistente",
            DataCriacao = DateTime.Now,
            Status = TaskStatus.Pendente
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((TaskManager.Domain.Task?)null);

        Func<System.Threading.Tasks.Task> act = async () => await _service.UpdateTaskAsync(task);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Tarefa com ID 999 não encontrada");
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTaskAsync_ChangeStatusToCompleted_ShouldSetCompletionDate()
    {
        var existingTask = new TaskManager.Domain.Task
        {
            Id = 1,
            Titulo = "Tarefa em progresso",
            DataCriacao = DateTime.Now.AddDays(-1),
            Status = TaskStatus.EmProgresso,
            DataConclusao = null
        };
        var updatedTask = new TaskManager.Domain.Task
        {
            Id = 1,
            Titulo = "Tarefa em progresso",
            DataCriacao = existingTask.DataCriacao,
            Status = TaskStatus.Concluida,
            DataConclusao = null
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingTask);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TaskManager.Domain.Task>())).Returns(System.Threading.Tasks.Task.CompletedTask);

        await _service.UpdateTaskAsync(updatedTask);

        updatedTask.DataConclusao.Should().NotBeNull();
        updatedTask.DataConclusao.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTaskAsync_ChangeStatusFromCompleted_ShouldThrowException()
    {
        var existingTask = new TaskManager.Domain.Task
        {
            Id = 1,
            Titulo = "Tarefa concluída",
            DataCriacao = DateTime.Now.AddDays(-2),
            Status = TaskStatus.Concluida,
            DataConclusao = DateTime.Now.AddDays(-1)
        };
        var updatedTask = new TaskManager.Domain.Task
        {
            Id = 1,
            Titulo = "Tarefa concluída",
            DataCriacao = existingTask.DataCriacao,
            Status = TaskStatus.Pendente,
            DataConclusao = existingTask.DataConclusao
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingTask);

        Func<System.Threading.Tasks.Task> act = async () => await _service.UpdateTaskAsync(updatedTask);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Não é possível alterar o status de uma tarefa concluída");
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteTaskAsync_WithValidId_ShouldDeleteTask()
    {
        var task = new TaskManager.Domain.Task
        {
            Id = 1,
            Titulo = "Tarefa a deletar",
            DataCriacao = DateTime.Now,
            Status = TaskStatus.Pendente
        };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
        _repositoryMock.Setup(r => r.DeleteAsync(1)).Returns(System.Threading.Tasks.Task.CompletedTask);

        await _service.DeleteTaskAsync(1);

        _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteTaskAsync_WithInvalidId_ShouldThrowException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((TaskManager.Domain.Task?)null);

        Func<System.Threading.Tasks.Task> act = async () => await _service.DeleteTaskAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Tarefa com ID 999 não encontrada");
    }
}
