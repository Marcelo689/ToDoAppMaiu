using SQLite;
using ToDoApp.Models;

namespace ToDoApp.Services;

public class TodoDatabase
{
    private SQLiteAsyncConnection _database;

    private async Task Init()
    {
        if (_database is not null)
            return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "todoapp_data.db3");
        _database = new SQLiteAsyncConnection(dbPath);
        await _database.CreateTableAsync<Tarefa>();
    }

    public async Task<List<Tarefa>> GetTarefasAsync()
    {
        await Init();
        return await _database.Table<Tarefa>().OrderBy(t => t.Ordem).ToListAsync();
    }

    public async Task<int> SaveTarefaAsync(Tarefa item)
    {
        await Init();
        if (item.Id != 0)
            return await _database.UpdateAsync(item);
        else
            return await _database.InsertAsync(item);
    }

    public async Task<int> DeleteTarefaAsync(Tarefa item)
    {
        await Init();
        return await _database.DeleteAsync(item);
    }

    public async Task SaveAllAsync(IEnumerable<Tarefa> tarefas)
    {
        await Init();
        await _database.UpdateAllAsync(tarefas);
    }
}