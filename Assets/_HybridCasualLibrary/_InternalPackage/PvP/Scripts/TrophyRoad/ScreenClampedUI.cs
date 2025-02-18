using UnityEngine;

public class ScreenClampedUI : MonoBehaviour
{
    [SerializeField] bool clampTop;
    [SerializeField] bool clampBottom;
    [SerializeField] bool clampLeft;
    [SerializeField] bool clampRight;

    Vector3 localPos;

    private void Awake()
    {
        localPos = transform.localPosition;
    }

    private void LateUpdate()
    {
        transform.localPosition = localPos;

        var top = clampTop ? Screen.height : Mathf.Infinity;
        var bottom = clampBottom ? 0f : Mathf.NegativeInfinity;
        var left = clampLeft ? 0f : Mathf.NegativeInfinity;
        var right = clampRight ? Screen.width : Mathf.Infinity;

        var newPos = transform.position;
        newPos.x = Mathf.Clamp(newPos.x, left, right);
        newPos.y = Mathf.Clamp(newPos.y, bottom, top);
        transform.position = newPos;
    }
}
