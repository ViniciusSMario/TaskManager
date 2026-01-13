using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TaskManager.API.DTOs;
using TaskManager.Application;
using TaskManager.Domain;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service)
    {
        _service = service;
    }

    [HttpGet]
    public async System.Threading.Tasks.Task<IActionResult> GetAll()
    {
        try
        {
            var tasks = await _service.GetAllTasksAsync();
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao buscar tarefas", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async System.Threading.Tasks.Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
            return BadRequest(new { error = "ID deve ser maior que zero" });

        try
        {
            var task = await _service.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound(new { error = $"Tarefa com ID {id} não encontrada" });
            return Ok(task);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao buscar tarefa", details = ex.Message });
        }
    }

    [HttpPost]
    public async System.Threading.Tasks.Task<IActionResult> Create([FromBody] TaskCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var task = new TaskManager.Domain.Task
            {
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Status = dto.Status,
                DataConclusao = dto.DataConclusao
            };

            var created = await _service.CreateTaskAsync(task);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async System.Threading.Tasks.Task<IActionResult> Update(int id, [FromBody] TaskUpdateDto dto)
    {
        if (id <= 0)
            return BadRequest(new { error = "ID deve ser maior que zero" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var task = await _service.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound(new { error = $"Tarefa com ID {id} não encontrada" });

            task.Titulo = dto.Titulo ?? task.Titulo;
            task.Descricao = dto.Descricao ?? task.Descricao;
            task.Status = dto.Status ?? task.Status;
            task.DataConclusao = dto.DataConclusao ?? task.DataConclusao;

            await _service.UpdateTaskAsync(task);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao atualizar tarefa", details = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async System.Threading.Tasks.Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return BadRequest(new { error = "ID deve ser maior que zero" });

        try
        {
            await _service.DeleteTaskAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro ao deletar tarefa", details = ex.Message });
        }
    }
}