using Patchwork.Tasks;
using System.Collections.ObjectModel;

namespace Patchwork.Maui;

public partial class MainPage : ContentPage
{
    private int completedTasks = 0;
    private ObservableCollection<string> runningTasks = new();
    private List<CancellationTokenSource> runningTaskTokens = new();
    private ObservableCollection<string> consoleOutput = new();

    public MainPage()
    {
        InitializeComponent();
        RunningTasksList.ItemsSource = runningTasks;
        UpdateTaskLabels();
        ConsoleOutputEditor.Text = string.Empty;
    }

    private void OnAddTaskClicked(object? sender, EventArgs e)
    {
        var cts = new CancellationTokenSource();
        runningTaskTokens.Add(cts);
        string taskName = $"DemoTask #{runningTasks.Count + 1}";
        runningTasks.Add(taskName);
        UpdateTaskLabels();
        RunDemoTaskAsync(taskName, cts.Token);
    }

    private async void RunDemoTaskAsync(string taskName, CancellationToken token)
    {
        var task = new DemoTask();
        try
        {
            LogToConsole($"Started {taskName}");
            await task.RunAsync(new Dictionary<string, string>(), token);
            LogToConsole($"Completed {taskName}");
        }
        catch (Exception ex)
        {
            LogToConsole($"Error in {taskName}: {ex.Message}");
        }
        finally
        {
            runningTasks.Remove(taskName);
            completedTasks++;
            UpdateTaskLabels();
        }
    }

    private void OnCancelTasksClicked(object? sender, EventArgs e)
    {
        foreach (var cts in runningTaskTokens)
        {
            cts.Cancel();
        }
        LogToConsole("All running tasks cancelled.");
        runningTaskTokens.Clear();
    }

    private void UpdateTaskLabels()
    {
        RunningTasksLabel.Text = $"Running Tasks: {runningTasks.Count}";
        CompletedTasksLabel.Text = $"Completed Tasks: {completedTasks}";
    }

    private void LogToConsole(string message)
    {
        if (ConsoleOutputEditor.Text.Length > 0)
            ConsoleOutputEditor.Text += "\n";
        ConsoleOutputEditor.Text += message;
    }
}

