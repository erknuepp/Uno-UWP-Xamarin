namespace Uno
{

    internal class ReverseCard : ActionCard, IActionable
    {
        public ReverseCard(Action action, Color color, string name) : base(action, color, name)
        {
        }
    }
}