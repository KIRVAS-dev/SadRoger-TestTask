namespace Core.Gameplay.GameFlow
{
    public interface IGameFlowService
    {
        void PrepareGame();
        void StartGame();
        void FinishGame(GameState result);
    }
}
