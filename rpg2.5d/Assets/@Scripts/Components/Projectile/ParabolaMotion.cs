using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolaMotion : ProjectileMotion
{
    private float _heightArc = 2;

    protected override IEnumerator LaunchProjectile()
    {
        float startTime = Time.time;
        float journeyLength = Vector2.Distance(_startPos, _endPos);
        float totalTime = journeyLength / _speed;

        while (Time.time - startTime < totalTime)
        {
            float normalizedTime = (Time.time - startTime) / totalTime;

            // ������ ������� �̵�
            float x = Mathf.Lerp(_startPos.x, _endPos.x, normalizedTime);
            float z = Mathf.Lerp(_startPos.z, _endPos.z, normalizedTime);
            float baseY = Mathf.Lerp(_startPos.y, _endPos.y, normalizedTime);
            float arc = _heightArc * Mathf.Sin(normalizedTime * Mathf.PI);

            float y = baseY + arc;

            var nextPos = new Vector3(x, y, z);
            //if (IsRotation)
            //    transform.rotation = LookAt2D(nextPos - (Vector3)transform.position);
            transform.position = nextPos;

            yield return null;
        }

        transform.position = _endPos;
        EndCallback?.Invoke();
    }
}
