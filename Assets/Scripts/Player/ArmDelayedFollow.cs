using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmDelayedFollow : MonoBehaviour
{
    public Rotate rotTarget;

    public int rotDelay;

    void Update()
    {
        if (rotTarget.rotQueue.Count >= rotDelay) {
            Quaternion rot = rotTarget.rotQueue.Dequeue();
            transform.rotation = rot;
        }
    }
}
