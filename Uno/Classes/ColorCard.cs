namespace Uno
{

    internal class ColorCard : Card
    {
        internal Color Color { get; private set; }

        public ColorCard(Color color, string name, int value) : base(name, value)
        {
            Color = color;
        }
    }
}
