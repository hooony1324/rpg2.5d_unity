using UnityEngine;
using TMPro;
using DG.Tweening;
using static Define;

public class DamageFont : BaseObject
{
    private TextMeshPro _damageText;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        GetComponent<MeshRenderer>().sortingOrder = SortingLayers.WROLD_FONT;

        return true;
    }

    public void SetInfo(Vector3 pos, float damage = 0, Transform parent = null, EDamageResult damageResult = EDamageResult.Hit)
    {
        _damageText = GetComponent<TextMeshPro>();
        _damageText.sortingOrder = SortingLayers.PROJECTILE;

        transform.position = pos;

        switch (damageResult)
        {
            case EDamageResult.Hit:
                NormalAscending();
                break;
            case EDamageResult.CriticalHit:
                break;
            case EDamageResult.Miss:
                break;
            case EDamageResult.Heal:
                break;
            case EDamageResult.CriticalHeal:
                break;

        }
    }

    float floatDistance = 1.5f;
    float duration = 1.0f;
    void NormalAscending()
    {
        Sequence seq = DOTween.Sequence();

        float randomX = Random.Range(-0.2f, 0.2f);
        float randomY = Random.Range(0, 0.4f);
        transform.position += new Vector3(randomX, randomY, 0);
        
        _damageText.alpha = 1.0f;

        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = initialPosition + Vector3.up * floatDistance;
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(targetPosition, duration).SetEase(Ease.OutQuad));
        sequence.Join(transform.DOScale(1.2f, duration / 2).SetEase(Ease.OutBack));
        sequence.Append(transform.DOScale(1f, duration / 2).SetEase(Ease.InQuad));
        sequence.Join(_damageText.DOFade(0, duration).SetEase(Ease.InQuad));
        sequence.OnComplete(() => Destroy(gameObject));
    }
}
