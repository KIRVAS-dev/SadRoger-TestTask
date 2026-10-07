namespace Core.Gameplay.GameFlow
{
    public sealed class GameFlowService : IGameFlowService
    {
        private readonly IGameStateMachine _gameStateMachine;

        public GameFlowService(IGameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        void IGameFlowService.PrepareGame()
        {
            _gameStateMachine.EnterState(GameState.Ready);
        }

        void IGameFlowService.StartGame()
        {
            _gameStateMachine.EnterState(GameState.Playing);
        }

        void IGameFlowService.FinishGame(GameState result)
        {
            _gameStateMachine.EnterState(result);
        }
    }
}
