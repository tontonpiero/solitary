using System;
using System.Collections.Generic;

namespace Solitary.Core
{
    [Serializable]
    public struct GameSaveData
    {
        public int Id;
        public int Score;
        public int Moves;
        public float TotalTime;

        public DeckData sData; // stock deck
        public DeckData wData; // Reserve deck
        public DeckData[] fData; // foundation decks
        public DeckData[] cData; // column decks
    }

    [Serializable]
    public struct DeckData
    {
        public List<CardData> Cards;
    }

    [Serializable]
    public struct CardData
    {
        public CardData(CardRank rank, CardSuit suit, bool isVisible)
        {
            r = rank;
            s = suit;
            v = isVisible;
        }

        public CardRank r; // rank
        public CardSuit s; // suit
        public bool v; // isVisible
    }
}
