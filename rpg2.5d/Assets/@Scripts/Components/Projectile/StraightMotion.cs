using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightMotion : ProjectileMotion
{
    protected override IEnumerator LaunchProjectile()
    {
        while (true)
        {
            Vector3 direction = (_endPos - Position).normalized;
            transform.rotation = LookAt2D(direction);
            transform.position += direction * (_speed * Time.deltaTime);

            if (Vector2.Distance(Position, _endPos) < 0.2f)
            {
                EndCallback?.Invoke();
                yield break;
            }
            yield return null;
        }
    }
}
