using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warning : MonoBehaviour
{

    public Transform camTrans;
    public GameObject behind;

    private bool enemy;


    // Start is called before the first frame update
    void Start()
    {
        enemy = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (enemy) {
            behind.SetActive(true);
            enemy = false;
        } else if (!enemy) {
            behind.SetActive(false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Base" || other.tag == "Shielded") {
            //Enemy in range
            if (Vector3.Dot(camTrans.forward, other.transform.position - transform.position) < 0) {
                //Enemy behind
                enemy = true;
            }
        }
    }
}
