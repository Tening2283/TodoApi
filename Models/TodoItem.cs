namespace TodoApi.Models;

// Modèle exact du tutoriel — section "Empêcher la sur-publication"
// Le champ Secret est ajouté pour démontrer l'utilité du DTO
public class TodoItem
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
    public string? Secret { get; set; }
}
