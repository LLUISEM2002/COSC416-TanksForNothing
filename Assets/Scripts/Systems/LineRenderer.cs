using UnityEngine;
using UnityEngine.UI;

public class LineRenderer : MonoBehaviour
{
    private RectTransform m_myTransform;
    private Image m_image;
    private Camera m_camera;
    private RectTransform m_canvasRect;

    private void Awake()
    {
        m_canvasRect = GetComponent<RectTransform>();

        m_image = GetComponentInChildren<Image>();
        m_myTransform = m_image.GetComponent<RectTransform>();

        m_camera = Camera.main;
    }

    public void CreateLine(Vector3 worldPositionA, Vector3 worldPositionB, Color color)
    {
        m_image.color = color;

        Vector2 canvasPosA = WorldToCanvasPosition(worldPositionA);
        Vector2 canvasPosB = WorldToCanvasPosition(worldPositionB);

        Vector2 midpoint = (canvasPosA + canvasPosB) / 2f;
        m_myTransform.anchoredPosition = midpoint;

        Vector2 dir = canvasPosB - canvasPosA;
        m_myTransform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        m_myTransform.sizeDelta = new Vector2(dir.magnitude, m_myTransform.sizeDelta.y);
    }

    private Vector2 WorldToCanvasPosition(Vector3 worldPos)
    {
        Vector2 screenPoint = m_camera.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_canvasRect, screenPoint, m_camera, out Vector2 localPoint);
        return localPoint;
    }
}
