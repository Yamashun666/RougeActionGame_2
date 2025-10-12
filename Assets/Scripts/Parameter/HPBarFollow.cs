using UnityEngine;
using UnityEngine.UI;
public class HPBarFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2f, 0);
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (target == null || cam == null) return;
        transform.position = cam.WorldToScreenPoint(target.position + offset);
    }
}
