using UnityEngine;

public class JokerProperty : IEggProperty
{
    public bool? Evaluate(EggColor expectedColor, EggColor actualColor)
    {
        return true; // her zaman do�ru
    }
    public void SetColor(GameObject egg, EggColor eggColor, bool isHiden = false)
    {
        if (isHiden)
        {
            ColorManager.instance.SetMaterial(egg, eggColor);
        }
    }
}
