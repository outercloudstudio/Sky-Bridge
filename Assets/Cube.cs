using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkyBridge;

public class Cube : MonoBehaviour
{
    NetworkedObject networkedObject;

    private void Start()
    {
        networkedObject = GetComponent<NetworkedObject>();

        GetComponent<Rigidbody>().isKinematic = !networkedObject.isOwner;
    }

    private void Update()
    {
        if(transform.position.y < 0 && networkedObject.isOwner) Destroy(gameObject);
    }
}
