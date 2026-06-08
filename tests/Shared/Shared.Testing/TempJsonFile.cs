using System.Text.Json;

namespace Shared.Testing;

public static class TempJsonFile
{
    public static string CreateCopy(string fileName)
    {
        var sourcePath = Path.Combine(AppContext.BaseDirectory, "data", fileName);
        var tempPath = Path.Combine(Path.GetTempPath(), $"library-test-{Guid.NewGuid():N}.json");
        File.Copy(sourcePath, tempPath, overwrite: true);
        return tempPath;
    }

    public static string CreateEmptyArrayFile()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"library-test-{Guid.NewGuid():N}.json");
        File.WriteAllText(tempPath, "[]");
        return tempPath;
    }

    public static async Task<T?> ReadAsync<T>(string filePath)
    {
        await using var stream = File.OpenRead(filePath);
        return await JsonSerializer.DeserializeAsync<T>(
            stream,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }
}
