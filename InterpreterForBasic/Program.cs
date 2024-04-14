namespace InterpreterForBasic;

internal class Program
{
    static Dictionary<int, string> labelTable = new Dictionary<int, string>();

    static void Main(string[] args)
    {
        string filePath = "assets/file/example.basic";

        try
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
            }
            else
            {
                Console.WriteLine($"Error: File does not exist at path {filePath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}