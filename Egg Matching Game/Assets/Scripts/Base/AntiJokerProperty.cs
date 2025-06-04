using UnityEngine;

public class AntiJokerProperty : IEggProperty
{
    public bool? Evaluate(EggColor expectedColor, EggColor actualColor)
    {
        return false; // her zaman yanlýþ
    }
    public void SetColor(GameObject egg, EggColor eggColor, bool isHiden = false)
    {
       if( isHiden)
        {
            ColorManager.instance.SetMaterial(egg, eggColor);
        }
    }
}
