namespace Game.Domain
{
    public interface IGameTurnRepository
    {
        public GameTurnEntity Insert(GameTurnEntity turn);
    }
}