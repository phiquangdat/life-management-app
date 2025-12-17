using System.Text.Json;

namespace NoteTakingApp.Services;

public class SerializationService : ISerializationService
{
    public string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj);
    }

    public T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }
}
