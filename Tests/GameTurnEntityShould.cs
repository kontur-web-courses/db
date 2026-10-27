using System;
using FluentAssertions;
using Game.Domain;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class GameTurnEntityShould
    {
        [Test]
        public void PreserveDecisionsAndWinnerWhenTurnIsFinished()
        {
            var firstUser = new UserEntity(Guid.NewGuid(), "first", "", "", 0, null);
            var secondUser = new UserEntity(Guid.NewGuid(), "second", "", "", 0, null);
            var game = new GameEntity(3);
            game.AddPlayer(firstUser);
            game.AddPlayer(secondUser);
            game.SetPlayerDecision(firstUser.Id, PlayerDecision.Rock);
            game.SetPlayerDecision(secondUser.Id, PlayerDecision.Scissors);

            var turn = game.FinishTurn();

            turn.GameId.Should().Be(game.Id);
            turn.TurnIndex.Should().Be(1);
            turn.WinnerId.Should().Be(firstUser.Id);
            turn.Players.Should().BeEquivalentTo(
                new GameTurnPlayer(firstUser.Id, "first", PlayerDecision.Rock),
                new GameTurnPlayer(secondUser.Id, "second", PlayerDecision.Scissors));
            game.Players[0].Decision.Should().BeNull();
            game.Players[1].Decision.Should().BeNull();
        }

        [Test]
        public void StoreNoWinnerForDraw()
        {
            var firstUser = new UserEntity(Guid.NewGuid(), "first", "", "", 0, null);
            var secondUser = new UserEntity(Guid.NewGuid(), "second", "", "", 0, null);
            var game = new GameEntity(1);
            game.AddPlayer(firstUser);
            game.AddPlayer(secondUser);
            game.SetPlayerDecision(firstUser.Id, PlayerDecision.Paper);
            game.SetPlayerDecision(secondUser.Id, PlayerDecision.Paper);

            var turn = game.FinishTurn();

            turn.WinnerId.Should().BeNull();
            game.Status.Should().Be(GameStatus.Finished);
        }
    }
}
