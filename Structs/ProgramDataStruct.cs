using System;
using PersonPlacementSystem.Persons;
using PersonPlacementSystem.Places;

namespace PersonPlacementSystem.Struct;

public struct ProgramDataStruct(List<Place> places)
{
    public readonly List<Place> Places = places;
}
