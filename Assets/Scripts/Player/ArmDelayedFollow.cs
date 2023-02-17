using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmDelayedFollow : MonoBehaviour
{
    public Rotate rotTarget;

    public int rotDelay;

    private float timeCount = 0.0f;

    private float turnTime = 50f;

    void Update()
    {
        /*if (rotTarget.rotQueue.Count >= rotDelay) {
            Quaternion rot = rotTarget.rotQueue.Dequeue();
            transform.rotation = rot;
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, timeCount);
            timeCount = timeCount + Time.deltaTime;
        }*/

        Quaternion target = rotTarget.transform.rotation;


        float angle = Quaternion.Angle(target, transform.rotation);
        Debug.Log(angle);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnTime * Time.deltaTime * angle);
    }
}
