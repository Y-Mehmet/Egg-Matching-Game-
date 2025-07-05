using UnityEngine;

// Bu, tüm yetenek eylemlerinin miras alacaðý temel sýnýftýr.
// Bir ScriptableObject'tur, yani bir veri varlýðý olarak kaydedilebilir.
public abstract class AbilityAction : ScriptableObject
{
    // Tüm alt sýnýflar bu metodu zorunlu olarak kendi mantýklarýna göre doldurmalýdýr.
    public abstract void Execute();
}