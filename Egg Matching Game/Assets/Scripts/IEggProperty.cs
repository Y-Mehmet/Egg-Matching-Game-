public interface IEggProperty
{
    /// <summary>
    /// Eðer null dönerse normal renk kontrolüne geçilir.
    /// true: her zaman doðru, false: her zaman yanlýþ
    /// </summary>
    bool? Evaluate(EggColor expectedColor, EggColor actualColor);
}
