using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Domain;

public class InMemoryGameTurnRepository : IGameTurnRepository
{
    private Dictionary<Guid, GameTurnEntity> _entities = new();

    public void Insert(GameTurnEntity gameTurn)
    {
        _entities[gameTurn.TurnId] = gameTurn;
    }

    public IEnumerable<GameTurnEntity> GetLastTurns(Guid gameId)
    {
        Console.WriteLine($"{_entities.Count} turns");
        const int lastTrunsNumber = 5;
        return _entities.Values
            .Where(ent => ent.GameId == gameId)
            .OrderBy(ent => -ent.TurnNumber)
            .Take(lastTrunsNumber)
            .Reverse();
    }
}