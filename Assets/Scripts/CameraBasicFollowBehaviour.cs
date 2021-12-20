using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBasicFollowBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _target;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(_target.transform.position.x, transform.position.y, transform.position.z);
    }
}
