using NUnit.Framework;

namespace Laboratorna_9_3.Tests;

[TestFixture]
public class Tests
{
    [Test]
    public void Read()
    {
        var fileName = "testfile.txt";
        var fileContent = "Student1,90,85,92\nStudent2,78,90,88\n";
        File.WriteAllText(fileName, fileContent);

        var result = Program.Read(fileName);

        CollectionAssert.AreEqual(result,
            new Student[]
            {
                new Student { LastName = "Student1", Exam1 = 90, Exam2 = 85, Exam3 = 92 },
                new Student { LastName = "Student2", Exam1 = 78, Exam2 = 90, Exam3 = 88 }
            });

        File.Delete(fileName);
    }

    [Test]
    public void Write()
    {
        var fileName = "testfile.txt";
        var students = new Student[]
        {
            new Student { LastName = "Student1", Exam1 = 90, Exam2 = 85, Exam3 = 92 },
            new Student { LastName = "Student2", Exam1 = 78, Exam2 = 90, Exam3 = 88 }
        };

        Program.Write(fileName, students);

        var result = File.ReadAllText(fileName);
        Assert.AreEqual(result, "Student1,90,85,92\nStudent2,78,90,88\n");

        File.Delete(fileName);
    }
}