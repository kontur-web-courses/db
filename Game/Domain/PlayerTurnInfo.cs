using System;

namespace Game.Domain;
public class PlayerTurnInfo
{
    public Guid UserId { get; set; }
    public string PlayerName { get; set; }
    public PlayerDecision Decision { get; set; }

    public PlayerTurnInfo()
    {
    }

    public PlayerTurnInfo(Guid userId, string playerName, PlayerDecision decision)
    {
        UserId = userId;
        PlayerName = playerName;
        Decision = decision;
    }
}