namespace PersonPlacementSystem.ConsoleManagment;

public interface ILogger
{
    public enum LogType
    {
        ErrorInvalidInput = 0,

    }

    void Log(object? message);
    void LogColored(ConsoleColor backgroundColor, string message, ConsoleColor foregroundColor = ConsoleColor.White);
    void PushError(string message);
    void PushWarning(string message);
}

public class ConsoleLogger :ILogger
{
    #region ConsoleUI

    public void Log(object? message)
    {
        Console.WriteLine(message + "\r");
    }

    public void PushError(string message)
    {
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.WriteLine("ERROR: " + message + "\r");
        
        Console.ResetColor();
    }

    public void PushWarning(string message)
    {
        Console.BackgroundColor = ConsoleColor.Yellow;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.WriteLine(message + "\r");
        
        Console.ResetColor();
    }

    public void LogColored(ConsoleColor backgroundColor, string message, ConsoleColor foregroundColor = ConsoleColor.White)
    {
        Console.BackgroundColor = backgroundColor;
        Console.ForegroundColor = foregroundColor;
        Console.WriteLine(message + "\r");
        
        Console.ResetColor();
    }
    #endregion

    public void Exit()
    {
        Console.Clear();
    }
}
