using System;
using System.IO;
using System.Globalization;
using Core;

namespace IO
{
    public class FileHandler : IDisposable
    {
        private readonly string _filePath;
        private bool _disposed = false;

        public FileHandler(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            if (!File.Exists(_filePath)) File.Create(_filePath).Close();
        }

        public string FilePath => _filePath;

        public void WriteEntity(Person entity)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(FileHandler));

            RetryOnFileAccess(() =>
            {
                using (FileStream fs = new FileStream(_filePath, FileMode.Append, FileAccess.Write))
                using (StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.UTF8))
                {
                    WriteEntityToWriter(entity, writer);
                }
            });
        }

        private void WriteEntityToWriter(Person entity, StreamWriter writer)
        {
            string type = entity.GetType().Name;
            string name = $"{entity.FirstName}{entity.LastName}";
            writer.WriteLine($"{type} {name}");
            writer.WriteLine("{");

            writer.WriteLine($"\"firstname\": \"{entity.FirstName}\",");
            writer.WriteLine($"\"lastname\": \"{entity.LastName}\",");

            if (entity is Student student)
            {
                writer.WriteLine($"\"course\": \"{student.Course}\",");
                writer.WriteLine($"\"studentId\": \"{student.StudentId}\",");
                writer.WriteLine($"\"averageGrade\": \"{student.AverageGrade.ToString(CultureInfo.InvariantCulture)}\",");
                writer.WriteLine($"\"country\": \"{student.Country}\",");
                writer.WriteLine($"\"recordBookNumber\": \"{student.RecordBookNumber}\"");
            }
            else if (entity is McdonaldsWorker worker)
            {
                writer.WriteLine($"\"position\": \"{worker.Position}\"");
            }
            else if (entity is Manager manager)
            {
                writer.WriteLine($"\"department\": \"{manager.Department}\"");
            }

            writer.WriteLine("};");
            writer.Flush();
        }

        public Person ReadNextEntity(StreamReader reader)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(FileHandler));
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            if (reader.EndOfStream) return null;

            string line = reader.ReadLine();
            if (line == null) return null;

            string[] parts = line.Split(' ');
            if (parts.Length < 2) return null;
            string type = parts[0];
            string name = parts[1];

            string openBrace = reader.ReadLine();
            if (openBrace != "{") return null;

            string firstName = ParseAttribute(reader.ReadLine());
            string lastName = ParseAttribute(reader.ReadLine());

            Person entity = null;
            if (type == "Student")
            {
                string courseStr = ParseAttribute(reader.ReadLine());
                if (!int.TryParse(courseStr, out int course))
                {
                    throw new FormatException($"Невірний формат курсу: {courseStr}");
                }
                string studentId = ParseAttribute(reader.ReadLine());
                string avgGradeStr = ParseAttribute(reader.ReadLine());
                if (!double.TryParse(avgGradeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double avgGrade))
                {
                    throw new FormatException($"Невірний формат середнього балу: {avgGradeStr}");
                }
                string country = ParseAttribute(reader.ReadLine());
                string recordBook = ParseAttribute(reader.ReadLine());

                entity = new Student(firstName, lastName, course, studentId, avgGrade, country, recordBook);
            }
            else if (type == "McdonaldsWorker")
            {
                string position = ParseAttribute(reader.ReadLine());
                entity = new McdonaldsWorker(firstName, lastName, position);
            }
            else if (type == "Manager")
            {
                string department = ParseAttribute(reader.ReadLine());
                entity = new Manager(firstName, lastName, department);
            }

            string close = reader.ReadLine();
            if (close != "};")
            {

            }

            return entity;
        }

        private string ParseAttribute(string line)
        {
            if (string.IsNullOrEmpty(line) || !line.Contains(":"))
                return string.Empty;

            int colonIndex = line.IndexOf(":");
            int start = line.IndexOf("\"", colonIndex + 1) + 1;
            int end = line.LastIndexOf("\"");
            if (start >= end || start < 0 || end < 0)
                return string.Empty;

            string value = line.Substring(start, end - start).Trim();
            return value;
        }

        public void ProcessStudentsFromFile(Action<Student> processAction)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(FileHandler));

            RetryOnFileAccess(() =>
            {
                using (FileStream fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
                using (StreamReader reader = new StreamReader(fs, System.Text.Encoding.UTF8))
                {
                    Person entity;
                    while ((entity = ReadNextEntity(reader)) != null)
                    {
                        if (entity is Student student)
                        {
                            processAction(student);
                        }
                    }
                }
            });
        }

        public void DeleteByLastName(string lastName)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(FileHandler));

            RetryOnFileAccess(() =>
            {
                string tempFile = Path.GetTempFileName();
                using (FileStream fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
                using (StreamReader reader = new StreamReader(fs, System.Text.Encoding.UTF8))
                using (FileStream tempFs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                using (StreamWriter writer = new StreamWriter(tempFs, System.Text.Encoding.UTF8))
                {
                    Person entity;
                    while ((entity = ReadNextEntity(reader)) != null)
                    {
                        if (entity.LastName != lastName)
                        {
                            WriteEntityToWriter(entity, writer);
                        }
                    }
                }
                File.Delete(_filePath);
                File.Move(tempFile, _filePath);
            });
        }

        public void UpdateEntity(Student updatedStudent)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(FileHandler));

            RetryOnFileAccess(() =>
            {
                string tempFile = Path.GetTempFileName();
                using (FileStream fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
                using (StreamReader reader = new StreamReader(fs, System.Text.Encoding.UTF8))
                using (FileStream tempFs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
                using (StreamWriter writer = new StreamWriter(tempFs, System.Text.Encoding.UTF8))
                {
                    Person entity;
                    bool found = false;
                    while ((entity = ReadNextEntity(reader)) != null)
                    {
                        if (entity is Student student && student.FirstName == updatedStudent.FirstName && student.LastName == updatedStudent.LastName)
                        {
                            WriteEntityToWriter(updatedStudent, writer);
                            found = true;
                        }
                        else
                        {
                            WriteEntityToWriter(entity, writer);
                        }
                    }
                    if (!found)
                    {
                        WriteEntityToWriter(updatedStudent, writer);
                    }
                }
                File.Delete(_filePath);
                File.Move(tempFile, _filePath);
            });
        }

        private void RetryOnFileAccess(Action action, int retries = 5, int delayMs = 200)
        {
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    action();
                    return;
                }
                catch (IOException ex)
                {
                    if (i == retries - 1)
                    {
                        throw;
                    }
                    Console.WriteLine($"Спроба {i + 1}: Файл заблокований, чекаємо {delayMs} мс...");
                    System.Threading.Thread.Sleep(delayMs);
                }
            }
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}