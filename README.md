# Cosmetics_GraduateWork

**Cosmetics_GraduateWork** — это дипломная работа, система управления клиникой косметологии.


https://github.com/user-attachments/assets/148c956d-1817-492a-8a9d-56145c40eb30


---

## ⚙️ Технологии

### Backend
- **ASP.NET Core Web API** — создание HTTP API
- **Entity Framework Core (PostgreSQL)** — работа с базой данных
- **MediatR** — реализация паттерна CQRS
- **FluentValidation** — валидация входящих запросов
- **Serilog** — логирование HTTP-запросов и внутренних событий
- **JWT-аутентификация** — защита API и управление доступом
- **Swagger (Swashbuckle)** — автоматическая генерация документации API
- **CORS** — разрешение запросов с клиентского приложения

### Frontend
- **React 19** — современный UI-фреймворк
- **TypeScript** — типизация JavaScript
- **Vite** — быстрый сборщик и dev-сервер
- **Tailwind CSS 4** — утилитарные CSS-классы
- **React Router** — навигация между страницами
- **Axios** — HTTP-клиент
- **Recharts** — графики и диаграммы
- **date-fns** — работа с датами
- **Framer Motion** — анимации
- **React Toastify** — уведомления

---

## 📋 Запуск проекта

### Требования
- .NET 8 SDK
- Node.js 18+
- PostgreSQL 14+

### 1. Настройка базы данных

Создайте базу данных PostgreSQL и обновите строку подключения в `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=CosmeticsDB;Username=postgres;Password=your_password"
  }
}
```

### 2. Запуск Backend

```bash
cd WebApi
dotnet restore
dotnet run
```

Backend будет доступен по адресу: `https://localhost:7135`

### 3. Запуск Frontend

```bash
cd webclient
npm install
npm run dev
```

Frontend будет доступен по адресу: `https://localhost:62284`

---

## 🔐 Демо-доступ

```
Email: demo@cosmetics.com
Пароль: Demo123!
```

---

## 📑 API Документация

Swagger доступен по адресу: `https://localhost:7135/swagger`

### Авторизация в Swagger
1. Нажмите кнопку **Authorize**
2. Введите JWT-токен в формате: `Bearer {your_token}`
3. Или получите токен через endpoint `/api/Users/Login`

---

## 🏗 Архитектура проекта

```
Cosmetics_GraduateWork/
├── Domain/              # Доменная логика, сущности, value objects
├── Application/         # CQRS команды и запросы, валидация
├── Infrastructure/      # Внешние сервисы, аутентификация
├── Persistence/         # EF Core, миграции, репозитории
├── WebApi/              # ASP.NET Core Web API, контроллеры
└── webclient/           # React frontend
```

### Доменные паттерны

#### Value Object (DDD)
- Абстрактный класс `ValueObject` — основа для объектов-значений
- Сравнение по атомарным свойствам, а не по идентификатору
- Примеры использования: Email, Username, PasswordHashed

#### Unit of Work
- Инкапсулирует вызов `_dbContext.SaveChangesAsync()`
- Обеспечивает атомарность сохранения изменений в базе данных

---

## 🎨 Возможности системы

### Для администраторов клиники
- 📊 **Панель управления** — быстрая статистика и навигация
- 👥 **Управление пациентами** — создание, редактирование, удаление карт пациентов
- 📅 **Расписание процедур** — календарь записи пациентов
- 📈 **Аналитика** — графики выручки, популярные процедуры, загруженность врачей
- 📝 **Медицинская информация** — состояния здоровья, особенности кожи, уход

### Основные функции
- Поиск и фильтрация пациентов
- Пагинация списков
- Создание новых типов данных (состояния, процедуры, уход)
- Экспорт аналитики в CSV
- JWT-аутентификация
- Валидация данных
- Логирование событий

---

## 🔧 Настройка CORS

В файле `WebApi/Program.cs` настроены CORS для frontend:

```csharp
policy.WithOrigins("https://localhost:62284", "http://localhost:62284")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials();
```

---

## 📝 Структура frontend

```
webclient/src/
├── api/                 # API клиент (axios)
├── Components/          # React компоненты
│   ├── Layout/          # Layout компоненты (Sidebar, Header)
│   └── UI/              # UI компоненты (Button, Input, Card, Modal)
├── Context/             # React Context (Auth)
├── Pages/               # Страницы приложения
├── TypesFromServer/     # TypeScript типы
└── index.css            # Глобальные стили
```

---

## 🚀 Развертывание

### Production сборка frontend

```bash
cd webclient
npm run build
```

Сборка будет в папке `dist/`

### Публикация backend

```bash
cd WebApi
dotnet publish -c Release -o ./publish
```

---

## 📄 Лицензия

Учебный проект — дипломная работа.
