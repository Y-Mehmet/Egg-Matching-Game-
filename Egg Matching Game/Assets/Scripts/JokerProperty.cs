public class JokerProperty : IEggProperty
{
    public bool? Evaluate(EggColor expectedColor, EggColor actualColor)
    {
        return true; // her zaman doðru
    }
}
