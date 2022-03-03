namespace Uno
{
    using System;

    internal class DrawTwoCard : ActionCard, IDrawable, IActionable
    {
        public DrawTwoCard(Action action, Color color, string name) : base(action, color, name)
        {

        }

        public void TakeDraw()
        {
            throw new NotImplementedException();
        }
    }
}