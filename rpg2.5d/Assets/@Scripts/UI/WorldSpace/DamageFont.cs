using UnityEngine;
using TMPro;
using DG.Tweening;
using static Define;
using UnityEngine.Rendering;
using System.Collections;
using UnityEditorInternal;

public class DamageFont : BaseObject
{
    TMP_Text _damageText;
    Vector3 _startPos;
    Vector3 _endPos;

    float floatDistance = 3f;
    float duration = 3.0f;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        _damageText = Util.FindChild(gameObject, "Text").GetComponent<TextMeshPro>();


        return true;
    }

    public void SetInfo(Vector3 worldPos, float damage = 0, Transform parent = null, EDamageResult damageResult = EDamageResult.Hit)
    {
        float randomX = Random.Range(-0.5f, 0.5f);
        float randomY = Random.Range(-0.5f, 1.0f);

        transform.position = worldPos + new Vector3(randomX, randomY, 0);
        _startPos = transform.position;
        _endPos = _startPos + Vector3.up * floatDistance;

        switch (damageResult)
        {
            case EDamageResult.Hit:
                _damageText.SetText(damage.ToString("F0"));
                _damageText.color = Color.white;
                break;
            case EDamageResult.CriticalHit:
                _damageText.SetText(damage.ToString("F0"));
                _damageText.color = Util.HexToColor("FF8000");
                break;
            case EDamageResult.Miss:
                _damageText.SetText("Miss");
                _damageText.color = Color.red;
                break;
            case EDamageResult.Heal:
                _damageText.SetText(damage.ToString("F0"));
                _damageText.color = Util.HexToColor("3DA55A");
                break;
            case EDamageResult.CriticalHeal:
                _damageText.SetText(damage.ToString("F0"));
                _damageText.color = Util.HexToColor("3DA55A");
                break;
        }
        
        StartCoroutine(CoAscending());
        FadeOut();
    }


    IEnumerator CoAscending()
    {
        float start = Time.time;
        float total = 0;
        while (total <= duration)
        {
            total = Time.time - start;
            float progress = total / duration;
            
            transform.position = Vector3.Lerp(_startPos, _endPos, progress);

            yield return null;
        }
    }

    void FadeOut()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOScale(1.2f, duration / 2).SetEase(Ease.OutBack));
        //sequence.Append(transform.DOScale(1f, duration / 2).SetEase(Ease.InQuad));
        sequence.Join(_damageText.DOFade(0, duration).SetEase(Ease.InQuad));
        sequence.OnComplete(() => Destroy(gameObject));
    }
}
