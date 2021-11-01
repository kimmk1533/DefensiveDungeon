using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Transform target;

    private void Start()
    {
        target = Camera.main.transform;
    }

    private void Update()
    {
        Vector3 targetPosition = new Vector3(transform.position.x, target.position.y, transform.position.z);
        transform.LookAt(targetPosition);
    }
}
