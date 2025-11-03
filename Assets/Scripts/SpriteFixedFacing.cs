using UnityEngine;

public class IgnoreParentFlip : MonoBehaviour
{
    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void LateUpdate()
    {
        // 親のスケール反転を打ち消す
        Vector3 parentScale = transform.parent.localScale;
        transform.localScale = new Vector3(
            initialScale.x / parentScale.x,
            initialScale.y / parentScale.y,
            initialScale.z / parentScale.z
        );
    }
}