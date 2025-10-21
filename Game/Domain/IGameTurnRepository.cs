using System;
using System.Collections.Generic;

namespace Game.Domain;

public interface IGameTurnRepository
{
    GameTurnEntity Insert(GameTurnEntity tour);
    IList<GameTurnEntity> GetGameTours(Guid gameId);
}