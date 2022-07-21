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
        private bool preventMove = false;

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
                preventMove = true;
                yield return new WaitForSeconds(0.5f);
                game.Deal();
                AudioManager.Instance.PlaySound("move_cards");
                yield return new WaitForSeconds(1f);
                preventMove = false;
            }
        }

        private void Update()
        {
            game?.Update(Time.unscaledDeltaTime);
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveGame();
            }
        }

        private void OnGameScoreChanged() => OnScoreChanged?.Invoke();

        private void OnGameMovesChanged() => OnMovesChanged?.Invoke();

        private void OnGameStateChanged(GameState newState) => OnStateChanged?.Invoke(newState);

        public void MoveCards(Deck source, Deck destination, int amount = 1)
        {
            if (preventMove) return;

            game.MoveCards(source, destination, amount);

            AudioManager.Instance.PlaySound("move_card");

            CheckEndGame();
        }

        private void CheckEndGame()
        {
            if (game.StockDeck.Count == 0 && game.ReserveDeck.Count == 0)
            {
                StartCoroutine(EndGame());
            }
        }

        private IEnumerator EndGame()
        {
            if (game.ResolveNextMove())
            {
                AudioManager.Instance.PlaySound("move_cards");
            }
            preventMove = true;
            while (game.ResolveNextMove())
            {
                yield return null;
            }
            preventMove = false;
        }

        public void UndoLastMove()
        {
            if (preventMove) return;

            if (game.UndoLastMove())
            {
                AudioManager.Instance.PlaySound("move_card");
            }

        }

        public void ResolveNextMove()
        {
            if (preventMove) return;

            if (game.ResolveNextMove())
            {
                AudioManager.Instance.PlaySound("move_card");
            }

            CheckEndGame();
        }

        public void Recycle()
        {
            if (preventMove) return;

            AudioManager.Instance.PlaySound("move_cards");
            game.Recycle();
        }

        public void PauseGame()
        {
            if (preventMove) return;

            game.Pause();
        }

        public void ResumeGame()
        {
            game.Resume();
        }

        public void SaveGame()
        {
            game.Save();
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
