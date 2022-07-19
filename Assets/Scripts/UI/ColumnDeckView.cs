using Solitary.Core;
using Solitary.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Solitary
{
    public class ColumnDeckView : DeckView
    {
        private ColumnDeck columnDeck => Deck as ColumnDeck;

        public override void AddCardView(CardView cardView)
        {
            // hack to know if we have to hide previous card in case of undo
            // this scenario could probably be managed better
            if (CardViews.Count > 0)
            {
                CardView previousCardView = CardViews.Last();
                bool legitMove = columnDeck.CanStack(previousCardView.Card, cardView.Card);

                if (!legitMove)
                {
                    previousCardView.Hide();
                }
            }

            base.AddCardView(cardView);
        }
    }
}
