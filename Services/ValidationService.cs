using System.ComponentModel;
using System.Globalization;
using PersonPlacementSystem.ConsoleManagment;

namespace PersonPlacementSystem.Validation;

public interface IValidatorService
{
    public bool InputIsValid(Type targetType, string? input, out object? result, bool debugMode = false);
    public bool InputIsValid<T>(string? input, out T result, bool debugMode = false);
    public bool InputIsValid(Type targetType, string? input, bool debugMode = false);
    public bool InputIsValid<T>(string? input, bool debugMode = false);
    public bool InputIsBoolFormat(string? input, string[] trueWords, string[] falseWords);

    public void ProcessValidation(Type targetType, out object? result, bool debugMode = false);
    public void ProcessInputValidation<T>(out T result, bool debugMode = false);
}

public class ValidatorService(ILogger logger) : IValidatorService
{
    public bool InputIsValid(Type targetType, string? input, out object? result, bool debugMode = false)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(input))
        {
            if (!debugMode)
                logger.PushError($"Ошибка ввода");
            else
                logger.PushError($"Ошибка ввода at ValidatorService InputIsValide<T>(); line 40: ");
            return false;
        }

        try
        {
            if (targetType == typeof(DateTime))
            {
                DateTime dateTime = new();
                bool success = DateTime.TryParseExact(input, "dd:HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime)!;
                if (success)
                {
                    result = dateTime;
                }

                return success;
            }
            result = TypeDescriptor.GetConverter(targetType).ConvertFromString(input)!;
        }
        catch (Exception e)
        {
            if (!debugMode)
                logger.PushError($"Ошибка ввода");
            else
                logger.PushError($"Ошибка ввода: {e}");
            return false;
        }

        return true;
    }
    public bool InputIsValid<T>(string? input, out T result, bool debugMode = false)
    {
        result = default!;

        if (string.IsNullOrWhiteSpace(input))
        {
            if (!debugMode)
                logger.PushError($"Ошибка ввода");
            else
                logger.PushError($"Ошибка ввода at ValidatorService InputIsValide<T>(); line 40: ");
            return false;
        }

        try
        {
            if (typeof(T) == typeof(DateTime))
            {
                DateTime dateTime = new();
                bool success = DateTime.TryParseExact(input, "dd:HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime)!;
                if (success)
                {
                    result = (T)(object)dateTime;
                }
                else
                {
                    logger.PushError("Некорректный ввод. Повторите попытку.");
                }

                return success;
            }
            result = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(input)!;
        }
        catch (Exception e)
        {
            if (!debugMode)
                logger.PushError($"Ошибка ввода");
            else
                logger.PushError($"Ошибка ввода: {e}");

            return false;
        }
        return true;
    }
    public bool InputIsValid(Type targetType, string? input, bool debugMode = false)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;

        try
        {
            TypeDescriptor.GetConverter(targetType).ConvertFromString(input!);
        }
        catch (Exception e)
        {
            if (!debugMode)
                logger.PushError($"Ошибка ввода");
            else
                logger.PushError($"Ошибка ввода: {e}");
            return false;
        }

        return true;
    }
    public bool InputIsValid<T>(string? input, bool debugMode = false)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;

        try
        {
            TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(input);
        }
        catch (Exception e)
        {
            if (!debugMode)
                logger.PushError($"Ошибка ввода");
            else
                logger.PushError($"Ошибка ввода: {e}");

            return false;
        }
        return true;
    }

    public bool InputIsBoolFormat(string? input, string[] trueWords, string[] falseWords)
    {
        if (trueWords.Contains(input))
            return true;

        if (falseWords.Contains(input))
            return false;

        logger.PushError("Некорректный ввод");
        return false;
    }

    public void ProcessValidation(Type targetType, out object? result, bool debugMode = false)
    {
        while (true)
        {
            string? input = Console.ReadLine();
            if (!InputIsValid(input, out result, debugMode))
            {
                continue;
            }
            if (targetType.IsEnum)
            {
                InputIsValid(input, out int e, debugMode);
                if (!Enum.IsDefined(targetType, e!))
                {
                    logger.PushWarning("Выбранное перечисление не определенно");
                    continue;
                }
            }
            return;
        }
    }
    public void ProcessInputValidation<T>(out T result, bool debugMode = false)
    {
        while (true)
        {
            string? input = Console.ReadLine();
            if (!InputIsValid(input, out result, debugMode))
            {
                continue;
            }
            if (typeof(T).IsEnum)
            {
                InputIsValid(input, out int e, debugMode);
                if (!Enum.IsDefined(typeof(T), e!))
                {
                    logger.PushWarning("Выбранное перечисление не определенно");
                    continue;
                }
            }
            return;
        }
    }
}
