namespace Uno
{
    internal class WildCard : Card, IActionable
    {
        const int value = 50;

        public WildCard(string name = "Wild"):base(name, value)
        {
        }

        public void TakeAction()
        {
            throw new System.NotImplementedException();
        }
    }
}