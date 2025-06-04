using UnityEngine;

public class HiddenProperty : IEggProperty
{
    public bool? Evaluate(EggColor expectedColor, EggColor actualColor)
    {
        if (expectedColor == actualColor)
            return true;
        return false;
    }
   
    public void SetColor(GameObject egg, EggColor eggColor, bool isHiden = false)
    {
       
    }
}
