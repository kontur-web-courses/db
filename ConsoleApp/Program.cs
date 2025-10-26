using System;
using System.Linq;
using Game.Domain;
using MongoDB.Driver;

namespace ConsoleApp
{
    class Program
    {
        private readonly IUserRepository userRepo;
        private readonly IGameRepository gameRepo;
        private readonly IGameTurnRepository gameTurnRepo;
        private readonly Random random = new Random();

        private Program(string[] args, IMongoDatabase mongoDatabase)
        {
            userRepo = new MongoUserRepository(mongoDatabase);
            gameRepo = new MongoGameRepository(mongoDatabase);
            gameTurnRepo = new MongoGameTurnRepository(mongoDatabase);
        }

        public static void Main(string[] args)
        {
            var mongoConnectionString = Environment.GetEnvironmentVariable("PROJECT5100_MONGO_CONNECTION_STRING") ?? "mongodb://admin:admin@localhost:27017?maxConnecting=100";
            var mongoClient = new MongoClient(mongoConnectionString);
            var mongoDatabase = mongoClient.GetDatabase("game");
            new Program(args, mongoDatabase).RunMenuLoop();
        }

        private void RunMenuLoop()
        {
            var humanUser = userRepo.GetOrCreateByLogin("Human");
            var aiUser = userRepo.GetOrCreateByLogin("AI");
            var game = FindCurrentGame(humanUser) ?? StartNewGame(humanUser);
            if (!TryJoinToGame(game, aiUser))
            {
                Console.WriteLine("Can't add AI user to the game");
                return;
            }

            while (HandleOneGameTurn(humanUser.Id))
            { }

            Console.WriteLine("Game is finished");
            Console.ReadLine();
        }

        private GameEntity StartNewGame(UserEntity user)
        {
            Console.WriteLine("Enter desired number of turns in game:");
            if (!int.TryParse(Console.ReadLine(), out var turnsCount))
            {
                turnsCount = 5;
                Console.WriteLine($"Bad input. Use default value for turns count: {turnsCount}");
            }

            var game = new GameEntity(turnsCount);
            game.AddPlayer(user);
            var savedGame = gameRepo.Insert(game);

            user.CurrentGameId = savedGame.Id;
            userRepo.Update(user);

            return savedGame;
        }

        private bool TryJoinToGame(GameEntity game, UserEntity user)
        {
            if (IsUserInGame(user, game))
                return true;

            if (user.CurrentGameId.HasValue)
                return false;

            if (game.Status != GameStatus.WaitingToStart)
                return false;

            game.AddPlayer(user);
            if (!gameRepo.TryUpdateWaitingToStart(game))
                return false;

            user.CurrentGameId = game.Id;
            userRepo.Update(user);

            return true;
        }

        private static bool IsUserInGame(UserEntity user, GameEntity game)
        {
            return user.CurrentGameId.HasValue && user.CurrentGameId.Value == game.Id && game.Players.Any(p => p.UserId == user.Id);
        }

        private GameEntity FindCurrentGame(UserEntity humanUser)
        {
            if (humanUser.CurrentGameId == null) return null;

            var game = gameRepo.FindById(humanUser.CurrentGameId.Value);
            if (game == null) return null;

            switch (game.Status)
            {
                case GameStatus.WaitingToStart:
                case GameStatus.Playing:
                    return game;
                case GameStatus.Finished:
                case GameStatus.Canceled:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool HandleOneGameTurn(Guid humanUserId)
        {
            var game = GetGameByUser(humanUserId);

            if (game.IsFinished())
            {
                UpdatePlayersWhenGameFinished(game);
                return false;
            }

            PlayerDecision? decision = AskHumanDecision();
            if (!decision.HasValue)
                return false;

            game.SetPlayerDecision(humanUserId, decision.Value);

            var aiPlayer = game.Players.First(p => p.UserId != humanUserId);
            game.SetPlayerDecision(aiPlayer.UserId, GetAiDecision());

            if (game.HaveDecisionOfEveryPlayer)
            {
                var finishedTurn = game.FinishTurn();
                gameTurnRepo.Insert(finishedTurn);
            }

            ShowScore(game);
            gameRepo.Update(game);
            return true;
        }

        private GameEntity GetGameByUser(Guid userId)
        {
            var user = userRepo.FindById(userId) ?? throw new Exception($"Unknown user with id {userId}");
            var userCurrentGameId = user.CurrentGameId ?? throw new Exception($"No current game for user: {user}");
            return gameRepo.FindById(userCurrentGameId);
        }

        private PlayerDecision GetAiDecision()
        {
            return (PlayerDecision)Math.Min(3, 1 + random.Next(4));
        }

        private void UpdatePlayersWhenGameFinished(GameEntity game)
        {
            // Вместо этого кода можно написать специализированный метод в userRepo, который сделает все эти обновления за одну операцию UpdateMany.
            // Вместо 4 запросов к БД будет 1, но усложнится репозиторий. В данном случае, это редкая операция, поэтому нет смысла оптимизировать.
            foreach (var player in game.Players)
            {
                var playerUser = userRepo.FindById(player.UserId);
                if (playerUser == null) continue;

                playerUser.ExitGame();
                userRepo.Update(playerUser);
            }
        }

        private static PlayerDecision? AskHumanDecision()
        {
            Console.WriteLine();
            Console.WriteLine("Select your next decision:");
            Console.WriteLine("1 - Rock");
            Console.WriteLine("2 - Scissors");
            Console.WriteLine("3 - Paper");

            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.KeyChar == '1') return PlayerDecision.Rock;
                if (key.KeyChar == '2') return PlayerDecision.Scissors;
                if (key.KeyChar == '3') return PlayerDecision.Paper;
                if (key.Key == ConsoleKey.Escape) return null;
            }
        }

        private void ShowScore(GameEntity game)
        {
            var players = game.Players;
            Console.WriteLine($"Score: {players[0].Name} {players[0].Score} : {players[1].Score} {players[1].Name}");
            
            var lastTurns = gameTurnRepo.GetLastTurns(game.Id, 5);
            if (lastTurns.Any())
            {
                Console.WriteLine("\nLast turns:");
                
                var humanPlayer = players.First(p => p.UserId == players[0].UserId);
                var aiPlayer = players.First(p => p.UserId != humanPlayer.UserId);
        
                foreach (var turn in lastTurns.OrderBy(t => t.TurnIndex))
                {
                    Console.Write($"Turn {turn.TurnIndex + 1}: ");
            
                    var humanDecision = turn.Decisions[humanPlayer.UserId];
                    var aiDecision = turn.Decisions[aiPlayer.UserId];
            
                    Console.Write($"{humanPlayer.Name} - {humanDecision} vs {aiPlayer.Name} - {aiDecision}");
            
                    if (turn.WinnerId.HasValue)
                    {
                        var winner = players.First(p => p.UserId == turn.WinnerId.Value);
                        Console.Write($" → Winner: {winner.Name}");
                    }
                    else
                    {
                        Console.Write(" → Draw");
                    }
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
        }
    }
}