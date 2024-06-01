using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private BaseObject _target;
    private bool isReady = false;

    public BaseObject Target
    {
        get { return _target; }
        set
        {
            _target = value;
            isReady = true;
        }
    }

    [SerializeField] public float smoothSpeed = 6f; // 스무딩 속도

    private int _targetOrthographicSize = 18;
    private Quaternion _angle = Quaternion.Euler(15, 0, 0);

    private void LateUpdate()
    {
        // Smoothly transition to the target camera size
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, _targetOrthographicSize, smoothSpeed * Time.deltaTime);

        HandleCameraPosition();
    }

    private void HandleCameraPosition()
    {
        if (isReady == false)
            return;

        Vector3 targetPosition = new Vector3(Target.Position.x, Target.Position.y + 4.5f, Target.Position.z - 8.0f);
        targetPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.fixedDeltaTime);

        transform.SetPositionAndRotation(targetPosition, _angle);
    }

    //public void TargetingCamera(InteractionObject dest)
    //{
    //    //이미 진행중이면 리턴
    //    if (State == ECameraState.Targeting)
    //        return;

    //    State = ECameraState.Targeting;
    //    Vector3 targetPosition = new Vector3(Target.CenterPosition.x, Target.CenterPosition.y, -10f);
    //    Vector3 destPosition = new Vector3(dest.Position.x, dest.Position.y, -10f);

    //    Sequence seq = DOTween.Sequence();
    //    seq.Append(transform.DOMove(destPosition, 0.8f).SetEase(Ease.Linear))
    //        .AppendInterval(2f)
    //        .Append(transform.DOMove(targetPosition, 0.8f).SetEase(Ease.Linear))
    //        .OnComplete(() => { State = ECameraState.Following; });
    //}
}
