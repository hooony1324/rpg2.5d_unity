using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class HurtFlashEffect : MonoBehaviour
{
    private int _flashCount = 1;
    private Color _flashColor = Color.red;
    private float _interval = 0.1f;
    private string _baseColorProperty = "BaseColor";

    MaterialPropertyBlock _mpb;
    Renderer _renderer;
    
    public void Init()
    {
        if (_mpb == null)
            _mpb = new MaterialPropertyBlock();

        _renderer = Util.FindChild(gameObject, "Mesh").GetComponent<Renderer>();       

        _renderer.SetPropertyBlock(_mpb);
    }
    public void Flash()
    {
        _renderer.GetPropertyBlock(_mpb);
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        int baseColor = Shader.PropertyToID(_baseColorProperty);

        WaitForSeconds wait = new WaitForSeconds(_interval);

        for (int i = 0; i < _flashCount; i++)
        {
            _mpb.SetColor(baseColor, _flashColor);
            _renderer.SetPropertyBlock(_mpb);
            yield return wait;

            _renderer.SetPropertyBlock(_mpb);
            yield return wait;
        }

        yield return null;
    }
}
