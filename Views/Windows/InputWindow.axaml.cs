using Avalonia.Controls;
using Avalonia.Interactivity;
using FluentAvalonia.UI.Windowing;


namespace RocketLauncherRemake;

public partial class InputWindow : AppWindow
{
    public InputWindow(string Title)
    {
        InitializeComponent();
        this.Title = Title;
    }

    private void Button_Click(object sender,RoutedEventArgs e)
    {
        this.Close($"{RootTextBox.Text}");
    }
}