namespace Uno
{
    using System;

    internal class ActionCard : ColorCard, IActionable
    {
        const int value = 20;
        internal Action Action { get; private set; }

        public ActionCard(Action action , Color color, string name) : base(color, name, value)
        {
            Action = action;
        }

        public void TakeAction()
        {
            throw new NotImplementedException();
        }
    }
}
