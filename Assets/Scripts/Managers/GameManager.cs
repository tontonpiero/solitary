using Solitary.Core;
using System;
using System.Collections;
using UnityEngine;

namespace Solitary.Manager
{
    public class GameManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private DeckManager deckManager;
        [SerializeField] private CardManager cardManager;

        public event Action OnScoreChanged;
        public event Action OnMovesChanged;
        public event Action<GameState> OnStateChanged;

        private Game game;
        private bool isEnding = false;

        private void Awake()
        {
            game = new Game.Builder().Build();

            game.OnScoreChanged += OnGameScoreChanged;
            game.OnMovesChanged += OnGameMovesChanged;
            game.OnStateChanged += OnGameStateChanged;
        }

        private IEnumerator Start()
        {
            game.Start();

            deckManager.InitializeDecks(game);

            if (game.IsNew)
            {
                yield return new WaitForSeconds(0.5f);
                game.Deal();
                AudioManager.Instance.PlaySound("move_cards");
            }
        }

        private void Update()
        {
            game?.Update(Time.unscaledDeltaTime);
        }

        private void OnGameScoreChanged() => OnScoreChanged?.Invoke();

        private void OnGameMovesChanged() => OnMovesChanged?.Invoke();

        private void OnGameStateChanged(GameState newState) => OnStateChanged?.Invoke(newState);

        public void MoveCards(Deck source, Deck destination, int amount = 1)
        {
            if (isEnding) return;

            game.MoveCards(source, destination, amount);

            AudioManager.Instance.PlaySound("move_card");

            CheckEndGame();
        }

        private void CheckEndGame()
        {
            if (game.StockDeck.Count == 0 && game.WasteDeck.Count == 0)
            {
                StartCoroutine(EndGame());
            }
        }

        private IEnumerator EndGame()
        {
            AudioManager.Instance.PlaySound("move_cards");
            isEnding = true;
            while (game.ResolveNextMove())
            {
                yield return null;
            }
            isEnding = false;

            if (game.State == GameState.Over)
            {
                Debug.Log("You win!");
            }
        }

        public void UndoLastMove()
        {
            if (isEnding) return;

            if (game.UndoLastMove())
            {
                AudioManager.Instance.PlaySound("move_card");
            }

        }

        public void ResolveNextMove()
        {
            if (isEnding) return;

            if (game.ResolveNextMove())
            {
                AudioManager.Instance.PlaySound("move_card");
            }

            CheckEndGame();
        }

        public void PauseGame()
        {
            if (isEnding) return;

            game.Pause();
        }

        public void ResumeGame()
        {
            game.Resume();
        }

        public int GetScore() => game.Score;

        public int GetMoves() => game.Moves;

        public float GetTotalTime() => game.TotalTime;

        private void OnDestroy()
        {
            game.Dispose();
            OnScoreChanged = null;
            OnMovesChanged = null;
            OnStateChanged = null;
        }
    }
}
