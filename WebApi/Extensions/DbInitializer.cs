using Domain.Entity;
using Domain.Errors;
using Domain.Shared;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Reflection;
using System.Text.Json;

namespace WebApi.Extensions
{
	public static class DbInitializer
	{
		private static readonly string _adminFile = "first-admin-login.json";

		private class Admin
		{
			public string Email { get; set; } = null!;
			public string Username { get; set; } = null!;
			public string Password { get; set; } = null!;
		}

		public static void InitializeDb(this WebApplication app)
		{
			if (IsTestEnvironment())
				return;

			using var scope = app.Services.CreateScope();
			var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

			context.Database.Migrate();
			context.SaveChanges();

			SeedAdminUser(context);
			SeedTestData(context);
		}

		private static void SeedAdminUser(AppDbContext context)
		{
			if (!context.Users.Any())
			{
				string json = File.ReadAllText(_adminFile);
				var admin = JsonSerializer.Deserialize<Admin>(json);

				if (admin == null) throw new Exception("Невозможно создать первого пользователя системы");

				var user = User.Create(
					Email.Create(admin.Email),
					Username.Create(admin.Username),
					PasswordHashed.Create(admin.Password)
				);

				if (user.IsSuccess)
				{
					context.Users.Add(user.Value);
					context.SaveChanges();
				}
				else
				{
					throw new Exception($"Ошибка создания администратора: {user.Error}");
				}
			}
		}

		private static void SeedTestData(AppDbContext context)
		{
			// Проверяем, есть ли уже данные в базе
			if (context.Patients.Any() || context.Procedures.Any())
				return;

			try
			{
				Console.WriteLine("Заполнение базы тестовыми данными...");

				// 1. Создаем врачей
				var doctors = CreateTestDoctors(context);

				// 2. Создаем типы процедур
				var procedureTypes = CreateProcedureTypes(context);

				// 3. Создаем пациентов с картами
				var patients = CreateTestPatients(context);

				// 4. Создаем процедуры
				CreateTestProcedures(context, patients, procedureTypes, doctors);

				context.SaveChanges();
				Console.WriteLine("Тестовые данные успешно добавлены в базу");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ошибка при заполнении тестовыми данными: {ex.Message}");
				// В продакшене здесь нужно логировать ошибку, но не прерывать работу приложения
			}
		}

		private static List<Doctor> CreateTestDoctors(AppDbContext context)
		{
			var doctors = new List<Doctor>();
			var doctorNames = new[] {
		"Др. Иванова Анна", "Др. Петров Сергей", "Др. Соколова Мария", "Др. Козлов Дмитрий",
		"Др. Смирнова Елена", "Др. Васильев Игорь", "Др. Новикова Ольга", "Др. Михайлов Александр"
	};

			foreach (var name in doctorNames)
			{
				var doctorResult = Doctor.Create(Username.Create(name));
				if (doctorResult.IsSuccess)
				{
					doctors.Add(doctorResult.Value);
					context.Doctors.Add(doctorResult.Value);
				}
			}
			context.SaveChanges();
			return doctors;
		}

		private static List<ProcedureType> CreateProcedureTypes(AppDbContext context)
		{
			var procedureTypes = new List<ProcedureType>();
			var procedureTypeData = new[]
			{
		("Чистка лица", "Глубокая чистка пор и удаление черных точек", 60, 3500),
		("Пилинг", "Химический пилинг для обновления кожи", 45, 4200),
		("Массаж лица", "Расслабляющий лимфодренажный массаж", 40, 3000),
		("Ботокс", "Инъекции ботокса для разглаживания морщин", 20, 12000),
		("Контурная пластика губ", "Увеличение и коррекция формы губ", 30, 18000),
		("Мезотерапия", "Инъекции витаминных коктейлей", 25, 3800),
		("RF-лифтинг", "Радиочастотное омоложение кожи", 50, 5500),
		("Биоревитализация", "Восстановление водного баланса кожи", 30, 7000),
		("Фракционный лазер", "Лазерное омоложение кожи", 45, 15000),
		("Плазмотерапия", "Омоложение с помощью плазмы крови", 40, 8500),
		("Карбокситерапия", "Лечение с помощью углекислого газа", 35, 6500),
		("Плазмолифтинг", "Омоложение с помощью тромбоцитарной плазмы", 30, 9000)
	};

			foreach (var (title, description, duration, price) in procedureTypeData)
			{
				var typeResult = ProcedureType.Create(
					Title.Create(title),
					description,
					duration,
					price
				);

				if (typeResult.IsSuccess)
				{
					procedureTypes.Add(typeResult.Value);
					context.ProcedureTypes.Add(typeResult.Value);
				}
			}
			context.SaveChanges();
			return procedureTypes;
		}

		private static List<Patient> CreateTestPatients(AppDbContext context)
		{
			var patients = new List<Patient>();
			var random = new Random();

			// Генерируем 60 пациентов с разнообразными данными
			string[] firstNamesFemale = { "Анна", "Мария", "Екатерина", "Ольга", "Наталья", "Ирина", "Татьяна", "Светлана", "Юлия", "Елена" };
			string[] lastNamesFemale = { "Иванова", "Петрова", "Сидорова", "Козлова", "Смирнова", "Попова", "Лебедева", "Козлова", "Новикова", "Морозова" };
			string[] firstNamesMale = { "Алексей", "Сергей", "Дмитрий", "Игорь", "Михаил", "Андрей", "Николай", "Владимир", "Александр", "Павел" };
			string[] lastNamesMale = { "Иванов", "Петров", "Сидоров", "Козлов", "Смирнов", "Попов", "Лебедев", "Козлов", "Новиков", "Морозов" };
			string[] streets = { "Тверская", "Арбат", "Ленина", "Пушкина", "Гагарина", "Кирова", "Советская", "Мира", "Новослободская", "Бакунинская", "Таганская", "Вернадского", "Новокузнецкая" };
			string[] complaints = {
		"Сухость кожи, морщины вокруг глаз",
		"Угревая сыпь, расширенные поры",
		"Птоз век, носогубные складки",
		"Пигментные пятна, тусклый цвет лица",
		"Чувствительная кожа, покраснения",
		"Глубокие морщины, дряблость кожи",
		"Купероз, отечность лица",
		"Провисание овала лица, второй подбородок",
		"Постакне, рубцы",
		"Жирная кожа, черные точки",
		"Расширенные поры, тусклый цвет лица",
		"Сухость и шелушение кожи",
		"Мешки под глазами, темные круги",
		"Неровный тон кожи, пигментация"
	};

			for (int i = 0; i < 60; i++)
			{
				bool isFemale = random.Next(0, 2) == 0;
				string firstName = isFemale ? firstNamesFemale[random.Next(0, firstNamesFemale.Length)] : firstNamesMale[random.Next(0, firstNamesMale.Length)];
				string lastName = isFemale ? lastNamesFemale[random.Next(0, lastNamesFemale.Length)] : lastNamesMale[random.Next(0, lastNamesMale.Length)];
				string fullName = $"{lastName} {firstName}";

				int age = random.Next(18, 70);
				string street = streets[random.Next(0, streets.Length)];
				string address = $"г. Москва, ул. {street}, д.{random.Next(1, 100)}";
				string phone = $"+79{random.Next(100000000, 999999999)}";
				string complaint = complaints[random.Next(0, complaints.Length)];

				var patientResult = Patient.Create(Username.Create(fullName));
				if (patientResult.IsSuccess)
				{
					var patient = patientResult.Value;
					var card = patient.Card;

					// Обновляем данные карты пациента
					card.Age = (byte)age;
					card.Address = Text.Create(address).Value;
					card.Complaints = Text.Create(complaint).Value;
					card.PhoneNumber = PhoneNumber.Create(phone).Value;

					patients.Add(patient);
					context.Patients.Add(patient);
				}
			}
			context.SaveChanges();
			return patients;
		}

		private static void CreateTestProcedures(AppDbContext context, List<Patient> patients,
											   List<ProcedureType> procedureTypes, List<Doctor> doctors)
		{
			var random = new Random();
			var startDate = DateTime.UtcNow.AddMonths(-6); // Начинаем с 6 месяцев назад
			var endDate = DateTime.UtcNow;

			// Создаем около 300 процедур
			int totalProcedures = 300;

			// Распределяем процедуры по дням за последние 6 месяцев
			var totalDays = (int)(endDate - startDate).TotalDays;
			var proceduresPerDay = totalProcedures / totalDays;

			var currentDateTime = startDate;
			while (currentDateTime <= endDate && totalProcedures > 0)
			{
				// Количество процедур в этот день (с некоторой вариативностью)
				int proceduresToday = random.Next(Math.Max(0, proceduresPerDay - 2), proceduresPerDay + 3);

				for (int i = 0; i < proceduresToday && totalProcedures > 0; i++)
				{
					// Выбираем случайного пациента
					var patient = patients[random.Next(0, patients.Count)];

					// Выбираем случайный тип процедуры
					var procedureType = procedureTypes[random.Next(0, procedureTypes.Count)];

					// Выбираем случайного врача
					var doctor = doctors[random.Next(0, doctors.Count)];

					// Время процедуры в течение дня (9:00 - 20:00)
					var procedureTime = currentDateTime.Date.AddHours(9 + random.Next(0, 12));

					// Определяем статус процедуры
					bool isComplete = procedureTime < DateTime.UtcNow;
					bool isCancelled = !isComplete && random.Next(1, 101) > 90; // 10% отмененных

					var procedureResult = Procedure.Create(
						Result.Success(patient.Card),
						Result.Success(procedureType),
						procedureType.StandartDuration,
						procedureType.StandartPrice,
						procedureTime,
						isCancelled ? null : doctor
					);

					if (procedureResult.IsSuccess)
					{
						var procedure = procedureResult.Value;

						if (isComplete)
						{
							procedure.Proceed();

							// Для 30% завершенных процедур создаем уведомление
							if (random.Next(1, 101) <= 30 && doctor != null)
							{
								var notificationDate = procedureTime.AddDays(random.Next(1, 14));
								var notification = Notification.Create(
									Result.Success(procedure),
									Text.Create($"Напоминание: ваша процедура '{procedureType.Title.Value}' проведена {procedureTime.ToString("d")}. Рекомендуем повторный визит через 2 недели."),
									notificationDate,
									patient.Card.PhoneNumber
								);

								if (notification.IsSuccess)
								{
									procedure.Notification = notification.Value;
									context.Notifications.Add(notification.Value);
								}
							}
						}
						else if (isCancelled)
						{
							procedure.IsCancelled = true;
						}

						context.Procedures.Add(procedure);
						totalProcedures--;
					}
				}

				currentDateTime = currentDateTime.AddDays(1);
			}

			context.SaveChanges();
		}

		private static bool IsTestEnvironment()
		{
			return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Test" ||
				   Assembly.GetEntryAssembly() == null ||
				   AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName != null && a.FullName.Contains("Test"));
		}
	}
}