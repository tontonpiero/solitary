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
            Game = new Game.Builder()
                .WithDeckFactory(new Deck.Factory())
                .WithCommandInvoker(new CommandInvoker())
                .Build();
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
    }
}
