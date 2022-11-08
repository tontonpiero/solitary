using Solitary.Core;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Utils.Audio;

namespace Solitary.Manager
{
    public class GameManager : MonoBehaviour
    {
        private static int nextGameId = 0;

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

        public void RestartGame()
        {
            nextGameId = game.Id;
        }

        private IEnumerator Start()
        {
            game.Start(nextGameId);
            nextGameId = 0;

            deckManager.InitializeDecks(game);

            if (game.IsNew)
            {
                preventMove = true;
                yield return new WaitForSeconds(0.5f);
                AudioManager.PlaySound("move_cards");
                yield return new WaitForSeconds(1f);
                preventMove = false;
            }
        }

        private void Update()
        {
            game?.Update(Time.unscaledDeltaTime);

            if (Input.GetKeyDown(KeyCode.H)) ResolveNextMove();
            if (Input.GetKeyDown(KeyCode.U)) UndoLastMove();
            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("IsSolvable=" + game.IsSolvable());
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                PauseGame();
            }
        }

        private void OnGameScoreChanged() => OnScoreChanged?.Invoke();

        private void OnGameMovesChanged() => OnMovesChanged?.Invoke();

        private void OnGameStateChanged(GameState newState) => OnStateChanged?.Invoke(newState);

        public void MoveCards(Deck source, Deck destination, int amount = 1)
        {
            if (preventMove) return;

            game.MoveCards(source, destination, amount);

            AudioManager.PlaySound("move_card");

            CheckEndGame();
        }

        private void CheckEndGame()
        {
            if (IsReadyToEndGame())
            {
                StartCoroutine(EndGame());
            }
        }

        private bool IsReadyToEndGame()
        {
            if (game.StockDeck.Count > 0) return false;
            if (game.ReserveDeck.Count > 0) return false;
            foreach (ColumnDeck columnDeck in game.ColumnDecks)
            {
                if (columnDeck.GetCards(columnDeck.Count).Any(c => !c.IsRevealed)) return false;
            }
            return true;
        }

        private IEnumerator EndGame()
        {
            if (game.ResolveNextMove())
            {
                AudioManager.PlaySound("move_cards");
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
                AudioManager.PlaySound("move_card");
            }
        }

        public void ResolveNextMove()
        {
            if (preventMove) return;

            if (game.ResolveNextMove())
            {
                AudioManager.PlaySound("move_card");
            }

            CheckEndGame();
        }

        public void Draw()
        {
            if (preventMove) return;

            AudioManager.PlaySound("move_card");
            game.Draw();
        }

        public void Recycle()
        {
            if (preventMove) return;

            AudioManager.PlaySound("move_cards");
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
