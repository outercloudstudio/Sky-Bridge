using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Rigidbody>().isKinematic = !GetComponent<SkyBridge.NetworkedObject>().isOwner;
    }
}
