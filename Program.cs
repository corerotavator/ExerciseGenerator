using Microsoft.Extensions.Configuration;

namespace ExerciseGenerator;

internal class Program
{
    private static void Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        var mainProjectPath = configuration.GetSection("Paths:MainProjectPath").Value;
        var testProjectPath = configuration.GetSection("Paths:TestProjectPath").Value;

        var namespaceName = GetNamespace(mainProjectPath, testProjectPath);
        Console.WriteLine($"Selected namespace: {namespaceName}");

        Console.Write("Enter class name: ");
        var className = Console.ReadLine();

        CreateFiles(className, namespaceName, mainProjectPath, testProjectPath);

        Console.Write("New class is created. You can close the window.");
        Console.ReadLine();
    }

    private static string GetNamespace(string mainProjectPath, string testProjectPath)
    {
        var mainProjectFolders = Directory.GetDirectories(mainProjectPath);
        var folderNames = mainProjectFolders.Select(Path.GetFileName).ToList();

        Console.WriteLine("Select an existing namespace or enter a new one:");
        for (var i = 0; i < folderNames.Count; i++) Console.WriteLine($"{i + 1}. {folderNames[i]}");

        while (true)
        {
            var input = Console.ReadLine();
            if (int.TryParse(input, out var selectedIndex))
            {
                if (selectedIndex >= 1 && selectedIndex <= folderNames.Count)
                    return folderNames[selectedIndex - 1];
                Console.WriteLine("Invalid selection. Please choose a valid number or enter a new namespace.");
            }
            else
            {
                var newNamespace = input;

                // Create new namespace folders
                Directory.CreateDirectory(Path.Combine(mainProjectPath, newNamespace));
                Directory.CreateDirectory(Path.Combine(testProjectPath, newNamespace));

                return newNamespace;
            }
        }
    }

    private static void CreateFiles(string className, string namespaceName, string mainProjectPath,
        string testProjectPath)
    {
        var mainFile = Path.Combine(mainProjectPath, namespaceName, $"{className}.cs");
        var testFile = Path.Combine(testProjectPath, namespaceName, $"{className}Tests.cs");

        var mainClassTemplate = $@"
namespace {namespaceName}
{{
    public class {className}
    {{
        // Your code here
    }}
}}
";

        var testClassTemplate = $@"using Xunit;

namespace {namespaceName}.Tests
{{
    public class {className}Tests
    {{
        private readonly {className} _sut;
        public {className}Tests(){{
            _sut = new {className}();
        }}
        [Fact]
        public void Test1()
        {{
            //_sut.
        }}
    }}
}}
";

        File.WriteAllText(mainFile, mainClassTemplate);
        File.WriteAllText(testFile, testClassTemplate);

        Console.WriteLine($"Created {mainFile}");
        Console.WriteLine($"Created {testFile}");
    }
}