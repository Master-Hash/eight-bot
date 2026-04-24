using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class EightBotBrain : MonoBehaviour
    {
        [SerializeField] private TextAsset data8Json;

        private Data8Book _book;
        private readonly EightBotSolver _solver = new EightBotSolver();

        public EightGameState CurrentState => GameConfig.CurrentState;

        public void SetState(EightGameState state)
        {
            GameConfig.CurrentState = state;
        }

        public void ApplyMove(EightMove move)
        {
            GameConfig.CurrentState = EightGameRules.ApplyMove(GameConfig.CurrentState, move);
        }

        public EightMove? GetOptimalMove()
        {
            return _solver.GetBestMove(GameConfig.CurrentState);
        }

        public StrategyEvaluation EvaluateCurrentState()
        {
            return _solver.Evaluate(GameConfig.CurrentState);
        }

        public IReadOnlyList<EightMove> GetRuleBasedMoves()
        {
            return EightGameRules.GetLegalMoves(GameConfig.CurrentState);
        }

        public IReadOnlyList<EightGameState> GetAllPossibleNextStatesFromData8()
        {
            EnsureBookLoaded();
            if (_book == null)
            {
                return new List<EightGameState>();
            }

            return _book.GetPossibleNextStates(GameConfig.CurrentState);
        }

        public IReadOnlyCollection<EightGameState> GetAllStatesFromData8()
        {
            EnsureBookLoaded();
            return _book?.States ?? new List<EightGameState>();
        }

        public bool TryGetCurrentStateClassFromData8(out string stateClass)
        {
            EnsureBookLoaded();
            if (_book == null)
            {
                stateClass = null;
                return false;
            }

            return _book.TryGetClass(GameConfig.CurrentState, out stateClass);
        }

        private void EnsureBookLoaded()
        {
            if (_book != null) return;
            if (data8Json == null) return;

            try
            {
                _book = Data8Book.LoadFromText(data8Json.text);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to parse data8.json: {ex.Message}");
            }
        }
    }
}
