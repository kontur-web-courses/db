using System;
using System.Collections.Generic;

namespace Game.Domain;

public interface IGameTurnRepository
{
    GameTurnEntity Insert(GameTurnEntity turn);
    IList<GameTurnEntity> FindLastTurns(Guid gameId, int limit);
}