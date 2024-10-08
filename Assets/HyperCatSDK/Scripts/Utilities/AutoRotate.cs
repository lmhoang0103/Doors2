#region

using UnityEngine;

#endregion

public class AutoRotate : HCMonoBehaviour
{
    private float _newValue;

    public RotateAxis axis = RotateAxis.Z;
    public float speed;

    private void Update()
    {
        _newValue += Time.deltaTime * speed;

        switch (axis)
        {
            case RotateAxis.X:
                Transform.localEulerAngles = new Vector3(_newValue, Transform.localEulerAngles.y, Transform.localEulerAngles.z);
                break;
            case RotateAxis.Y:
                Transform.localEulerAngles = new Vector3(Transform.localEulerAngles.x, _newValue, Transform.localEulerAngles.z);
                break;
            case RotateAxis.Z:
                Transform.localEulerAngles = new Vector3(Transform.localEulerAngles.x, Transform.localEulerAngles.y, _newValue);
                break;
        }
    }
}

public enum RotateAxis
{
    X,
    Y,
    Z
}