namespace Uno
{
    using System;


    internal class WildDrawFourCard : WildCard, IDrawable
    {
        const int value = 50;

        public WildDrawFourCard(string name = "Wild Draw Four") : base(name)
        {
        }

        public void TakeDraw()
        {
            throw new NotImplementedException();
        }
    }
}
