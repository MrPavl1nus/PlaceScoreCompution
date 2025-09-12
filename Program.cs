using PersonPlacementSystem.Places;
using PersonPlacementSystem.Persons;
using PersonPlacementSystem.State;
using PersonPlacementSystem.Animation;
using PersonPlacementSystem.ConsoleManagment;
using PersonPlacementSystem.ProgramModeManagement;
using PersonPlacementSystem.Computation;
using Microsoft.Extensions.DependencyInjection;
using PersonPlacementSystem.Interfaces.PersonInterfaces;
using PersonPlacementSystem.Validation;

namespace PersonPlacementSystem;

public class ScoreCompution
{
    #region Main

    static ILogger? consoleLogger;
    static IPersonSortingService? personsSortingService;
    static PlaceService? placeService;
    static PriorityCalculator? priorityCalculator;
    static ProgramModeManager? programModeManager;
    static IPersonPlacementService? personsPlacementService;
    static IPersonRegistrationService? personRegistrationService; //? Нужен ли он?
    static IValidatorService? validator;
    static ProgramState programState = new();

    static bool isStartedWithArgument = false;
    static bool isDebug = false;
    static bool isSkipMode = false;

    static void Main(string[] args)
    {
        IServiceCollection services = new ServiceCollection();


        services.AddSingleton<ILogger, ConsoleLogger>();
        services.AddSingleton<IValidatorService, ValidatorService>();
        services.AddSingleton<PlaceService>();

        services.AddTransient<IPersonSortingService, PersonSortingService>();
        services.AddTransient<IPersonRegistrationService, PersonRegistrationService>();
        services.AddTransient<PriorityCalculator>();
        services.AddTransient<IPersonsAmountService, PersonCountService>();
        services.AddTransient<ProgramModeManager>();
        services.AddTransient<IPersonPlacementService, PersonPlacementService>();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        validator = serviceProvider.GetRequiredService<IValidatorService>();
        personsSortingService = serviceProvider.GetRequiredService<IPersonSortingService>();
        placeService = serviceProvider.GetRequiredService<PlaceService>();
        priorityCalculator = serviceProvider.GetRequiredService<PriorityCalculator>();
        consoleLogger = serviceProvider.GetRequiredService<ILogger>();
        personRegistrationService = serviceProvider.GetRequiredService<IPersonRegistrationService>();
        programModeManager = serviceProvider.GetRequiredService<ProgramModeManager>();
        personsPlacementService = serviceProvider.GetRequiredService<IPersonPlacementService>();

        isStartedWithArgument = args.Length > 0;
        isDebug = isStartedWithArgument && args[0] == "Debug";
        isSkipMode = isStartedWithArgument && args.Length > 1 && args[1] == "SkipAll";

        RunProgram(args);
    }

    private static void RunProgram(string[] args)
    {
        consoleLogger!.LogColored(ConsoleColor.DarkGreen, "Добро пожаловать в великий компутатор приоритета 3000 v0.3", ConsoleColor.Black);

        if (isStartedWithArgument)
        {
            consoleLogger.LogColored(ConsoleColor.DarkRed, $"количество введеных аргументов: {args.Length}, аргументы: {string.Join(", ", args)}");
        }

        consoleLogger.Log("");

        Thread.Sleep(1000);

        Console.WriteLine($@"Выберите режим расчета: 
        1 = PlaceBattle(На переработке)
        2 = PlacesBattle
        3 = SortPersons");

        //TODO сделать реализацию валидации Enum
        validator!.ProcessInputValidation(out ProgramModeManager.ProgramMode modeInput, isDebug);

        switch (modeInput)
        {
            case ProgramModeManager.ProgramMode.PlaceBattle:
                consoleLogger.Log("Вы выбрали зарубу за одну парту. Вам придется повторять операции для каждой парты");
                ConsoleLineAnimation.PlayAnimation("Подготовка", ConsoleLineAnimation.AnimationType.Propeller, 3, 500);
                if (!isDebug)
                {
                    consoleLogger.PushWarning("Режим временно не доступен");
                    break;
                }

                placeService!.ProcessPlaces(programState, 1, isDebug);

                break;

            case ProgramModeManager.ProgramMode.PlacesBattle:
                programModeManager!.ProcessPersonsPlaces(programState, isDebug);

                break;

            case ProgramModeManager.ProgramMode.SortPersons:
                programModeManager!.SortPersonsMode(programState, isDebug);

                break;

        }

        ConsoleLineAnimation.PlayAnimation("Считаю приоритет", ConsoleLineAnimation.AnimationType.Propeller, 2, 500);

        if (isDebug)
        {
            Console.WriteLine($"количество человек введено: {programState.PeopleCount}");
            Console.WriteLine($"количество человек: {programState.Persons.Count}");
        }

        consoleLogger.Log("");

        List<IPerson> personsWithPriority = [.. programState.Persons];
        personsWithPriority.ForEach(person => person.SetPersonPriority(priorityCalculator!.ComputePersonPriority(person, programState.Persons, isDebug)));


        foreach (Person sortedPerson in personsWithPriority)
        {
            consoleLogger.PushWarning(sortedPerson.ToString());
        }

        consoleLogger.Log("");

        consoleLogger.Log("исходный список");
        foreach (Person person in programState.Persons)
        {
            consoleLogger.PushWarning(person.ToString());
        }

        consoleLogger.Log("");

        //printing of all persons
        Console.WriteLine("Список отсортированных людей с приоритетом");
        foreach (Person person in personsSortingService!.SortPersonsPriority(personsWithPriority))
        {
            Console.WriteLine($" Данные человека {person}. Приоритет = {person.PersonPriority}");
        }

        consoleLogger.Log("");

        List<Place> personsPlaces = personsPlacementService!.GetPersonsPlaces(consoleLogger, personsWithPriority);
        //printing places of persons
        for (int i = 0; i < personsPlaces.Count; i++)
            placeService!.PrintPlace(personsPlaces[i]);
    }

    static void SkipAll(ILogger logger, int columnsCount = 1, int rowsCount = 4, int personCount = 2)
    {
        logger.PushWarning("Временно не работает");

        //TODO: Переписать метод для отладки. Цель Метода пропуск всех шагов, автоматически запонив все необходимы формы.
    }
}
#endregion
