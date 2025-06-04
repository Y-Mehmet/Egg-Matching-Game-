using UnityEngine;

public interface IEggProperty
{
    /// <summary>
    /// E�er null d�nerse normal renk kontrol�ne ge�ilir.
    /// true: her zaman do�ru, false: her zaman yanl��
    /// </summary>
    bool? Evaluate(EggColor expectedColor, EggColor actualColor);
    void SetColor(GameObject egg, EggColor eggColor, bool isHiden=false);
}
