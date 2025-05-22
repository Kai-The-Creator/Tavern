public class ChangedValue
{
    public ChangedValue(int value, int max = 0, bool infinite = false)
    {
        Value = value;
        Max = max;
        Infinite = infinite;
    }

    public int Value { get; private set; }
    public int Max {  get; private set; }
    public bool Infinite { get; private set; }

    public void Add(int value)
    {
        Value += value;

        if(!Infinite)
            if(Value > Max) Max = Value;
    }

    public void TryRemove(int value, out bool canRemove)
    {
        if(value > Value)
        {
            canRemove = false;
            return;
        }

        Value -= value;
        canRemove = true;
    }
}
