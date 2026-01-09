# Homework Ordering System API

CRUD-сервис на .NET для системы продажи и заказа домашних заданий.

## Технологии

- **.NET 8.0**
- **PostgreSQL** (через EF Core)
- **Dapper** (для транзакций)
- **Redis** (кэширование)
- **JWT Bearer** (аутентификация)
- **API Key** (альтернативная аутентификация)
- **Liquibase** (миграции БД)
- **Prometheus** (метрики)
- **Serilog** (логирование)
- **Swagger** (документация API)

## Быстрый старт

### Требования

- Docker и Docker Compose
- .NET 8.0 SDK

### Запуск через Docker Compose

1. Запустите PostgreSQL и Redis:
```bash
docker-compose up -d postgres redis
```

2. Дождитесь готовности сервисов (проверьте health check):
```bash
docker-compose ps
```

3. Примените миграции Liquibase:
```bash
docker-compose up liquibase
```

4. Запустите приложение:
```bash
dotnet run
```

Приложение будет доступно по адресу: `https://localhost:5001` или `http://localhost:5000`

### Swagger UI

После запуска приложения откройте в браузере:
- **Development**: `https://localhost:5001/swagger` или `http://localhost:5000/swagger`

## Архитектура

Проект следует принципам Clean Architecture:

- **Controllers** - обработка HTTP-запросов
- **Services** - бизнес-логика
- **Repositories** - доступ к данным
- **DTO** - объекты передачи данных
- **Middleware** - обработка запросов, ошибок, логирование

## Основные возможности

### Аутентификация

1. **JWT Bearer Token**
   - Регистрация: `POST /api/auth/register`
   - Вход: `POST /api/auth/login`
   - Использование: заголовок `Authorization: Bearer {token}`

2. **API Key**
   - Использование: заголовок `X-API-KEY: {your-api-key}`

### Роли пользователей

- **User** - обычный пользователь (может создавать заказы, видеть только свои заказы)
- **Manager** - менеджер (может создавать/изменять продукты и заказы)
- **Admin** - администратор (полный доступ, включая удаление)

### Кэширование

- GET-запросы кэшируются в Redis (TTL: 5 минут)
- Кэш автоматически инвалидируется при обновлении/удалении данных

### Метрики Prometheus

Доступны по адресу: `/metrics`

Собираются метрики:
- Количество HTTP-запросов
- Время выполнения запросов
- Ошибки

### Health Checks

Проверка состояния сервиса: `/health`

Проверяются:
- Доступность Web API
- Подключение к PostgreSQL
- Подключение к Redis

## Тестирование

Запуск unit-тестов:
```bash
dotnet test
```

Тесты покрывают:
- ProductRepository (CRUD операции)
- OrderRepository (CRUD операции)
- UserRepository (CRUD операции)
- ApiKeyRepository (CRUD операции)

## Конфигурация

Основные настройки в `appsettings.json`:

- **ConnectionStrings** - строки подключения к БД и Redis
- **JwtSettings** - настройки JWT токенов

### Настройка для разработки

1. Скопируйте `appsettings.example.json` в `appsettings.json` (если файл еще не существует)
2. Измените значения в `appsettings.json` согласно вашей локальной конфигурации
3. `appsettings.Development.json` используется для настроек разработки (уже в .gitignore)

## Структура базы данных

### Таблицы

- **Users** - пользователи системы
- **Products** - продукты (домашние задания)
- **Orders** - заказы
- **OrderProducts** - связь many-to-many между заказами и продуктами
- **ApiKeys** - API ключи для системных клиентов

### Миграции

Миграции выполняются через Liquibase. Файлы находятся в `Liquibase/change-sets/`

## API Endpoints

### Аутентификация

- `POST /api/auth/register` - регистрация нового пользователя
- `POST /api/auth/login` - вход в систему

### Продукты

- `GET /api/products` - получить все продукты
- `GET /api/products/{id}` - получить продукт по ID
- `GET /api/products/paged?page=1&pageSize=10&search=&subject=` - получить продукты с пагинацией и фильтрацией
- `POST /api/products` - создать продукт (требуется роль Manager или Admin)
- `PUT /api/products/{id}` - обновить продукт (требуется роль Manager или Admin)
- `DELETE /api/products/{id}` - удалить продукт (требуется роль Admin)

### Заказы

- `GET /api/orders` - получить заказы (User видит только свои, Manager/Admin - все)
- `GET /api/orders/{id}` - получить заказ по ID
- `POST /api/orders` - создать заказ (требуется авторизация)
- `PUT /api/orders/{id}` - обновить заказ (требуется роль Manager или Admin)
- `DELETE /api/orders/{id}` - удалить заказ (требуется роль Admin)

## Дополнительные возможности

**Grafana** - визуализация метрик (опционально)

## Разработка

### Добавление новых эндпоинтов

1. Создайте DTO в `Models/DTO/`
2. Добавьте методы в соответствующий Repository
3. Добавьте бизнес-логику в Service
4. Создайте эндпоинт в Controller
5. Добавьте документацию в Swagger (XML комментарии)

### Логирование

Все запросы и ошибки логируются через Serilog. Логи выводятся в консоль в структурированном формате.

## Лицензия

Проект создан в учебных целях.
