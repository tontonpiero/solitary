using Solitary.Core;
using Solitary.UI;
using System;
using System.Collections;
using UnityEngine;

namespace Solitary.Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private DeckManager deckManager;
        [SerializeField] private CardManager cardManager;

        public event Action OnScoreChanged;
        public event Action OnMovesChanged;

        private Game game;
        private bool isEnding = false;

        private void Awake()
        {
            game = new Game.Builder().Build();

            game.OnScoreChanged += OnGameScoreChanged;
            game.OnMovesChanged += OnGameMovesChanged;
        }

        private void OnGameScoreChanged() => OnScoreChanged?.Invoke();

        private void OnGameMovesChanged() => OnMovesChanged?.Invoke();

        private void Start()
        {
            StartCoroutine(InitializeGame());
        }

        private IEnumerator InitializeGame()
        {
            game.Start();

            deckManager.InitializeDecks(game);

            yield return new WaitForSeconds(0.5f);

            game.InitializeColumns();
        }

        public void MoveCards(Deck source, Deck destination, int amount = 1)
        {
            if (isEnding) return;

            game.MoveCards(source, destination, amount);

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

            game.UndoLastMove();
        }

        public void ResolveNextMove()
        {
            if (isEnding) return;

            game.ResolveNextMove();

            CheckEndGame();
        }

        public int GetScore() => game.Score;

        public int GetMoves() => game.Moves;

        private void OnDestroy()
        {
            game.Dispose();
            OnScoreChanged = null;
            OnMovesChanged = null;
        }
    }
}
