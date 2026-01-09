using System.Text;
using System.Text.Json;
using Lab1.Models;

namespace Lab1.Services;

public class PersonSerializer
{
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public string SerializeToJson(Person person)
    {
        return JsonSerializer.Serialize(person, _options);
    }

    public Person DeserializeFromJson(string json)
    {
        return JsonSerializer.Deserialize<Person>(json);
    }

    public void SaveToFile(Person person, string filePath)
    {
        string json = JsonSerializer.Serialize(person, _options);
        File.WriteAllText(filePath, json, Encoding.UTF8);
    }

    public Person LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Файл не найден", filePath);

        string json = File.ReadAllText(filePath, Encoding.UTF8);
        return JsonSerializer.Deserialize<Person>(json);
    }

    public async Task SaveToFileAsync(Person person, string filePath)
    {
        string json = JsonSerializer.Serialize(person, _options);
        await File.WriteAllTextAsync(filePath, json, Encoding.UTF8);
    }

    public async Task<Person> LoadFromFileAsync(string filePath)
    {
        string json = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
        return JsonSerializer.Deserialize<Person>(json);
    }

    public void SaveListToFile(List<Person> people, string filePath)
    {
        string json = JsonSerializer.Serialize(people, _options);
        File.WriteAllText(filePath, json, Encoding.UTF8);
    }

    public List<Person> LoadListFromFile(string filePath)
    {
        string json = File.ReadAllText(filePath, Encoding.UTF8);
        return JsonSerializer.Deserialize<List<Person>>(json);
    }
}