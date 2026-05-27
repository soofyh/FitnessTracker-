using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FitnessTracker.Client;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    
    public ApiClient(string baseUrl)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }
    
    public void SetToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
    
    // Auth
    public async Task<AuthResponse?> RegisterAsync(string username, string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", new { username, email, password });
        await HandleError(response);
        return await response.Content.ReadFromJsonAsync<AuthResponse>();
    }
    
    public async Task<AuthResponse?> LoginAsync(string username, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { username, password });
        await HandleError(response);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (auth != null) SetToken(auth.Token);
        return auth;
    }
    
    // Workouts
    public async Task<List<WorkoutResponse>?> GetWorkoutsAsync()
    {
        var response = await _httpClient.GetAsync("api/workouts");
        await HandleError(response);
        return await response.Content.ReadFromJsonAsync<List<WorkoutResponse>>();
    }
    
    public async Task<WorkoutResponse?> CreateWorkoutAsync(WorkoutRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/workouts", request);
        await HandleError(response);
        return await response.Content.ReadFromJsonAsync<WorkoutResponse>();
    }
    
    public async Task<WorkoutResponse?> UpdateWorkoutAsync(Guid id, WorkoutRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/workouts/{id}", request);
        await HandleError(response);
        return await response.Content.ReadFromJsonAsync<WorkoutResponse>();
    }
    
    public async Task<bool> DeleteWorkoutAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/workouts/{id}");
        await HandleError(response);
        return response.IsSuccessStatusCode;
    }
    
    // Files
    public async Task<List<FileResponse>?> GetFilesAsync()
    {
        var response = await _httpClient.GetAsync("api/files");
        await HandleError(response);
        return await response.Content.ReadFromJsonAsync<List<FileResponse>>();
    }
    
    public async Task<FileResponse?> UploadFileAsync(string filePath)
    {
        using var form = new MultipartFormDataContent();
        using var fs = File.OpenRead(filePath);
        using var sc = new StreamContent(fs);
        sc.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        form.Add(sc, "file", Path.GetFileName(filePath));
        var response = await _httpClient.PostAsync("api/files/upload", form);
        await HandleError(response);
        return await response.Content.ReadFromJsonAsync<FileResponse>();
    }
    
    public async Task DownloadFileAsync(Guid fileId, string savePath)
    {
        var response = await _httpClient.GetAsync($"api/files/{fileId}/download");
        await HandleError(response);
        await using var stream = await response.Content.ReadAsStreamAsync();
        await using var fs = File.Create(savePath);
        await stream.CopyToAsync(fs);
    }
    
    public async Task<bool> DeleteFileAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/files/{id}");
        await HandleError(response);
        return response.IsSuccessStatusCode;
    }
    
    // Goals
    public async Task<List<GoalResponse>?> GetGoalsAsync()
    {
        var response = await _httpClient.GetAsync("api/goals");
        await HandleError(response);
        return await response.Content.ReadFromJsonAsync<List<GoalResponse>>();
    }
    
    public async Task<GoalResponse?> GetGoalAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"api/goals/{id}");
        await HandleError(response);
        return await response.Content.ReadFromJsonAsync<GoalResponse>();
    }
    
    public async Task<GoalResponse?> CreateGoalAsync(GoalRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/goals", request);
        await HandleError(response);
        return await response.Content.ReadFromJsonAsync<GoalResponse>();
    }
    
    public async Task<GoalResponse?> UpdateGoalAsync(Guid id, GoalRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/goals/{id}", request);
        await HandleError(response);
        return await response.Content.ReadFromJsonAsync<GoalResponse>();
    }
    
    public async Task<bool> DeleteGoalAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/goals/{id}");
        await HandleError(response);
        return response.IsSuccessStatusCode;
    }
    
    public async Task<GoalResponse?> CompleteGoalAsync(Guid id)
    {
        var response = await _httpClient.PostAsync($"api/goals/{id}/complete", null);
        await HandleError(response);
        return await response.Content.ReadFromJsonAsync<GoalResponse>();
    }
    
    // Progress Photos
    public async Task<ProgressPhotoResponse?> UploadProgressPhotoAsync(Guid goalId, string filePath, bool isBefore)
    {
        using var form = new MultipartFormDataContent();
        using var fs = File.OpenRead(filePath);
        using var sc = new StreamContent(fs);
        form.Add(sc, "file", Path.GetFileName(filePath));
        var response = await _httpClient.PostAsync($"api/goals/{goalId}/photos?isBeforePhoto={isBefore}", form);
        await HandleError(response);
        return await response.Content.ReadFromJsonAsync<ProgressPhotoResponse>();
    }
    
    public async Task<bool> DeletePhotoAsync(Guid goalId, Guid photoId)
    {
        var response = await _httpClient.DeleteAsync($"api/goals/{goalId}/photos/{photoId}");
        await HandleError(response);
        return response.IsSuccessStatusCode;
    }
    
    private static async Task HandleError(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new ApiException(response.StatusCode, error);
        }
    }
}

public class ApiException : Exception
{
    public System.Net.HttpStatusCode StatusCode { get; }
    public ApiException(System.Net.HttpStatusCode code, string msg) : base(msg) => StatusCode = code;
}

public class AuthResponse { public string Token { get; set; } = ""; public string Username { get; set; } = ""; }
public class WorkoutResponse { public Guid Id { get; set; } public string Name { get; set; } = ""; public string Description { get; set; } = ""; public int DurationMinutes { get; set; } public int CaloriesBurned { get; set; } public DateTime Date { get; set; } }
public class WorkoutRequest { public string Name { get; set; } = ""; public string Description { get; set; } = ""; public int DurationMinutes { get; set; } public int CaloriesBurned { get; set; } public DateTime Date { get; set; } }
public class FileResponse { public Guid Id { get; set; } public string FileName { get; set; } = ""; public string ContentType { get; set; } = ""; public long Size { get; set; } public DateTime UploadedAt { get; set; } }
public class GoalResponse { public Guid Id { get; set; } public string Title { get; set; } = ""; public string Description { get; set; } = ""; public DateTime StartDate { get; set; } public DateTime? TargetDate { get; set; } public bool IsCompleted { get; set; } public DateTime? CompletedAt { get; set; } public List<ProgressPhotoResponse> BeforePhotos { get; set; } = new(); public List<ProgressPhotoResponse> AfterPhotos { get; set; } = new(); }
public class GoalRequest { public string Title { get; set; } = ""; public string Description { get; set; } = ""; public DateTime StartDate { get; set; } public DateTime? TargetDate { get; set; } }
public class ProgressPhotoResponse { public Guid Id { get; set; } public string FileName { get; set; } = ""; public string ContentType { get; set; } = ""; public long Size { get; set; } public bool IsBeforePhoto { get; set; } public DateTime UploadedAt { get; set; } }