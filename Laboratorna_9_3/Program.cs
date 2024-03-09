using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Explicit)]
public struct Student
{
    [FieldOffset(0)] public string LastName;

    [FieldOffset(32)] public int Exam1;

    [FieldOffset(36)] public int Exam2;

    [FieldOffset(40)] public int Exam3;
}

enum SortOption
{
    AverageGrade = 1,
    LastName,
    SubjectGrade
}

public class Program
{
    static void Main()
    {
        Console.WriteLine("Введіть ім'я файлу:");
        string fileName = Console.ReadLine();
        Student[] students = null;

        if (!File.Exists(fileName))
        {
            try
            {
                using (StreamWriter writer = File.CreateText(fileName))
                {
                    Console.WriteLine($"Створено новий файл '{fileName}'.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при створенні файлу: {ex.Message}");
                return;
            }
        }

        while (true)
        {
            Console.WriteLine("\nМеню:");
            Console.WriteLine("1. Зчитати дані з файлу");
            Console.WriteLine("2. Записати дані у файл");
            Console.WriteLine("3. Додати/редагувати інформацію");
            Console.WriteLine("4. Вилучити інформацію");
            Console.WriteLine("5. Сортувати список");
            Console.WriteLine("6. Вихід");

            int choice = GetMenuChoice();

            switch (choice)
            {
                case 1:
                    students = Read(fileName);
                    Print(students);
                    break;
                case 2:
                    Write(fileName, students);
                    break;
                case 3:
                    students = AddOrUpdateStudent(students);
                    Print(students);
                    break;
                case 4:
                    students = Remove(students);
                    Print(students);
                    break;
                case 5:
                    students = Sort(students);
                    Print(students);
                    break;
                case 6:
                    return;
                default:
                    Console.WriteLine("Неправильний вибір. Спробуйте ще раз.");
                    break;
            }
        }
    }

    static int GetMenuChoice()
    {
        Console.Write("Введіть номер опції: ");
        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice))
        {
            Console.WriteLine("Неправильний ввід. Будь ласка, введіть ціле число.");
            Console.Write("Введіть номер опції: ");
        }

        return choice;
    }


    public static Student[] Sort(Student[] students)
    {
        Console.WriteLine("\nСортування списку:");
        Console.WriteLine("1. За середнім балом");
        Console.WriteLine("2. За прізвищами (в алфавітному порядку)");
        Console.WriteLine("3. За оцінками із заданого предмету");

        int sortChoice = GetMenuChoice();

        SortOption sortOption;
        if (Enum.TryParse<SortOption>(sortChoice.ToString(), out sortOption))
        {
            switch (sortOption)
            {
                case SortOption.AverageGrade:
                    return students.OrderByDescending(s => (s.Exam1 + s.Exam2 + s.Exam3) / 3.0).ToArray();
                case SortOption.LastName:
                    return students.OrderByDescending(s => s.LastName).ToArray();
                case SortOption.SubjectGrade:
                    Console.Write("Введіть номер предмету для сортування (1, 2 або 3): ");
                    int subjectChoice = GetSubjectChoice();
                    return students.OrderByDescending(s => GetSubjectGrade(s, subjectChoice)).ToArray();
            }
        }

        return students;
    }

    public static Student[] Read(string fileName)
    {
        if (File.Exists(fileName))
        {
            try
            {
                string[] lines = File.ReadAllLines(fileName);
                Student[] students = new Student[lines.Length];

                for (int i = 0; i < lines.Length; i++)
                {
                    string[] data = lines[i].Split(',');
                    students[i].LastName = data[0];
                    students[i].Exam1 = int.Parse(data[1]);
                    students[i].Exam2 = int.Parse(data[2]);
                    students[i].Exam3 = int.Parse(data[3]);
                }

                Console.WriteLine($"Дані зчитано з файлу '{fileName}'.");
                return students;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при зчитуванні даних: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Файл '{fileName}' не знайдено.");
        }

        return null;
    }

    public static void Write(string fileName, Student[] students)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                foreach (var student in students)
                {
                    writer.WriteLine($"{student.LastName},{student.Exam1},{student.Exam2},{student.Exam3}");
                }
            }

            Console.WriteLine($"Дані записано у файл '{fileName}'.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка при записі даних: {ex.Message}");
        }
    }

    public static void Print(Student[] students)
    {
        if (students != null && students.Length > 0)
        {
            Console.WriteLine("\nСписок студентів:");

            foreach (var student in students)
            {
                Console.WriteLine($"{student.LastName}, {student.Exam1}, {student.Exam2}, {student.Exam3}");
            }
        }
        else
        {
            Console.WriteLine("Список порожній.");
        }
    }

    public static Student[] AddOrUpdateStudent(Student[] students)
    {
        Console.WriteLine("\nДодавання/редагування інформації про студента.");

        Console.Write("Введіть прізвище студента: ");
        string lastName = Console.ReadLine();

        if (students == null)
        {
            students = new Student[1];
            students[0].LastName = lastName;
        }

        int index = Array.FindIndex(students, s => s.LastName == lastName);

        if (index == -1)
        {
            Array.Resize(ref students, students.Length + 1);
            index = students.Length - 1;
            students[index].LastName = lastName;
        }

        Console.Write("Введіть оцінку з екзамену 1: ");
        students[index].Exam1 = GetValidGrade();

        Console.Write("Введіть оцінку з екзамену 2: ");
        students[index].Exam2 = GetValidGrade();

        Console.Write("Введіть оцінку з екзамену 3: ");
        students[index].Exam3 = GetValidGrade();


        Console.WriteLine("Інформацію успішно змінено.");
        return students;
    }


    static int GetValidGrade()
    {
        int grade;
        while (!int.TryParse(Console.ReadLine(), out grade) || grade < 0 || grade > 100)
        {
            Console.WriteLine("Неправильний ввід. Будь ласка, введіть ціле число від 0 до 100.");
            Console.Write("Введіть оцінку: ");
        }

        return grade;
    }

    public static Student[] Remove(Student[] students)
    {
        Console.Write("Введіть прізвище студента, якого ви хочете вилучити: ");
        string lastName = Console.ReadLine();

        int index = Array.FindIndex(students, s => s.LastName == lastName);

        if (index != -1)
        {
            students = students.Where((s, i) => i != index).ToArray();
            Console.WriteLine("Студента вилучено зі списку.");
        }
        else
        {
            Console.WriteLine("Студент з таким прізвищем не знайдений.");
        }

        return students;
    }

    static int GetSubjectChoice()
    {
        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 3)
        {
            Console.WriteLine("Неправильний ввід. Будь ласка, введіть 1, 2 або 3.");
            Console.Write("Введіть номер предмету: ");
        }

        return choice;
    }

    static int GetSubjectGrade(Student student, int subjectChoice)
    {
        switch (subjectChoice)
        {
            case 1:
                return student.Exam1;
            case 2:
                return student.Exam2;
            case 3:
                return student.Exam3;
            default:
                return 0;
        }
    }
}