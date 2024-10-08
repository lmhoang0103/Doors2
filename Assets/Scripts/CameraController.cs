#region

using UnityEngine;

#endregion

public class CameraController : HCMonoBehaviour
{
    public static CameraController instance;

    private Vector3 _originOffset;
    public Vector3 offset;

    public Transform targetFollow;

    public float transitionSpeed;

    private void Awake()
    {
        instance = this;
        if (targetFollow != null)
            offset = Transform.position - targetFollow.transform.position;
        _originOffset = offset;
    }

    public void ResetOffset()
    {
        offset = _originOffset;
    }

    private void LateUpdate()
    {
        if (targetFollow == null)
            return;

        Transform.position = Vector3.Lerp(Transform.position,
            targetFollow.position + offset, Time.deltaTime * transitionSpeed);
    }
}