using System.Text.Json;

namespace HospitalClassesLibrary
{
    public class StreamService<T>
    {
        public async Task WriteToStreamAsync(Stream stream, IEnumerable<T> data, IProgress<string>? progress = null)
        {
            progress?.Report("Starting to write to stream...");
            await JsonSerializer.SerializeAsync(stream, data);
            stream.Position = 0;
            progress?.Report("Writing complete.");
        }

        public async Task CopyFromStreamAsync(Stream stream, string filePath, IProgress<string>? progress = null)
        {
            progress?.Report("Starting to copy stream to file...");
            using var fileStream = File.Create(filePath);
            stream.Position = 0;
            await stream.CopyToAsync(fileStream);
            progress?.Report("Copy complete.");
        }

        public async Task<int> GetStatisticsAsync(string filePath, Func<T, bool> predicate)
        {
            using var fileStream = File.OpenRead(filePath);
            var data = await JsonSerializer.DeserializeAsync<List<T>>(fileStream);
            return data?.Count(predicate) ?? 0;
        }
    }
}
