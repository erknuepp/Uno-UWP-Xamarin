namespace Uno
{

    internal abstract class Card
    {
        public Card(string name, int value)
        {
            Name = name;
            Value = value;
        }

        internal string Name { get; private set; }
        internal int Value { get; private set; }
    }
}
