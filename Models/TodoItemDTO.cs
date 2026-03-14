namespace TodoApi.Models;

// DTO exact du tutoriel — empêche la sur-publication en excluant le champ Secret
public class TodoItemDTO
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
}
