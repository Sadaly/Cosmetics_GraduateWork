# Cosmetics_GraduateWork [WIP]
**Cosmetics_GraduateWork** - это моя дипломная работа
---

## ⚙️ Технологии и компоненты

- **ASP.NET Core Web API** — создание HTTP API  
- **Entity Framework Core (PostgreSQL)** — работа с базой данных  
- **MediatR** — реализация паттерна CQRS  
- **FluentValidation** — валидация входящих запросов  
- **Serilog** — логирование HTTP-запросов и внутренних событий  
- **JWT-аутентификация** — защита API и управление доступом  
- **Swagger (Swashbuckle)** — автоматическая генерация документации API  
- **CORS** — разрешение запросов с клиентского приложения

## Дополнительно:
- **Middleware** - кастомная обработка 

---

## 📑 Swagger

- Поддержка авторизации через JWT  
- Кнопка **Authorize** в Swagger UI  
- Передача токена в заголовке `Authorization: Bearer {token}`  

---

## 🏗 UnitOfWork

- Инкапсулирует вызов `_dbContext.SaveChangesAsync()`  
- Обеспечивает атомарность сохранения изменений в базе данных  

---

## 🧱 Доменные паттерны

### Value Object (DDD)

- Абстрактный класс `ValueObject` — основа для объектов-значений  
- Сравнение по атомарным свойствам, а не по идентификатору  
- Реализованы методы `GetAtomicValues()`, `Equals(...)`, `GetHashCode()`  
- Примеры использования: Email, Username, PasswordHashed

---
