# 🌐 AtonWebApi

&#x20;  &#x20;

> **Web API** для управления пользователями (`User`), реализованное на **.NET 9**, демонстрирующее **Clean Architecture**, **CQRS**, **MediatR**, **EF Core InMemory** и **AutoMapper**. Данные хранятся в памяти, а Swagger UI предоставляет удобную документацию и тестирование всех эндпоинтов.

---

## 🚀 Особенности

- **Полный цикл CRUD** над сущностью `User`:
  - **Create** — POST `/api/users`
  - **Read**   — GET `/api/users` и GET `/api/users/{login}`
  - **Update** — PUT `/api/users/profile/{login}`, `/api/users/password/{login}`, `/api/users/login/{login}`
  - **Delete** — DELETE `/api/users/{login}?hard=[true|false]`
  - **Restore**— PUT `/api/users/restore/{login}`
- **Ролевой доступ** через заголовок `X-Current-User`:
  - **Admin** может управлять всеми пользователями.
  - **Self** (активный пользователь) может менять свой профиль, пароль и логин.
- **CQRS + MediatR**
  - Логика разделена на команды и запросы (Handlers) без «толстых» контроллеров.
- **EF Core InMemory**
  - Простое хранение данных в памяти для демо; при перезапуске все сбрасывается.
- **AutoMapper**
  - Быстрый маппинг сущностей в DTO и обратно.
- **Swagger UI**
  - Полная OpenAPI-документация: каждая операция снабжена XML-комментариями и примерами.
- **Seed Admin**
  - При старте создаётся пользователь `Admin`/`Admin123`, у которого есть все права.

---

## 📦 Структура проекта

```
AtonWebApi.sln
└── src/
    ├── AtonWebApi.Domain/           # Сущности и Enums
    ├── AtonWebApi.Application/      # Интерфейсы, DTO, CQRS команды и Handlers
    ├── AtonWebApi.Infrastructure/   # EF Core DbContext и репозитории
    └── AtonWebApi.Api/              # ASP.NET Core Web API (контроллеры, DI, Swagger)
└── tests/
    └── AtonWebApi.Tests/            # Юнит-тесты (xUnit, Moq, EF Core InMemory)
    └── AtonWebApi.InegrationTests/  # Интеграционные тесты (WebApplicationFactory)
```

---

## ⚙️ Технологии

- **.NET 9 / ASP.NET Core**
- **Entity Framework Core** (InMemory)
- **MediatR** (CQRS)
- **AutoMapper**
- **Swashbuckle (Swagger UI)**
- **xUnit + Moq** (юнит-тесты)

---

## 🛠 Установка и запуск

1. **Клонировать репозиторий**
   ```bash
   git clone https://github.com/Avazbek22/AtonWebApi.git
   cd AtonWebApi
   ```
2. **Сборка**
   ```bash
   dotnet build
   ```
3. **Запуск Web API**
   ```bash
   cd src/AtonWebApi.Api
   dotnet run
   ```
   Swagger будет доступен по адресу:
   ```
   https://localhost:5102/swagger/index.html
   ```
4. **Запуск тестов**
   ```bash
   cd tests/AtonWebApi.Tests
   dotnet test
   ```

---

## 📝 Описание работы

1. **Seed Admin**: при старте проверяется наличие `Admin`, иначе создаётся с паролем `Admin123`.
2. **Авторизация**: эмуляция через заголовок `X-Current-User`, роли и права проверяются в Handler’ах.
3. **CQRS**: каждый экшен обёрнут в команду или запрос. Handlers содержат логику работы с репозиторием и проверку прав.
4. **Маппинг**: AutoMapper профили автоматически конвертируют `User` ↔ `UserViewDto`.
5. **Хранение**: EF Core InMemory для быстрого демо; легко сменить на реальную БД.

---

## 📈 Дальнейшее развитие

- Переключиться на постоянное хранилище (*SQL Server/PostgreSQL*)
- Добавить **JWT**-авторизацию
- Внедрить **FluentValidation** для DTO
- Подключить **CI/CD** (GitHub Actions) для автоматической сборки и тестирования

---
