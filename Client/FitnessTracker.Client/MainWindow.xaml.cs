using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;

namespace FitnessTracker.Client;

public partial class MainWindow : Window
{
    private ApiClient _apiClient;
    private Guid? _editingWorkoutId;
    private Guid? _selectedGoalIdForPhotos;
    private string? _selectedFilePath;
    
    public MainWindow()
    {
        InitializeComponent();
        _apiClient = new ApiClient("https://localhost:5001");
    }
    
    private void ShowLoading(bool s) => LoadingOverlay.Visibility = s ? Visibility.Visible : Visibility.Hidden;
    
    // Auth
    private async void Login_Click(object sender, RoutedEventArgs e)
    {
        try { ShowLoading(true); await _apiClient.LoginAsync(LoginUsername.Text, LoginPassword.Password);
            LoginPanel.Visibility = Visibility.Hidden; MainPanel.Visibility = Visibility.Visible;
            WelcomeText.Text = $"👤 {LoginUsername.Text}"; await LoadWorkouts(); }
        catch (ApiException ex) { LoginError.Text = ex.StatusCode == System.Net.HttpStatusCode.Unauthorized ? "❌ Неверный логин или пароль" : $"❌ Ошибка: {ex.Message}"; }
        finally { ShowLoading(false); }
    }
    
    private async void Register_Click(object sender, RoutedEventArgs e)
    {
        try { ShowLoading(true); await _apiClient.RegisterAsync(LoginUsername.Text, $"{LoginUsername.Text}@fitness.com", LoginPassword.Password);
            LoginError.Text = "✅ Регистрация успешна!"; }
        catch (ApiException ex) { LoginError.Text = $"❌ Ошибка: {ex.Message}"; }
        finally { ShowLoading(false); }
    }
    
    private void Logout_Click(object sender, RoutedEventArgs e) { LoginPanel.Visibility = Visibility.Visible; MainPanel.Visibility = Visibility.Hidden; LoginPassword.Password = ""; }
    
    // Navigation
    private void ShowPanel(string p)
    {
        WorkoutsPanel.Visibility = p == "Workouts" ? Visibility.Visible : Visibility.Hidden;
        GoalsPanel.Visibility = p == "Goals" ? Visibility.Visible : Visibility.Hidden;
        FilesPanel.Visibility = p == "Files" ? Visibility.Visible : Visibility.Hidden;
        PhotoViewerPanel.Visibility = Visibility.Hidden;
        _editingWorkoutId = null;
    }
    private void ShowWorkouts_Click(object sender, RoutedEventArgs e) => ShowPanel("Workouts");
    private void ShowGoals_Click(object sender, RoutedEventArgs e) => ShowPanel("Goals");
    private void ShowFiles_Click(object sender, RoutedEventArgs e) => ShowPanel("Files");
    private void ShowAddWorkout_Click(object sender, RoutedEventArgs e) { }
    private void ShowAddGoal_Click(object sender, RoutedEventArgs e) { }
    private void ShowUploadFile_Click(object sender, RoutedEventArgs e) { }
    
    // Workouts
    private async Task LoadWorkouts()
    {
        try { ShowLoading(true); var w = await _apiClient.GetWorkoutsAsync(); WorkoutsList.Items.Clear(); if (w != null) foreach (var x in w) WorkoutsList.Items.Add(new TextBlock { Text = $"{x.Name} - {x.Date:yyyy-MM-dd}" }); }
        catch (ApiException ex) { MessageBox.Show(ex.Message); }
        finally { ShowLoading(false); }
    }
    
    // Goals
    private async Task LoadGoals()
    {
        try { ShowLoading(true); var g = await _apiClient.GetGoalsAsync(); GoalsList.Items.Clear(); if (g != null) foreach (var x in g) GoalsList.Items.Add(new TextBlock { Text = x.Title }); }
        catch (ApiException ex) { MessageBox.Show(ex.Message); }
        finally { ShowLoading(false); }
    }
    
    // Files
    private async Task LoadFiles()
    {
        try { ShowLoading(true); var f = await _apiClient.GetFilesAsync(); FilesList.Items.Clear(); if (f != null) foreach (var x in f) FilesList.Items.Add(new TextBlock { Text = x.FileName }); }
        catch (ApiException ex) { MessageBox.Show(ex.Message); }
        finally { ShowLoading(false); }
    }
    
    // Photo Viewer
    private void OpenPhotoViewer(GoalResponse g) { _selectedGoalIdForPhotos = g.Id; PhotoViewerPanel.Visibility = Visibility.Visible; }
    private void ClosePhotoViewer_Click(object sender, RoutedEventArgs e) => PhotoViewerPanel.Visibility = Visibility.Hidden;
    private async void UploadBeforePhoto_Click(object sender, RoutedEventArgs e) { }
    private async void UploadAfterPhoto_Click(object sender, RoutedEventArgs e) { }
}