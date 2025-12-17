namespace NoteTakingApp.Services;

public interface ISerializationService
{
    string Serialize<T>(T obj);
    T Deserialize<T>(string json);
}
