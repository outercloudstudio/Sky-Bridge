using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public float timer = 0;
    public float delay = 1;

    public GameObject cube;

    void Update()
    {
        if (SkyBridge.SkyBridge.isHost)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                Instantiate(cube, new Vector3(0, 15, 0), Quaternion.identity);

                timer = delay;
            }
        }
    }
}
