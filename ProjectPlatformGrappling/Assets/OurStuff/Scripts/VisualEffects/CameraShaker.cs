using UnityEngine;
using System.Collections;

public class CameraShaker : BaseClass {
    private Vector3 originalPos;
    private bool smoothShake = false;
    private bool isShaking = false;

    private bool overrideShake; //sätts som argument
    // Use this for initialization
    void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        originalPos = transform.localPosition;

        Reset();
        //StartCoroutine(Shake(2, 2));
    }

    public override void Reset()
    {
        base.Reset();
        isShaking = false;
        smoothShake = false;
        transform.localPosition = originalPos;
    }

    public void ShakeCamera(float duration, float magnitude, bool smooth)
    {
        smoothShake = smooth;
        overrideShake = false;
        transform.localPosition = originalPos;
        StopAllCoroutines();
        StartCoroutine(Shake(duration, magnitude));
    }

    public void ShakeCamera(float duration, float magnitude, bool smooth, bool overrideCurrShake)
    {
        smoothShake = smooth;
        transform.localPosition = originalPos;
        overrideShake = overrideCurrShake;
        StopAllCoroutines();
        StartCoroutine(Shake(duration, magnitude));
    }

    IEnumerator Shake(float duration, float magnitude)
    {
        if (isShaking)
        {
            if (!overrideShake)
            {
                yield break;
            }
        }

        float elapsed = 0.0f;

        while (elapsed < duration)
        {

            elapsed += Time.deltaTime;

            float percentComplete = elapsed / duration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            // map value to [-1, 1]
            float x, y;

            if (smoothShake)
            {
                x = Mathf.PerlinNoise(Random.value, Random.value) * 2.0f - 1.0f;
                y = Mathf.PerlinNoise(Random.value, Random.value) * 2.0f - 1.0f;
            }
            else
            {
                x = Random.value * 2.0f - 1.0f;
                y = Random.value * 2.0f - 1.0f;
            }

            x *= magnitude * damper;
            y *= magnitude * damper;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            yield return null;
        }

        transform.localPosition = originalPos;
        overrideShake = false;
    }

}
