using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input; // Necessário para ICommand
using ToDoApp.Models;
using ToDoApp.Services;

namespace ToDoApp.ViewModel;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly TodoDatabase _database;
    private Tarefa _draggedItem;
    private string _novoTitulo = string.Empty;

    public ObservableCollection<Tarefa> ListaTarefas { get; set; } = new();

    // 1. Propriedade para o texto digitado no Entry
    public string Titulo
    {
        get => _novoTitulo;
        set
        {
            if (_novoTitulo != value)
            {
                _novoTitulo = value;
                OnPropertyChanged();
            }
        }
    }

    // 2. Command para o botão de Adicionar
    public ICommand AdicionarTarefaCommand { get; }
    public ICommand DragStartingCommand { get; }
    public ICommand OnDropCompletedCommand { get; }
    public ICommand RemoverTarefaCommand { get; }

    public MainViewModel()
    {
        _database = new TodoDatabase();

        // Inicializa o Command apontando para a ação de adicionar
        AdicionarTarefaCommand = new Command(async () => await AdicionarTarefaAsync());
        DragStartingCommand = new Command<Tarefa>(async (tarefa) => await DragStartingAsync(tarefa));
        RemoverTarefaCommand = new Command<Tarefa>(async (tarefa) => await RemoverTarefaAsync(tarefa));
        OnDropCompletedCommand = new Command<Tarefa>(async (targetTarefa) => await OnDropCompleted(targetTarefa));
    }

    public async Task CarregarTarefasAsync()
    {
        var tarefas = await _database.GetTarefasAsync();
        ListaTarefas.Clear();
        foreach (var t in tarefas)
        {
            ListaTarefas.Add(t);
        }
    }
    public async Task DragStartingAsync(Tarefa tarefa)
    {
        _draggedItem = tarefa;
    }
    public async Task AdicionarTarefaAsync()
    {
        if (string.IsNullOrWhiteSpace(Titulo)) return;

        var novaTarefa = new Tarefa 
        { 
            Titulo = Titulo, 
            Ordem = ListaTarefas.Count 
        };

        await _database.SaveTarefaAsync(novaTarefa);
        ListaTarefas.Add(novaTarefa);

        // Limpa o campo de texto na tela após adicionar
        Titulo = string.Empty;
    }

    public async Task RemoverTarefaAsync(Tarefa tarefa)
    {
        if (tarefa == null) return;

        await _database.DeleteTarefaAsync(tarefa);
        ListaTarefas.Remove(tarefa);

        await AtualizarIndicesEGuardarAsync();
    }

    public void OnDragStarting(Tarefa tarefa)
    {
        _draggedItem = tarefa;
    }

    public async Task OnDropCompleted(Tarefa targetTarefa)
    {
        if (_draggedItem == null || targetTarefa == null || _draggedItem == targetTarefa)
            return;

        int oldIndex = ListaTarefas.IndexOf(_draggedItem);
        int newIndex = ListaTarefas.IndexOf(targetTarefa);

        if (oldIndex != -1 && newIndex != -1)
        {
            ListaTarefas.Move(oldIndex, newIndex);
            await AtualizarIndicesEGuardarAsync();
        }

        _draggedItem = null;
    }
    public async Task UpdateItem(Tarefa tarefa)
    {
        await _database.SaveTarefaAsync(tarefa);
    }
    private async Task AtualizarIndicesEGuardarAsync()
    {
        for (int i = 0; i < ListaTarefas.Count; i++)
        {
            ListaTarefas[i].Ordem = i;
        }
        await _database.SaveAllAsync(ListaTarefas);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}