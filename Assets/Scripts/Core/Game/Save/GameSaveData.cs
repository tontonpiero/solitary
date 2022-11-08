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
        public float Time;

        public DeckData SData; // stock deck
        public DeckData RData; // reserve deck
        public DeckData[] FData; // foundation decks
        public DeckData[] CData; // column decks

        public MoveCommandData[] Cmds;
    }

    [Serializable]
    public struct DeckData
    {
        public List<CardData> Cards;
    }

    [Serializable]
    public struct MoveCommandData
    {
        public string Src; // source deck name
        public int SrcIdx; // source deck index
        public string Dest; // destination deck name
        public int DestIdx; // destination deck index
        public int Amount; // amount of cards
        public int PrevScore; // previous score
        public bool Reverse; // reverse moved cards
        public bool CarRev; // was card revealed
    }

    [Serializable]
    public struct CardData
    {
        public CardData(CardRank rank, CardSuit suit, bool isVisible)
        {
            R = rank;
            S = suit;
            V = isVisible;
        }

        public CardRank R; // rank
        public CardSuit S; // suit
        public bool V; // is visible
    }
}
