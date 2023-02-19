using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adapted from: https://roystan.net/articles/camera-shake.html and https://www.youtube.com/watch?v=tu-Qe66AvtY mostly by Scott King, adapted by Will Ozeas
public class CameraShake : MonoBehaviour
{
    // How trauma relates to camera shake (quadratically by default). This makes shake taper off more realistically.
    [SerializeField] float trauma_exponent = 2;
    // How quickly truama decays each second (linearly)
    [SerializeField] float trauma_decay = 0.8f;

    // Maximum transitional shake
    [SerializeField] float max_transitional_shake = 1.3f;
    // Maximum rotational shake
    [SerializeField] float max_rotational_shake = 2.5f;

    // Controls the speed of the shake
    [SerializeField] float frequency = 25;

    private float shake;
    private float seed;

    private bool shaking = false;

    public static CameraShake i;

    public Camera cam;
    private float trauma;
    private float z;

    private float shakeMultiplier = 1f;

    private void Awake()
    {
        // Seed for noise generator
        seed = Random.value;
        i = this;
    }
    private void Start() {
        cam = GetComponent<Camera>();
        trauma = 0;
        z = transform.position.z;
    }


    // Update is called once per frame
    void Update()
    {
        // Determine necassary amount of shake
        shake = Mathf.Pow(trauma, trauma_exponent);

        // Translational shake centered on parent's (player's) position
        // The staggered seeds make it move independently in each direction (z stays the same)
        transform.localPosition = new Vector3(calculateShake(seed)* shake * max_transitional_shake, calculateShake(seed + 1) * shake * max_transitional_shake, z) ;

        // Rotational shake only rotates on z axis
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, calculateShake(seed + 2)) * shake * max_rotational_shake);

        // trauma decay scaled by time so as not to be frame rate dependent and clamped betweeen 0 and 1
        if(!shaking) {
            trauma = Mathf.Clamp01(trauma - (trauma_decay * Time.deltaTime));
        }
    }


    private float calculateShake(float seed)
    {
        // Perlin noise makes the shake smooth, but it returns in range [0, 1], so it's scaled to the range [-1, 1]
        return (Mathf.PerlinNoise(seed, Time.time * frequency) * 2) - 1;
    }

    public void ShakeRef(ref int intensity) {
        Shake(intensity);
    }

    public void Shake(int intensity) {
        trauma = shakeMultiplier * intensity/5f;
    }

    public void StartShaking(float traumaIn) {
        shaking = true;
        trauma = traumaIn;
    }

    public void StopShaking() {
        shaking = false;
    }

    int originalCullingMask;
    public void BlackScreen() {
        originalCullingMask = cam.cullingMask;
        cam.cullingMask = (1 << LayerMask.NameToLayer("Nothing"));
    }

    public void UnBlackScreen() {
        cam.cullingMask = originalCullingMask;
    }

    public void UpdateShake(float value) {
        shakeMultiplier = value;
    }
}
