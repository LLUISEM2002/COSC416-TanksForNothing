using UnityEngine;
public class CanvasController : MonoBehaviour
{
    [SerializeField] private Transform tankTransform;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        Vector3 tankPos = tankTransform.position;
        Vector3 mousePos = InputManager.Instance.MouseWorldPosition;

        lineRenderer.CreateLine(tankPos, mousePos, Color.green);
    }
}
