using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class HurtFlashEffect : MonoBehaviour
{
    private int _flashCount = 3;
    private Color _flashColor = Color.red;
    private float _interval = 0.1f;
    private string _baseColorProperty = "_BaseColor";

    SpriteRenderer _renderer;
    
    public void Init()
    {
        _renderer = Util.FindChild(gameObject, "Mesh").GetComponent<SpriteRenderer>();

        
    }
    public void Flash()
    {
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(_interval);

        for (int i = 0; i < _flashCount; i++)
        {
            _renderer.color = _flashColor;
            yield return wait;

            _renderer.color = Color.white;
            yield return wait;
        }

    }
}
