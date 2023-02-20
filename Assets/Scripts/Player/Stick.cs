using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : MonoBehaviour
{
    public bool sticking;
    public Vector3 normal;

    public GameObject doubleJumpText;

    bool stickable;

    // Start is called before the first frame update
    void Start()
    {
        sticking = false;
        stickable = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pillar" && transform.position.y > -3.8f && stickable) {
            sticking = true;
            doubleJumpText.SetActive(true);
            StartCoroutine(Unstick());
            stickable = false;
        }

        if (other.tag == "Floor") {
            stickable = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Pillar") {
            sticking = false;
            doubleJumpText.SetActive(false);
        }
    }

    IEnumerator Unstick()
    {
        yield return new WaitForSeconds(3f);
        sticking = false;
    }
}
