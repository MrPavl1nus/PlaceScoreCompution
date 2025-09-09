using PersonPlacementSystem.Interfaces.PersonInterfaces;
using PersonPlacementSystem.Persons;
using PersonPlacementSystem.ConsoleManagment;

namespace PersonPlacementSystem.Computation;

public class PriorityCalculator(IPersonSortingService sortingService, ILogger stateLogger)
{
    public int? ComputePersonPriority(IPerson person, List<IPerson> persons, bool debugMode = false)
    {
        int? priority = null;
        try
        {
            List<IPerson> sortedPersonsByOrder = sortingService.SortPersonsOrder(persons, debugMode);
            int sortedPersonOrder = sortedPersonsByOrder.IndexOf(person) + 1;

            priority = sortedPersonOrder + person.IsCorrect.GetHashCode() + person.FriendIndex;
        }
        catch (Exception e)
        {
            stateLogger.PushError($"ОШИБКА: что-то пошло не так! \n {e.Message}");
        }

        return priority;

    }
}
