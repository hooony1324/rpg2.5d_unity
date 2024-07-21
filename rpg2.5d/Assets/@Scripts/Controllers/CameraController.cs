using Castle.DynamicProxy.Contributors;
using Cinemachine;
using DG.Tweening;
using NSubstitute.Routing.Handlers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class CameraController : InitBase
{
    private Camera _mainCamera;
    private CinemachineVirtualCamera _virtualCamera;
    private Transform _target;

    // Fading Near Camera
    [SerializeField]
    private LayerMask _fadingMask;
    private float _fadedAlpha = 0.3f;

    [SerializeField]
    private Vector3 TargetPositionOffset = Vector3.up;
    private float _fadeSpeed = 3.0f;

    private List<FadingObject> _fadingObjects = new List<FadingObject>();
    private Dictionary<FadingObject, Coroutine> _fadingCoroutines = new Dictionary<FadingObject, Coroutine>();
    private RaycastHit[] Hits = new RaycastHit[10];

    public Transform Target
    {
        get => _target;
        private set => _target = value;
    }

    public void Init()
    {
        _mainCamera = Camera.main;
        _virtualCamera = gameObject.GetComponent<CinemachineVirtualCamera>();

        _fadingMask = LayerMask.GetMask("Wall");
    }

    public void SetTarget(Transform target)
    {
        _virtualCamera.Follow = target;
        _virtualCamera.LookAt = target;
        Target = target;

        StartCoroutine(CheckFadingObjects());
    }

    private IEnumerator CheckFadingObjects()
    {
        while (true)
        {
            int hits = Physics.RaycastNonAlloc(transform.position, (Target.transform.position + TargetPositionOffset - transform.position), Hits, Vector3.Distance(transform.position, Target.transform.position), _fadingMask);

            Debug.DrawRay(transform.position, (Target.transform.position + TargetPositionOffset - transform.position));

            if (hits > 0)
            {
                for (int i = 0; i < hits; i++)
                {
                    FadingObject fadingObject = GetFadingObejct(Hits[i]);

                    if (fadingObject != null && !_fadingObjects.Contains(fadingObject))
                    {
                        if (_fadingCoroutines.ContainsKey(fadingObject))
                        {
                            if (_fadingCoroutines[fadingObject] != null)
                            {
                                StopCoroutine(_fadingCoroutines[fadingObject]);
                            }

                            _fadingCoroutines.Remove(fadingObject);
                        }

                        _fadingCoroutines.Add(fadingObject, StartCoroutine(FadeObjectOut(fadingObject)));
                        _fadingObjects.Add(fadingObject);
                    }
                }
            }

            FadeObjectsNoLongerBeingHit();
            ClearHits();

            yield return null;
        }
    }

    private FadingObject GetFadingObejct(RaycastHit hit)
    {
        return hit.collider?.GetComponent<FadingObject>() ?? null;
    }

    private IEnumerator FadeObjectOut(FadingObject fadingObject)
    {
        fadingObject.EnableMaterials();

        float time = 0;

        while (fadingObject.Alpha > _fadedAlpha)
        {
            float alpha = Mathf.Lerp(fadingObject.InitialAlpha, _fadedAlpha, time * _fadeSpeed);
            fadingObject.SetAlpha(alpha);

            time += Time.deltaTime;
            yield return null;
        }

        if (_fadingCoroutines.ContainsKey(fadingObject))
        {
            StopCoroutine(_fadingCoroutines[fadingObject]);
            _fadingCoroutines.Remove(fadingObject);
        }
    }

    private IEnumerator FadeObjectIn(FadingObject fadingObject)
    {
        float time = 0;

        while (fadingObject.Alpha < fadingObject.InitialAlpha)
        {
            float alpha = Mathf.Lerp(_fadedAlpha, fadingObject.InitialAlpha, time * _fadeSpeed);
            fadingObject.SetAlpha(alpha);

            time += Time.deltaTime;
            yield return null;
        }

        fadingObject.DisableMaterials();

        if (_fadingCoroutines.ContainsKey(fadingObject))
        {
            StopCoroutine(_fadingCoroutines[fadingObject]);
            _fadingCoroutines.Remove(fadingObject);
        }
    }

    private void FadeObjectsNoLongerBeingHit()
    {
        List<FadingObject> objectsToRemove = new List<FadingObject>(_fadingObjects.Count);

        foreach (FadingObject fadingObject in _fadingObjects)
        {
            bool objectIsBeingHit = false;
            for (int i = 0; i < Hits.Length; i++)
            {
                FadingObject hitFadeingObject = GetFadingObejct(Hits[i]);
                if (hitFadeingObject != null && fadingObject == hitFadeingObject)
                {
                    objectIsBeingHit = true;
                    break;
                }
            }


            if (!objectIsBeingHit)
            {
                if (_fadingCoroutines.ContainsKey(fadingObject))
                {
                    if (_fadingCoroutines[fadingObject] != null)
                    {
                        StopCoroutine(_fadingCoroutines[fadingObject]);
                    }
                    _fadingCoroutines.Remove(fadingObject);
                }

                _fadingCoroutines.Add(fadingObject, StartCoroutine(FadeObjectIn(fadingObject)));
                objectsToRemove.Add(fadingObject);
            }
        }

        foreach (FadingObject removeObject in  objectsToRemove)
        {
            _fadingObjects.Remove(removeObject);
        }

    }

    private void ClearHits()
    {
        System.Array.Clear(Hits, 0, Hits.Length);
    }

}
