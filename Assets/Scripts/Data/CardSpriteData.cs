using Solitary.Core;
using UnityEngine;

namespace Solitary.Data
{
    [CreateAssetMenu(fileName = "CardSpriteData")]
    public class CardSpriteData : ScriptableObject
    {
        [SerializeField] private Sprite[] rankSprites;
        [SerializeField] private Sprite[] suitSprites;

        public Sprite GetRankSprite(CardRank rank) => rankSprites[(int)rank];

        public Sprite GetSuitSprite(CardSuit suit) => suitSprites[(int)suit];
    }
}
