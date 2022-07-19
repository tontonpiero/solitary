using Solitary.Core;
using Solitary.UI;
using System.Collections;
using UnityEngine;

namespace Solitary.Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private DeckManager deckManager;
        [SerializeField] private CardManager cardManager;

        public Game Game { get; private set; }

        private void Awake()
        {
            Game = new Game.Builder().Build();
        }

        private void Start()
        {
            StartCoroutine(InitializeGame());
        }

        private IEnumerator InitializeGame()
        {
            Game.Start();

            deckManager.InitializeDecks(Game);

            yield return new WaitForSeconds(0.5f);

            Game.InitializeColumns();
        }

        public void UndoLastMove()
        {
            Game.UndoLastMove();
        }

        public void ResolveNextMove()
        {
            Game.ResolveNextMove();
        }
    }
}
