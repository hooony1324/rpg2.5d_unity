using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Test_PlayerController : MonoBehaviour
{
    private void Update()
    {
        Vector3 moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            moveDir += Vector3.forward;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveDir += Vector3.right;
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            moveDir -= Vector3.forward;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveDir -= Vector3.right;
        }

        transform.Translate(moveDir * Time.deltaTime * 4);
    }
}
