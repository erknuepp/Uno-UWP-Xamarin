namespace Uno
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class SkipCard : ActionCard, IActionable
    {
        public SkipCard(Action action, Color color, string name) : base(action, color, name)
        {
        }


    }
}
