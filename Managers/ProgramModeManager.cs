using PersonPlacementSystem.Persons;
using PersonPlacementSystem.State;
using PersonPlacementSystem.Animation;
using PersonPlacementSystem.Places;
using PersonPlacementSystem.Interfaces.PersonInterfaces;

namespace PersonPlacementSystem.ProgramModes;

public class ProgramModeManager
{
    readonly IPersonsAmountService _personsAmountService;
    readonly IPersonSortingService _personsSortingService;
    readonly IPersonRegistrationService _personRegistaritonService;
    readonly PlaceService _placeService;

    public ProgramModeManager(IPersonsAmountService personsAmountService, IPersonSortingService personsSortingService, IPersonRegistrationService personRegistrationService, PlaceService placeService)
    {
        _personsAmountService = personsAmountService;
        _personsSortingService = personsSortingService;
        _personRegistaritonService = personRegistrationService;
        _placeService = placeService;
    }

    public enum ProgramMode
    {
        PlaceBattle = 1,
        PlacesBattle = 2,
        SortPersons = 3
    }

    public void SortPersonsMode(ProgramState programState, bool debugMode = false)
    {
        Console.WriteLine("Вы вошли в режим сортировки людей по времени...");

        programState.PeopleCount = _personsAmountService.ProcessPersonsAmount();

        for (int i = 0; i < programState.PeopleCount; i++)
        {
            programState.AddPerson(_personRegistaritonService.ProcessPersonRegistration(Person.PersonRegistrationType.Order, i));
            ConsoleLineAnimation.PlayAnimation("Добавление в общий список", ConsoleLineAnimation.AnimationType.Dots, 1, 500);
        }

        Console.WriteLine();

        List<IPerson> sortedPersons = _personsSortingService.SortPersonsOrder(programState.Persons);

        for (int i = 0; i < sortedPersons.Count; i++)
        {
            Console.WriteLine($"{i + 1} {sortedPersons[i]}");
        }

        Console.WriteLine();
    }

    public void ProcessPersonsPlaces(ProgramState programState, bool debugMode)
    {
        programState.PeopleCount = _personsAmountService.ProcessPersonsAmount();

        _placeService.ProcessPlaces(programState, 2, debugMode);
    }
}
