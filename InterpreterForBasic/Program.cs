﻿using InterpreterForBasic.Domain;

namespace InterpreterForBasic;

internal class Program
{
    static void Main(string[] args)
    {
        string filePath = "assets/file/example.basic";

        try
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                Lexer lexer = new Lexer();
                lexer.Tokenize(lines);

                foreach (var entry in lexer.ProgramLines)
                {
                    Console.WriteLine($"Line {entry.Key}: {entry.Value}");
                }
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