# Лабораторная работа 1  
**Тема:** сериализация JSON, управление ресурсами и работа с файлами в C#

## 1. Краткое описание

В лабораторной реализованы:

1. Класс `Person` (папка `src/Lab1/Models`) с атрибутами JSON и валидацией полей.
2. Класс `PersonSerializer` (папка `src/Lab1/Services`) для сериализации/десериализации и работы с JSON‑файлами (синхронно и асинхронно).
3. Класс `FileResourceManager` (папка `src/Lab1/Services`), реализующий паттерн `IDisposable` для корректного управления файловыми ресурсами.
4. Консольное приложение `Lab1` (`src/Lab1/Program.cs`) для демонстрации работы.
5. Отдельный тестовый проект `Lab1.Tests` (`tests/Lab1.Tests`) с юнит‑тестами для всех основных классов.

### Используемые технологии

| Технология   | Версия (минимальная/целевой фреймворк) |
|--------------|-----------------------------------------|
| C#           | 10+                                    |
| .NET SDK     | 6.0+ / 8.0+ / 9.0                      |

IDE:
Visual Studio Code

### Используемые пространства имён

- `System`
- `System.IO`
- `System.Text`
- `System.Text.Json`
- `System.Text.Json.Serialization`
- `System.Collections.Generic`
- `System.Threading.Tasks`

---

## 2. Класс Person и атрибуты JSON

Реализован класс `Person` со следующими полями и свойствами:

- `FirstName` — имя  
- `LastName` — фамилия  
- `Age` — возраст  
- `FullName` — только для чтения (конкатенация `FirstName + " " + LastName`)  
- `IsAdult` — только для чтения (`Age >= 18`)  
- `Password` — помечено атрибутом `[JsonIgnore]` (не попадает в JSON)  
- `Id` — сериализуется с именем поля `"personId"` через `[JsonPropertyName("personId")]`  
- `BirthDate` — приватное поле `_birthDate` с публичным свойством и атрибутом `[JsonInclude]`  
- `Email` — свойство с валидацией (обязательное наличие символа `'@'`, иначе `ArgumentException`)  
- `PhoneNumber` — сериализуется под именем `"phone"` через `[JsonPropertyName("phone")]`

---

## 3. Менеджер сериализации PersonSerializer

Класс `PersonSerializer` реализует:

1. `SerializeToJson(Person person)` — сериализация объекта `Person` в строку JSON.  
2. `DeserializeFromJson(string json)` — десериализация строки JSON в объект `Person`.  
3. `SaveToFile(Person person, string filePath)` — синхронное сохранение объекта в файл.  
4. `LoadFromFile(string filePath)` — синхронная загрузка объекта из файла.  
5. `SaveToFileAsync(Person person, string filePath)` — асинхронное сохранение объекта в файл.  
6. `LoadFromFileAsync(string filePath)` — асинхронная загрузка объекта из файла.  
7. `SaveListToFile(List<Person> people, string filePath)` — экспорт списка объектов в JSON‑файл.  
8. `LoadListFromFile(string filePath)` — импорт списка объектов из JSON‑файла.

Для работы с JSON используются общие настройки `JsonSerializerOptions`:

- `WriteIndented = true` — красивое форматирование JSON с отступами;
- `Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping` — чтобы символ `+` в телефоне не экранировался как `\u002B`.

Во всех файловых операциях используется кодировка UTF‑8.

---

## 4. Класс FileResourceManager и IDisposable

Реализован класс `FileResourceManager`, который управляет файловыми ресурсами и корректно освобождает их через `IDisposable`.

### Поля

- `FileStream _fileStream` — базовый поток для работы с файлом.
- `StreamWriter _writer` — текстовая запись.
- `StreamReader _reader` — текстовое чтение.
- `bool _disposed` — флаг, указывающий, что объект уже освобождён.
- `string _filePath` — путь к файлу.

### Основные методы

- Конструктор `FileResourceManager(string filepath)` — принимает путь к файлу.
- `OpenForWriting(string path)` — открытие файла для записи (создание/перезапись файла, `FileMode.Create`).
- `OpenForReading()` — открытие файла для чтения, с проверкой существования.
- `WriteLine(string text)` — запись строки в файл.
- `ReadAllText()` — чтение всего содержимого файла.
- `AppendText(string text)` — добавление текста в конец файла с использованием вложенных `using`.
- `GetFileInfo()` — возвращает `FileInfo` для файла (размер, дата создания и т.д.).
- `Dispose()` / `Dispose(bool disposing)` — корректное освобождение `StreamWriter`, `StreamReader`, `FileStream` и установка `_disposed = true`.  
- Финализатор `~FileResourceManager()` — вызывает `Dispose(false)` на случай, если пользователь не вызвал `Dispose()` явно.

Во всех публичных методах вызывается `ThrowIfDisposed()`, чтобы предотвратить использование освобождённого объекта после освобождения ресурсов.

---

## 5. Тестирование функционала

Тесты реализованы в отдельном проекте `Lab1.Tests` (папка `tests/Lab1.Tests`) на базе xUnit.

Покрываются следующие сценарии:

- `PersonTests`  
  - проверка вычисляемых свойств `FullName` и `IsAdult`;  
  - проверка валидации `Email` (исключения при некорректных значениях).

- `PersonSerializerTests`  
  - сериализация в строку и последующая десериализация (сравнение ключевых полей);  
  - проверка, что поле `Password` не попадает в JSON (`[JsonIgnore]`);  
  - проверка, что JSON форматирован с отступами (`WriteIndented = true`);  
  - проверка синхронного и асинхронного сохранения/загрузки одного `Person`;  
  - проверка сохранения и загрузки списка `List<Person>`.

- `FileResourceManagerTests`  
  - запись и чтение текста через `WriteLine` / `ReadAllText`;  
  - добавление текста в конец файла методом `AppendText`;  
  - получение корректной информации о файле через `GetFileInfo`;  
  - проверка, что методы бросают `ObjectDisposedException` после вызова `Dispose()`.

---

## 6. Инструкция по запуску

### Запуск консольного приложения

В корне репозитория (папка `lab01`):

dotnet run --project src/Lab1/Lab1.csproj

чтобы запустить тесты:

dotnet test