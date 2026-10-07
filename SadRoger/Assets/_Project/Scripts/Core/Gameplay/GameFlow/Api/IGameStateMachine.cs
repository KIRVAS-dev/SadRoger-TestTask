namespace Core.Gameplay.GameFlow
{
    public interface IGameStateMachine
    {
        void EnterState(GameState state);
    }
}
