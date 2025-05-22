public class AntiJokerProperty : IEggProperty
{
    public bool? Evaluate(EggColor expectedColor, EggColor actualColor)
    {
        return false; // her zaman yanlýþ
    }
}
