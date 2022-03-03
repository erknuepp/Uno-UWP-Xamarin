namespace Uno
{
    internal class SkipCard : ActionCard, IActionable
    {
        public SkipCard(Action action, Color color, string name) : base(action, color, name)
        {
        }
    }
}