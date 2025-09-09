using PersonPlacementSystem.Places;
using PersonPlacementSystem.Persons;

namespace PersonPlacementSystem.State;


public class ProgramState
{
    public readonly List<Place> Places = [];
    public readonly List<IPerson> Persons = [];
    public int PeopleCount = 1;
    public int NumberOfPlaces = 22;

    public void AddPerson(IPerson person)
    {
        Persons.Add(person);
    }

    public void AddPlace(Place place)
    {
        Places.Add(place);
    }

    public void Reset()
    {
        Places.Clear();
        Persons.Clear();
        PeopleCount = 0;
        NumberOfPlaces = 0;
    }

    public IReadOnlyList<IPerson> GetPersons() => Persons.AsReadOnly();
    public IReadOnlyList<Place> GetPlaces() => Places.AsReadOnly();
}
