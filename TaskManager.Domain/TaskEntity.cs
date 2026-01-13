using System.ComponentModel.DataAnnotations;

namespace TaskManager.Domain;

public enum TaskStatus
{
    Pendente,
    EmProgresso,
    Concluida
}

public class Task
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "O título é obrigatório")]
    [MaxLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres")]
    public string Titulo { get; set; } = string.Empty;
    
    public string? Descricao { get; set; }
    
    public DateTime DataCriacao { get; set; }
    
    public DateTime? DataConclusao { get; set; }
    
    public TaskStatus Status { get; set; }
    
    // Validação customizada
    public bool IsValid(out string errorMessage)
    {
        if (DataConclusao.HasValue && DataConclusao.Value < DataCriacao)
        {
            errorMessage = "A data e hora de conclusão não pode ser anterior à data e hora de criação";
            return false;
        }
        errorMessage = string.Empty;
        return true;
    }
}
