# Лабораторная работа 4

**Тема:** Синхронизация потоков — deadlock, race condition, starvation

---

## Описание задачи

Реализованы три классические задачи синхронизации потоков:

### 1. Обедающие философы (Dining Philosophers)

5 философов сидят за столом, между ними 5 вилок. Для еды нужны 2 вилки. Реализованы две версии:
- **С deadlock** — демонстрация проблемы взаимоблокировки
- **Без deadlock** — решение через упорядочивание ресурсов

### 2. Спящий парикмахер (Sleeping Barber)

Парикмахер спит, если нет клиентов. Клиенты будят его или уходят, если зал ожидания полон.

### 3. Производитель-Потребитель (Producer-Consumer)

Производители кладут товары в ограниченный буфер, потребители забирают.

---

## Используемые технологии

| Технология | Версия |
|------------|--------|
| C# | 12 |
| .NET SDK | 8.0+ |
| xUnit | 2.5+ |

**IDE:** Visual Studio Code

---

## Используемые библиотеки и пространства имён

- `System.Threading` — `lock`, `SemaphoreSlim`, `Interlocked`, `CancellationToken`
- `System.Threading.Tasks` — `Task`, `async/await`
- `System.Collections.Concurrent` — `BlockingCollection`, `ConcurrentQueue`

---

## Структура проекта

```
Lab4/
├── Lab4.Core/              # Библиотека с логикой
│   ├── DiningPhilosophers/
│   ├── SleepingBarber/
│   └── ProducerConsumer/
├── Lab4.Console/           # Консольное приложение
└── Lab4.Tests/             # Юнит-тесты
```

---

## Инструкция по запуску

### Сборка

```bash
cd Lab4
dotnet build
```

### Запуск программы

```bash
dotnet run --project Lab4.Console
```

### Запуск тестов

```bash
dotnet test
```