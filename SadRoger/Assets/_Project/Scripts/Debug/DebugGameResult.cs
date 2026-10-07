#if UNITY_EDITOR
using Core.Gameplay.GameFlow;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace ProjectDebug
{
    internal sealed class DebugGameResult : MonoBehaviour
    {
        [SerializeField] private Key _winHotkey = Key.W;
        [SerializeField] private Key _loseHotkey = Key.L;
        [SerializeField] private LifetimeScope _coreScope;

        private IGameFlowService _gameFlowService;
        private IReadOnlyGameStateModel _gameStateModel;
        private bool _isInjected;

        [Inject]
        private void Construct(IGameFlowService gameFlowService, IReadOnlyGameStateModel gameStateModel)
        {
            _gameFlowService = gameFlowService;
            _gameStateModel = gameStateModel;
            _isInjected = true;
        }

        private void Start()
        {
            if (_coreScope == null)
            {
                return;
            }

            _coreScope.Container.InjectGameObject(gameObject);
        }

        private void Update()
        {
            if (!_isInjected
             || _gameStateModel.State.CurrentValue != GameState.Playing)
            {
                return;
            }

            if (DebugHotkey.WasPressedThisFrame(_winHotkey))
            {
                _gameFlowService.FinishGame(GameState.Win);
            }
            else if (DebugHotkey.WasPressedThisFrame(_loseHotkey))
            {
                _gameFlowService.FinishGame(GameState.Lose);
            }
        }
    }
}
#endif
