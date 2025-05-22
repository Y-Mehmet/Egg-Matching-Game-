public class HiddenProperty : IEggProperty
{
    public bool? Evaluate(EggColor expectedColor, EggColor actualColor)
    {
        if (expectedColor == actualColor)
            return true;
        return false;
    }
}
