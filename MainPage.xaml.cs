using ToDoApp.Models;
using ToDoApp.ViewModel;

namespace ToDoApp;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage()
    {
        InitializeComponent();
        _viewModel = new MainViewModel();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CarregarTarefasAsync();
    }
    private async void OnChecking(object sender, CheckedChangedEventArgs e)
    {
        var checkbox = sender as CheckBox;
        var tarefa = checkbox?.BindingContext as Tarefa;

        if (tarefa != null)
        {
            tarefa.Concluida = e.Value;
            await _viewModel.UpdateItem(tarefa);
        }
    }

    private void OnDragStarting(object sender, DragStartingEventArgs e)
    {
        var element = sender as Element;
        var tarefa = element?.BindingContext as Tarefa;

        if (tarefa != null)
        {
            _viewModel.OnDragStarting(tarefa);
        }
    }


}