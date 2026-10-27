using Game.Domain;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class PlayerDecisionTest
{
    [TestCase(PlayerDecision.Rock, PlayerDecision.Scissors)]
    [TestCase(PlayerDecision.Scissors, PlayerDecision.Paper)]
    [TestCase(PlayerDecision.Paper, PlayerDecision.Rock)]
    public void GetLoosingDecision_ReturnsCorrectDecision(PlayerDecision currentDecision, PlayerDecision expectedDecision)
    {
        Assert.AreEqual(expectedDecision, currentDecision.GetLoosingDecision());
    }
}