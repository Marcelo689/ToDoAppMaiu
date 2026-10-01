using SQLite;

namespace ToDoApp.Models;

public class Tarefa
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;
    public bool Concluida { get; set; }
    public int Ordem { get; set; } // Útil para manter a ordenação do Drag & Drop!
}