using PersonPlacementSystem.Persons;
using PersonPlacementSystem.Places;
using PersonPlacementSystem.ConsoleManagment;
namespace PersonPlacementSystem.Interfaces.PersonInterfaces;

public interface IPersonRegistrationService
{
    Person ProcessPersonRegistration(Person.PersonRegistrationType type, int displayNumber = 1, bool extraData = true, bool debugMode = false);
}

public interface IPersonsAmountService
{
    int ProcessPersonsAmount();
}

public interface IPersonPlacementService
{
    List<Place> GetPersonsPlaces(ILogger logger, List<IPerson> persons);
}
public interface IPersonPriorityCalculationService
{
    int? ComputePersonPriority(Person person, List<Person> persons, bool debugMode = false);
}
