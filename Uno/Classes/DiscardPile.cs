namespace Uno
{
    using System.Collections.Generic;

    internal class DiscardPile
    {
        Stack<Card> cards;

        public DiscardPile()
        {
            cards = new Stack<Card>();
        }

        public void AddCard(Card card)
        {
            cards.Push(card);
        }
        
        public Card LastCardPlayed()
        {
            return cards.Peek();
        }
    }
}
