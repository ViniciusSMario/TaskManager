using System.ComponentModel.DataAnnotations;

namespace TaskManager.API.DTOs;

public record TaskCreateDto(
    [Required(ErrorMessage = "O título é obrigatório")]
    [MaxLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres")]
    string Titulo, 
    string? Descricao, 
    TaskManager.Domain.TaskStatus Status,
    DateTime? DataConclusao);
