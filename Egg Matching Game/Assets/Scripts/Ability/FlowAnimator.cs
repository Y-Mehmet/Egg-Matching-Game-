using UnityEngine;
using DG.Tweening;

public class FlowAnimator : MonoBehaviour
{
    Vector3 startRotation = new Vector3(180, 0, 30), startPos = new Vector3(0, -3, -1f);
   
    private void OnEnable()
    {
        transform.position = startPos; // Baþlangýç pozisyonunu ayarla
        StartFlowAnimation();
    }

    private void StartFlowAnimation()
    {
        SoundManager.instance.PlaySfx(SoundType.FreezeTime,startTime:3);
        // Önceki tween'leri temizle
        transform.DOKill();

        // Baþlangýç rotasyonu sýfýrla
      transform.localEulerAngles = startRotation;
        transform.DOMove(startPos + new Vector3(0, 2, 0f), 1.5f).SetEase(Ease.InOutSine).onComplete = () =>
        {
            transform
    .DORotate(new Vector3(0, 0, -60f), .5f, RotateMode.LocalAxisAdd)
    .SetLoops(-1, LoopType.Yoyo)
    .SetEase(Ease.InOutSine);
            // Yumuþak geçiþ

            // 5 saniye sonra durdur ve objeyi kapat
            DOVirtual.DelayedCall(5.5f, () =>
            {
                transform.DOKill();
                transform.localEulerAngles = startRotation;  // Baþlangýca dön
                gameObject.SetActive(false);
            });
        };
        // DOTween animasyonu
        
    }
}
