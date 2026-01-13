using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.API.Controllers;
using TaskManager.API.DTOs;
using TaskManager.Application;
using TaskManager.Domain;
using Xunit;
using TaskStatus = TaskManager.Domain.TaskStatus;

namespace TaskManager.Tests.API;

public class TasksControllerTests
{
    private readonly Mock<ITaskService> _serviceMock;
    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        _serviceMock = new Mock<ITaskService>();
        _controller = new TasksController(_serviceMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAll_ShouldReturnOkWithTasks()
    {
        var tasks = new List<TaskManager.Domain.Task>
        {
            new() { Id = 1, Titulo = "Tarefa 1", DataCriacao = DateTime.Now, Status = TaskStatus.Pendente },
            new() { Id = 2, Titulo = "Tarefa 2", DataCriacao = DateTime.Now, Status = TaskStatus.EmProgresso }
        };
        _serviceMock.Setup(s => s.GetAllTasksAsync()).ReturnsAsync(tasks);

        var result = await _controller.GetAll();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTasks = okResult.Value.Should().BeAssignableTo<IEnumerable<TaskManager.Domain.Task>>().Subject;
        returnedTasks.Should().HaveCount(2);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetById_WithValidId_ShouldReturnOkWithTask()
    {
        var task = new TaskManager.Domain.Task
        {
            Id = 1,
            Titulo = "Tarefa de teste",
            DataCriacao = DateTime.Now,
            Status = TaskStatus.Pendente
        };
        _serviceMock.Setup(s => s.GetTaskByIdAsync(1)).ReturnsAsync(task);

        var result = await _controller.GetById(1);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTask = okResult.Value.Should().BeAssignableTo<TaskManager.Domain.Task>().Subject;
        returnedTask.Id.Should().Be(1);
        returnedTask.Titulo.Should().Be("Tarefa de teste");
    }

    [Fact]
    public async System.Threading.Tasks.Task GetById_WithInvalidId_ShouldReturnNotFound()
    {
        _serviceMock.Setup(s => s.GetTaskByIdAsync(999)).ReturnsAsync((TaskManager.Domain.Task?)null);

        var result = await _controller.GetById(999);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Create_WithValidTask_ShouldReturnCreatedAtAction()
    {
        var createDto = new TaskCreateDto(
            Titulo: "Nova tarefa",
            Descricao: "Descrição da tarefa",
            Status: TaskStatus.Pendente,
            DataConclusao: null
        );
        var createdTask = new TaskManager.Domain.Task
        {
            Id = 1,
            Titulo = createDto.Titulo,
            Descricao = createDto.Descricao,
            DataCriacao = DateTime.Now,
            Status = createDto.Status
        };
        _serviceMock.Setup(s => s.CreateTaskAsync(It.IsAny<TaskManager.Domain.Task>())).ReturnsAsync(createdTask);

        var result = await _controller.Create(createDto);

        var createdAtActionResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdAtActionResult.ActionName.Should().Be(nameof(TasksController.GetById));
        createdAtActionResult.RouteValues!["id"].Should().Be(1);
        var returnedTask = createdAtActionResult.Value.Should().BeAssignableTo<TaskManager.Domain.Task>().Subject;
        returnedTask.Titulo.Should().Be("Nova tarefa");
    }

    [Fact]
    public async System.Threading.Tasks.Task Create_WithInvalidTask_ShouldReturnBadRequest()
    {
        var createDto = new TaskCreateDto(
            Titulo: "Título válido",
            Descricao: "Descrição",
            Status: TaskStatus.Pendente,
            DataConclusao: null
        );
        _serviceMock.Setup(s => s.CreateTaskAsync(It.IsAny<TaskManager.Domain.Task>()))
            .ThrowsAsync(new InvalidOperationException("O título é obrigatório"));

        var result = await _controller.Create(createDto);

        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().BeEquivalentTo(new { error = "O título é obrigatório" });
    }

    [Fact]
    public async System.Threading.Tasks.Task Update_WithValidTask_ShouldReturnNoContent()
    {
        var existingTask = new TaskManager.Domain.Task
        {
            Id = 1,
            Titulo = "Tarefa original",
            DataCriacao = DateTime.Now.AddDays(-1),
            Status = TaskStatus.Pendente
        };
        var updateDto = new TaskUpdateDto(
            Titulo: "Tarefa atualizada",
            Descricao: "Descrição atualizada",
            Status: TaskStatus.EmProgresso,
            DataConclusao: null
        );
        
        _serviceMock.Setup(s => s.GetTaskByIdAsync(1)).ReturnsAsync(existingTask);
        _serviceMock.Setup(s => s.UpdateTaskAsync(It.IsAny<TaskManager.Domain.Task>())).Returns(System.Threading.Tasks.Task.CompletedTask);

        var result = await _controller.Update(1, updateDto);

        result.Should().BeOfType<NoContentResult>();
        _serviceMock.Verify(s => s.UpdateTaskAsync(It.Is<TaskManager.Domain.Task>(t => t.Id == 1)), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Update_WithNonExistentTask_ShouldReturnNotFound()
    {
        var updateDto = new TaskUpdateDto(
            Titulo: "Tarefa inexistente",
            Descricao: "Descrição",
            Status: TaskStatus.Pendente,
            DataConclusao: null
        );
        _serviceMock.Setup(s => s.GetTaskByIdAsync(999)).ReturnsAsync((TaskManager.Domain.Task?)null);

        var result = await _controller.Update(999, updateDto);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async System.Threading.Tasks.Task Update_WithInvalidTask_ShouldReturnBadRequest()
    {
        var existingTask = new TaskManager.Domain.Task
        {
            Id = 1,
            Titulo = "Tarefa original",
            DataCriacao = DateTime.Now.AddDays(-1),
            Status = TaskStatus.Pendente
        };
        var updateDto = new TaskUpdateDto(
            Titulo: "Título válido",
            Descricao: "Descrição",
            Status: TaskStatus.Pendente,
            DataConclusao: null
        );
        
        _serviceMock.Setup(s => s.GetTaskByIdAsync(1)).ReturnsAsync(existingTask);
        _serviceMock.Setup(s => s.UpdateTaskAsync(It.IsAny<TaskManager.Domain.Task>()))
            .ThrowsAsync(new InvalidOperationException("O título é obrigatório"));

        var result = await _controller.Update(1, updateDto);

        var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().BeEquivalentTo(new { error = "O título é obrigatório" });
    }

    [Fact]
    public async System.Threading.Tasks.Task Delete_WithValidId_ShouldReturnNoContent()
    {
        _serviceMock.Setup(s => s.DeleteTaskAsync(1)).Returns(System.Threading.Tasks.Task.CompletedTask);

        var result = await _controller.Delete(1);

        result.Should().BeOfType<NoContentResult>();
        _serviceMock.Verify(s => s.DeleteTaskAsync(1), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Delete_WithNonExistentTask_ShouldReturnNotFound()
    {
        _serviceMock.Setup(s => s.DeleteTaskAsync(999))
            .ThrowsAsync(new KeyNotFoundException("Tarefa com ID 999 não encontrada"));

        var result = await _controller.Delete(999);

        result.Should().BeOfType<NotFoundObjectResult>();
    }
}

