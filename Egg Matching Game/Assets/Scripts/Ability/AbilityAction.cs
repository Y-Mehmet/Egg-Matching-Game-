using UnityEngine;

// Bu, t�m yetenek eylemlerinin miras alaca�� temel s�n�ft�r.
// Bir ScriptableObject'tur, yani bir veri varl��� olarak kaydedilebilir.
public abstract class AbilityAction : ScriptableObject
{
    // T�m alt s�n�flar bu metodu zorunlu olarak kendi mant�klar�na g�re doldurmal�d�r.
    public abstract void Execute();
}