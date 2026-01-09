using Lab1.Models;
using Lab1.Services;

class Program
{
    static async Task Main()
    {
        var serializer = new PersonSerializer();

        string baseDir = Path.Combine(Directory.GetCurrentDirectory(), "TestData");
        Directory.CreateDirectory(baseDir);

        string personFilePath = Path.Combine(baseDir, "person.json");
        string listFilePath = Path.Combine(baseDir, "people.json");

        try
        {
            Console.WriteLine("Демонстрация работы программы\n");

            // Демонстрация сериализации
            var person = CreateSamplePerson();
            Console.WriteLine("Созданный объект Person:");
            Console.WriteLine($"  FullName: {person.FullName}");
            Console.WriteLine($"  IsAdult: {person.IsAdult}");
            Console.WriteLine($"  Email: {person.Email}");
            Console.WriteLine();

            // Сериализация в JSON
            string json = serializer.SerializeToJson(person);
            Console.WriteLine("JSON (сериализация):");
            Console.WriteLine(json);
            Console.WriteLine();

            // Сохранение в файл
            serializer.SaveToFile(person, personFilePath);
            Console.WriteLine($"Сохранено в файл: {personFilePath}");

            // Загрузка из файла
            var loaded = serializer.LoadFromFile(personFilePath);
            Console.WriteLine($"Загружено из файла: {loaded.FullName}");
            Console.WriteLine();

            // Работа со списком
            var people = new List<Person>
            {
                person,
                new Person("Petr", "Petrov", 30, "pwd2", "p2", "+7-999-111-11-11", "petr@example.com")
            };
            serializer.SaveListToFile(people, listFilePath);
            Console.WriteLine($"Список из {people.Count} человек сохранён в: {listFilePath}");

            Console.WriteLine("\nПрограмма завершена успешно");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    static Person CreateSamplePerson()
    {
        return new Person(
            fname: "Ivan",
            lname: "Ivanov",
            age: 25,
            pswd: "secret123",
            id: "p1",
            phone: "+7-999-000-00-00",
            email: "ivan.ivanov@example.com")
        {
            BirthDate = new DateTime(1999, 1, 1)
        };
    }
}
