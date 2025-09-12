using System.Numerics;
using PersonPlacementSystem.ConsoleManagment;
using PersonPlacementSystem.Places;
using PersonPlacementSystem.Animation;
using PersonPlacementSystem.Interfaces.PersonInterfaces;
using PersonPlacementSystem.Validation;

namespace PersonPlacementSystem.Persons;

public interface IPerson
{
    string PersonName { get; }
    int FriendIndex { get; }
    DateTime RequestTime { get; }
    bool IsCorrect { get; }
    int? PersonPriority { get; }
    Vector2 Coords { get; }
    int Variant { get; }

    void SetPersonName(string name);

    void SetFriendIndex(int index);

    void SetRequestTime(DateTime dateTime);

    void SetPersonCorrect(bool areCorrect);
    void SetPersonPriority(int? priority);

    void SetPersonCoords(Vector2 coords);

    void SetPersonVariant(int variant);
}

public class Person(string personName, int friendIndex, DateTime requestTime, bool isCorrect, int? personPriority, Vector2 coords, int variant = 1) : IPerson
{
    public enum PersonRegistrationType
    {
        Priority,
        Order
    }

    public string PersonName { get; private set; } = personName;
    public int FriendIndex { get; private set; } = friendIndex;
    public DateTime RequestTime { get; private set; } = requestTime;
    public bool IsCorrect { get; private set; } = isCorrect;
    public int? PersonPriority { get; private set; } = personPriority;
    public Vector2 Coords { get; private set; } = coords;
    public int Variant { get; private set; } = variant;

    public void SetPersonName(string name)
    {
        //TODO сделать валидацию имени в отдельном классе
        PersonName = name;
    }

    public void SetFriendIndex(int index)
    {
        FriendIndex = index;
    }

    public void SetRequestTime(DateTime dateTime)
    {
        RequestTime = dateTime;
    }

    public void SetPersonCorrect(bool areCorrect)
    {
        IsCorrect = areCorrect;
    }

    public void SetPersonPriority(int? priority)
    {
        PersonPriority = priority;
    }

    public void SetPersonCoords(Vector2 coords)
    {
        Coords = coords;
    }

    public void SetPersonVariant(int variant)
    {
        Variant = variant;
    }

    public override string ToString() => $"({PersonName}, {FriendIndex}, {RequestTime:dd:HH:mm}, {IsCorrect}, {Coords}, {Variant}, {PersonPriority})";
}

public class PersonRegistrationService(ILogger logger, IValidatorService validator) : IPersonRegistrationService
{
    readonly ILogger _logger = logger;
    readonly IValidatorService _validator = validator;

    public Person ProcessPersonRegistration(Person.PersonRegistrationType type, int displayNumber = 1, bool extraData = true, bool debugMode = false)
    {
        int personFriendIndex = 0;
        int personVariant = 1;
        bool personRequestAreCorrect = false;
        Vector2 personCoords = new();

        Console.WriteLine($"Введите имя и фамилию человека №{displayNumber + 1}");

        _validator.ProcessInputValidation<string>(out string? personName, debugMode);

        DateTime localTime = DateTime.Now;
        Console.WriteLine($"Введите время запроса человека(дд:чч:мм -- {localTime:dd:HH:mm})");

        _validator.ProcessInputValidation(out DateTime personRequestTime, debugMode);

        if (extraData)
        {
            Console.WriteLine($"Введите ряд, парту и вариант человека:");

            Console.WriteLine("Ряд");

            _validator.ProcessInputValidation(out int x, debugMode);

            Console.WriteLine("Парта");

            _validator.ProcessInputValidation(out int y, debugMode);

            Console.WriteLine("Вариант (1 или 2)");

            _validator.ProcessInputValidation(out int v, debugMode);

            v = Math.Clamp(v, 1, 2);

            personCoords = new(x, y);
            personVariant = v;
        }

        if (type == Person.PersonRegistrationType.Priority)
        {
            Console.WriteLine($"{personName} правильно написал(а) запрос на место? (Да/Нет)");

            string[] trueWords = ["yes", "да", "y", "д"];
            string[] falseWords = ["no", "нет", "n", "н"];

            //*не знаю как сократить, при этом не засорив класс валидатора
            bool inputIsCorrect = false;
            while (inputIsCorrect == false)
            {
                if (!_validator.InputIsValid(Console.ReadLine(), out string output, debugMode))
                {
                    _logger.PushError("Некорректный ввод");
                    continue;
                }

                //TODO придумать сокращение и вынести в валидатор все таки

                output = output.Trim().ToLower();

                personRequestAreCorrect = _validator.InputIsBoolFormat(output, trueWords, falseWords);

                inputIsCorrect = trueWords.Contains(output) || falseWords.Contains(output);
            }
            //*

            Console.WriteLine("Введите индекс дружбы от -3(ненавижу) до 3(0 = ну нормально)");
            _validator.ProcessInputValidation(out personFriendIndex, debugMode);

            personFriendIndex = Math.Clamp(personFriendIndex, -3, 3);
        }

        Person person = new(personName, personFriendIndex, personRequestTime, personRequestAreCorrect, 0, personCoords, personVariant);

        Console.WriteLine($"{person.PersonName} был(а) успешно зарегистрирована(а)");

        return person;

    }
}

public class PersonCountService(ILogger logger, IValidatorService validator) : IPersonsAmountService
{
    readonly ILogger _logger = logger;
    readonly IValidatorService _validator = validator;

    public int ProcessPersonsAmount(bool debugMode = false)
    {
        Console.WriteLine("Введите количество человек");
        _validator.ProcessInputValidation<int>(out int peopleCount, debugMode);
        // while (isCorrect == false)
        // {
        //     string? peopleCountArg = Console.ReadLine();

        //     //обработка кол-ва человек
        //     if (!int.TryParse(peopleCountArg, out peopleCount))
        //     {
        //         _logger.PushError($"Возможно вы допустили ошибку в оформлении аргумента <<{peopleCountArg}>>, убедитесь, что ввод является натуральным числом!");
        //         ConsoleLineAnimation.PlayAnimation("Перезапуск", ConsoleLineAnimation.AnimationType.Dots, 500, 1);
        //     }
        //     else
        //     {
        //         isCorrect = true;
        //         Console.Clear();
        //     }

        // }
        return peopleCount;
    }
}

public class PersonPlacementService : IPersonPlacementService
{
    public List<Place> GetPersonsPlaces(ILogger logger, List<IPerson> persons)
    {
        if (persons.Count <= 1)
        {
            logger.PushError($"Количество человек: {persons.Count}. Невозможно определить места для нескольких человек, если количество человек <2");
            return new();
        }

        Dictionary<Vector2, List<Person>> groups = [];

        foreach (Person person in persons)
        {
            Vector2 curCoords = person.Coords;

            if (!groups.ContainsKey(curCoords))
            {
                groups[curCoords] = [];
            }

            groups[curCoords].Add(person);
        }

        List<Place> allPlaces = [];

        foreach (var group in groups)
        {
            List<Person> personsInGroup = group.Value;
            Vector2 coords = group.Key;

            allPlaces.Add(new Place((int)coords.X, (int)coords.Y, personsInGroup));

        }

        return allPlaces;
    }
}

public interface IPersonSortingService
{
    public List<IPerson> SortPersonsPriority(List<IPerson> from);
    public List<IPerson> SortPersonsOrder(List<IPerson> from, bool debugMode = false);
}

public class PersonSortingService : IPersonSortingService
{
    public List<IPerson> SortPersonsPriority(List<IPerson> from)
    {
        List<IPerson> sortedPersons = [.. from];

        sortedPersons = from.OrderByDescending(p => p.PersonPriority).ToList();

        return sortedPersons;
    }

    public List<IPerson> SortPersonsOrder(List<IPerson> from, bool debugMode = false)
    {
        List<IPerson> sortedPersons = [.. from];
        // Person temp;

        // for (int j = 0; j <= from.Count - 1; j++)
        // {
        //     for (int i = 0; i < from.Count - 1; i++)
        //     {
        //         // string time1 = $"{from[i].RequestTime.Day}{from[i].RequestTime.Hour}{from[i].RequestTime.Minute:D2}";
        //         // string time2 = $"{from[i + 1].RequestTime.Day}{from[i + 1].RequestTime.Hour}{from[i + 1].RequestTime.Minute:D2}";
        //         if (sortedPersons[i].RequestTime < sortedPersons[i + 1].RequestTime)
        //         {
        //             temp = from[i + 1];
        //             sortedPersons[i + 1] = sortedPersons[i];
        //             sortedPersons[i] = temp;
        //         }
        //     }
        // }

        sortedPersons = from.OrderByDescending(p => p.RequestTime).ToList();

        return sortedPersons;
    }
}
