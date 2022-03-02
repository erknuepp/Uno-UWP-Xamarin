namespace Uno
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class ReverseCard : ActionCard, IActionable
    {
        public ReverseCard(Action action, Color color, string name) : base(action, color, name)
        {
        }
    }
}
