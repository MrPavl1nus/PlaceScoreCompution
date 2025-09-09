using PersonPlacementSystem;
using PersonPlacementSystem.Persons;
using PersonPlacementSystem.State;
using PersonPlacementSystem.ConsoleManagment;
using PersonPlacementSystem.Animation;
using PersonPlacementSystem.Interfaces.PersonInterfaces;
using PersonPlacementSystem.Validation;

namespace PersonPlacementSystem.Places;

public interface IPlace
{
    int X { get; }
    int Y { get; }

    public IReadOnlyList<Person> Persons { get; }

    public string ToString();
}
public class Place(int x, int y, List<Person> persons) : IPlace
{
    public int X { get; } = x;
    public int Y { get; } = y;

    private readonly List<Person> _persons = persons;

    public IReadOnlyList<Person> Persons => _persons.AsReadOnly();

    public override string ToString() => $"X:{X}, Y:{Y}, Persons:{_persons.ToString}";
}

public class PlaceService
{
    readonly ILogger _logger;
    readonly IPersonRegistrationService _personRegistrationService;
    readonly IPersonSortingService _personsSortingService;
    readonly IValidatorService _validator;

    public PlaceService(ILogger logger, IPersonRegistrationService personRegistrationService, IPersonSortingService personsSortingService, IValidatorService validator)
    {
        _logger = logger;
        _personRegistrationService = personRegistrationService;
        _personsSortingService = personsSortingService;
        _validator = validator;
    }

    public void ProcessPlaces(ProgramState programMode, int mode = 1, bool debugMode = false)
    {
        bool isCorrect = false;
        do
        {
            if (mode == 1)
            {
                Console.WriteLine("Сколько человек претендует на место?");
                _validator.ProcessInputValidation(out programMode.PeopleCount, debugMode);

                Thread.Sleep(1000);

                if (mode == 1) programMode.NumberOfPlaces = 1;
                for (int i = 1; i <= programMode.NumberOfPlaces; i++)
                {
                    List<Person> neighboors = [];

                    Console.WriteLine($"Парта №{i}");

                    Console.WriteLine("введите ряд:");
                    _validator.ProcessInputValidation(out int x, debugMode);

                    Console.WriteLine("введите парту(сверху вниз):");
                    _validator.ProcessInputValidation(out int y, debugMode);

                    Console.WriteLine("Введите кол-во человек за партой(от 0 до 2):");
                    _validator.ProcessInputValidation(out int personAmount, debugMode);

                    for (int p = 0; p < programMode.NumberOfPlaces; p++)
                    {
                        Person person = _personRegistrationService.ProcessPersonRegistration(Person.PersonRegistrationType.Priority, p, false, debugMode);

                        neighboors.Add(person);
                    }

                    Place place = new(x, y, neighboors);

                    programMode.AddPlace(place);
                    isCorrect = true;

                }
            }

            else if (mode == 2)
            {
                for (int p = 0; p < programMode.PeopleCount; p++)
                {
                    //TODO вынести регистрацию списка в person service, а не в place service
                    programMode.Persons.Add(_personRegistrationService.ProcessPersonRegistration(Person.PersonRegistrationType.Priority, p, true, debugMode));
                }

                // places.Add(place);
                isCorrect = true;
            }

        } while (isCorrect == false);
    }

    public void PrintPlace(Place place)
    {
        List<IPerson> sortedPersons = _personsSortingService.SortPersonsPriority([.. place.Persons]);

        List<IPerson> passedPersons = [];
        List<IPerson> notPassedPersons = [];

        for (int i = 0; i < Math.Clamp(place.Persons.Count, 0, 2); i++)
        {
            passedPersons.Add(sortedPersons[i]);
        }

        for (int i = 2; i < sortedPersons.Count; i++)
        {
            notPassedPersons.Add(sortedPersons[i]);
        }



        Console.BackgroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"Место на {place.X} ряду, {place.Y} парта:");
        Console.ResetColor();

        List<Person> writedPersons = [];
        foreach (Person passed in passedPersons)
        {
            if (writedPersons.Contains(passed)) continue;
            Console.WriteLine($"   {passed} = {passed.PersonPriority}");
            writedPersons.Add(passed);
        }

        if (notPassedPersons.Count > 0)
        {
            Console.WriteLine("------------------------ \n не подходят на место:");

            foreach (Person other in notPassedPersons)
            {
                if (writedPersons.Contains(other)) continue;
                _logger.LogColored(ConsoleColor.DarkRed, $"   {other} = {other.PersonPriority}", ConsoleColor.Gray);
                writedPersons.Add(other);

            }
        }
        Console.ResetColor();


        Console.WriteLine();
    }

    //?Нужен ли этот метод?
    // public void ProcessPlacesAmount(ProgramState programState)
    // {
    //     bool isCorrect = false;
    //     do
    //     {
    //         Console.WriteLine("Введите количество парт");
    //         string? numberOfPlacesArg = Console.ReadLine(); //24

    //         if (int.TryParse(numberOfPlacesArg, out programState.NumberOfPlaces))
    //         {
    //             isCorrect = true;
    //             Console.Clear();
    //         }
    //         else
    //         {
    //             _logger.PushError($"Возможно вы допустили ошибку в оформлении аргумента <<{numberOfPlacesArg}>>, убедитесь, что ввод является натуральным числом!");

    //             ConsoleLineAnimation.PlayAnimation("Перезапуск", ConsoleLineAnimation.AnimationType.Dots, 1, 500);
    //         }
    //     } while (!isCorrect);
    // }
}