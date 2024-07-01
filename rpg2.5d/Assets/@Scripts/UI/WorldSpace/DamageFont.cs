using UnityEngine;
using TMPro;
using DG.Tweening;
using static Define;
using UnityEngine.Rendering;
using System.Collections;
using UnityEditorInternal;

public class DamageFont : UI_Base
{
    enum Texts
    {
        Text,
    }

    TMP_Text _damageText;
    Camera _camera;
    RectTransform _rect;
    Vector3 _worldPos;
    float floatDistance = 1.5f;
    float duration = 30.0f;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));
        _damageText = GetText((int)Texts.Text);
        
        _camera = Camera.main;
        _rect = _damageText.rectTransform;
        
        return true;
    }

    public void SetInfo(Vector3 worldPos, float damage = 0, Transform parent = null, EDamageResult damageResult = EDamageResult.Hit)
    {
        float randomX = Random.Range(-0.5f, 0.5f);
        float randomY = Random.Range(-0.5f, 1.0f);
        _worldPos = worldPos + new Vector3(randomX, randomY, 0);

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
            Vector3 offset = Vector3.up * 50 * progress;

            Vector3 position = _camera.WorldToScreenPoint(_worldPos + offset);

            _rect.position = position;
            //transform.position = position;

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
