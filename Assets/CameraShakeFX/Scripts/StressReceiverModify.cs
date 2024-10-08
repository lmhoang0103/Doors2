using MEC;
using System.Collections.Generic;
using UnityEngine;
public class StressReceiverModify : MonoBehaviour
{
    private Vector3 lastPosition;
    private Vector3 lastRotation;
    [Tooltip("Maximum angle that the gameobject can shake. In euler angles.")]
    [SerializeField] private Vector3 MaximumAngularShake = Vector3.one * 5;
    [Tooltip("Maximum translation that the gameobject can receive when applying the shake effect.")]
    [SerializeField] private Vector3 MaximumTranslationShake = Vector3.one * .75f;

    /// <summary>
    ///  Applies a stress value to the current object.
    /// </summary>
    /// <param name="Stress">[0,1] Amount of stress to apply to the object</param>

    public IEnumerator<float> _ShakeCamera(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        Vector3 originalRot = transform.localEulerAngles;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            var previousRotation = lastRotation;
            var previousPosition = lastPosition;

            lastPosition = new Vector3(
                MaximumTranslationShake.x * (Mathf.PerlinNoise(0, Time.time * 25) * 2 - 1),
                MaximumTranslationShake.y * (Mathf.PerlinNoise(1, Time.time * 25) * 2 - 1),
                MaximumTranslationShake.z * (Mathf.PerlinNoise(2, Time.time * 25) * 2 - 1)
            ) * magnitude;

            lastRotation = new Vector3(
                MaximumAngularShake.x * (Mathf.PerlinNoise(3, Time.time * 25) * 2 - 1),
                MaximumAngularShake.y * (Mathf.PerlinNoise(4, Time.time * 25) * 2 - 1),
                MaximumAngularShake.z * (Mathf.PerlinNoise(5, Time.time * 25) * 2 - 1)
            ) * magnitude;

            transform.localPosition += lastPosition - previousPosition;
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + lastRotation - previousRotation);

            elapsed += Time.deltaTime;

            yield return Timing.WaitForOneFrame;
        }

        transform.localPosition = originalPos;
        transform.localEulerAngles = originalRot;

    }
}