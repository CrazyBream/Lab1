using System;
using Core;

namespace IO
{
    public class ConsoleMenu
    {
        private readonly FileHandler _fileHandler;

        public ConsoleMenu(FileHandler fileHandler)
        {
            _fileHandler = fileHandler ?? throw new ArgumentNullException(nameof(fileHandler));
        }

        public void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("\nМеню:");
                Console.WriteLine("1. Додати сутність");
                Console.WriteLine("2. Прочитати та вивести всі сутності");
                Console.WriteLine("3. Видалити сутність за прізвищем");
                Console.WriteLine("4. Пошук за прізвищем");
                Console.WriteLine("5. Обчислити студентів 3-го курсу з України");
                Console.WriteLine("6. Демонстрація дій (Study, PlayChess)");
                Console.WriteLine("7. Вихід");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": AddEntity(); break;
                    case "2": ReadAndDisplay(); break;
                    case "3": DeleteEntity(); break;
                    case "4": SearchByLastName(); break;
                    case "5": CalculateUkrainianStudents(); break;
                    case "6": DemonstrateActions(); break;
                    case "7": return;
                    default: Console.WriteLine("Невірний вибір. Спробуйте ще раз."); break;
                }
            }
        }

        private void AddEntity()
        {
            Console.WriteLine("Виберіть тип: 1 - Student, 2 - McdonaldsWorker, 3 - Manager");
            string type = Console.ReadLine();

            Console.Write("FirstName: "); string first = Console.ReadLine();
            if (!Validator.ValidateName(first)) { Console.WriteLine("Невірне ім'я."); return; }

            Console.Write("LastName: "); string last = Console.ReadLine();
            if (!Validator.ValidateName(last)) { Console.WriteLine("Невірне прізвище."); return; }

            Person entity = null;
            if (type == "1")
            {
                Console.Write("Course (1-5): "); int course = int.Parse(Console.ReadLine());
                if (!Validator.ValidateCourse(course)) { Console.WriteLine("Невірний курс."); return; }

                Console.Write("StudentId (e.g., KB123456): "); string id = Console.ReadLine();
                if (!Validator.ValidateStudentId(id)) { Console.WriteLine("Невірний StudentId."); return; }

                Console.Write("AverageGrade (0-5): "); double grade = double.Parse(Console.ReadLine());
                if (!Validator.ValidateAverageGrade(grade)) { Console.WriteLine("Невірний середній бал."); return; }

                Console.Write("Country: "); string country = Console.ReadLine();
                if (!Validator.ValidateCountry(country)) { Console.WriteLine("Невірна країна."); return; }

                Console.Write("RecordBookNumber (5 цифр): "); string book = Console.ReadLine();
                if (!Validator.ValidateRecordBookNumber(book)) { Console.WriteLine("Невірний номер залікової книжки."); return; }

                entity = new Student(first, last, course, id, grade, country, book);
            }
            else if (type == "2")
            {
                Console.Write("Position: "); string pos = Console.ReadLine();
                entity = new McdonaldsWorker(first, last, pos);
            }
            else if (type == "3")
            {
                Console.Write("Department: "); string dep = Console.ReadLine();
                entity = new Manager(first, last, dep);
            }

            if (entity != null)
            {
                _fileHandler.WriteEntity(entity);
                Console.WriteLine("Сутність додана.");
            }
        }

        private void ReadAndDisplay()
        {
            using (FileStream fs = new FileStream(_fileHandler.FilePath, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(fs, System.Text.Encoding.UTF8))
            {
                Person entity;
                while ((entity = _fileHandler.ReadNextEntity(reader)) != null)
                {
                    DisplayEntity(entity);
                }
            }
        }

        private void DisplayEntity(Person entity)
        {
            Console.WriteLine($"Тип: {entity.GetType().Name}, Ім'я: {entity.FirstName} {entity.LastName}");
            if (entity is Student student)
            {
                Console.WriteLine($"Course: {student.Course}, StudentId: {student.StudentId}, AverageGrade: {student.AverageGrade}, Country: {student.Country}, RecordBook: {student.RecordBookNumber}");
            }
            else if (entity is McdonaldsWorker worker)
            {
                Console.WriteLine($"Position: {worker.Position}");
            }
            else if (entity is Manager manager)
            {
                Console.WriteLine($"Department: {manager.Department}");
            }
        }

        private void DeleteEntity()
        {
            Console.Write("Введіть прізвище для видалення: "); string last = Console.ReadLine();
            _fileHandler.DeleteByLastName(last);
            Console.WriteLine("Сутність видалена (якщо знайдена).");
        }

        private void SearchByLastName()
        {
            Console.Write("Введіть прізвище для пошуку: "); string last = Console.ReadLine();
            using (FileStream fs = new FileStream(_fileHandler.FilePath, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(fs, System.Text.Encoding.UTF8))
            {
                Person entity;
                bool found = false;
                while ((entity = _fileHandler.ReadNextEntity(reader)) != null)
                {
                    if (entity.LastName.Equals(last, StringComparison.OrdinalIgnoreCase))
                    {
                        DisplayEntity(entity);
                        found = true;
                    }
                }
                if (!found) Console.WriteLine("Сутність не знайдена.");
            }
        }

        private void CalculateUkrainianStudents()
        {
            int count = 0;
            using (FileStream fs = new FileStream(_fileHandler.FilePath, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(fs, System.Text.Encoding.UTF8))
            {
                Person entity;
                while ((entity = _fileHandler.ReadNextEntity(reader)) != null)
                {
                    if (entity is Student student && student.Course == 3 && student.Country.Equals("Ukraine", StringComparison.OrdinalIgnoreCase))
                    {
                        count++;
                        DisplayEntity(student);
                    }
                }
            }
            Console.WriteLine($"Кількість студентів 3-го курсу з України: {count}");
        }

        private void DemonstrateActions()
        {
            using (FileStream fs = new FileStream(_fileHandler.FilePath, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(fs, System.Text.Encoding.UTF8))
            {
                Person entity = _fileHandler.ReadNextEntity(reader);
                if (entity != null)
                {
                    if (entity is IStudyable studyable)
                    {
                        studyable.Study();
                        Console.WriteLine($"Дія Study виконана для {entity.FirstName} {entity.LastName}.");
                    }
                    if (entity is IChessPlayer player)
                    {
                        player.PlayChess();
                        Console.WriteLine($"Дія PlayChess виконана для {entity.FirstName} {entity.LastName}.");
                    }
                }
                else
                {
                    Console.WriteLine("Немає сутностей для демонстрації.");
                }
            }
        }
    }
}