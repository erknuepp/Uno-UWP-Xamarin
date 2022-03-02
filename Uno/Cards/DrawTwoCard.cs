namespace Uno
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
