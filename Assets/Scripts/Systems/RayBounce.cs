using UnityEngine;
using System.Collections.Generic;

public class RayBounce : MonoBehaviour
{
    [SerializeField] private float maxDistance = 30f;
    [SerializeField] private int rayCount = 2;
    private LayerMask collisionMask = 0;
    private Transform Mantle;
    private Transform Close;
    private Transform Middle;
    private Transform Far;
    private void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            Tank playerTank = playerObject.GetComponent<Tank>();
            if (playerTank != null)
            {
                Mantle = playerTank.Mantle;
                if (Mantle == null) 
                {
                    Debug.LogWarning("Mantle Transformation get unsuccessful.");
                }
            }
            else
            {
                Debug.LogWarning("Tank component not found on Player object.");
            }
        }
        else
        {
            Debug.LogWarning("Player tank not found! Make sure it has the 'Player' tag.");
        }
        Close = transform.Find("Close");
        Middle = transform.Find("Middle");
        Far = transform.Find("Far");

        if (Close == null || Middle == null || Far == null)
        {
            Debug.LogWarning($"Image position(s) not found on {gameObject.name}. Make sure it has all three images named 'Close', 'Middle', and 'Far'.");
        }

        collisionMask = LayerMask.GetMask("Wall");

        if (collisionMask == 0)
        {
            Debug.LogWarning("Wall mask not found.");
        }
    }
    private void Update()
    {
        if (InputManager.Instance.ShowRay)
        {
            if (Mantle != null && Close != null && Middle != null && Far != null)
            {
                Vector3 start = Mantle.position;
                start.z = 1;
                CastRay(start, Mantle.forward);
            }
        }
        else
        {
            Vector3 offscreen = new Vector3(100, 0, 100);
            Close.SetPositionAndRotation(offscreen, Quaternion.identity);
            Middle.SetPositionAndRotation(offscreen, Quaternion.identity);
            Far.SetPositionAndRotation(offscreen, Quaternion.identity);
        }
    }

    private void CastRay(Vector3 position, Vector3 direction)
    {


        List<Vector3> points = new() { position };

        for (int i = 0; i < rayCount; i++)
        {
            Ray ray = new Ray(position, direction);
            if (Physics.Raycast(ray, out RaycastHit hit, (rayCount-i)*maxDistance, collisionMask))
            {
                points.Add(hit.point);
                direction = Vector3.Reflect(direction, hit.normal);
                position = hit.point;
            }
            else
            {
                Vector3 endPoint = position + direction.normalized * maxDistance;
                points.Add(endPoint);
                break;
            }
        }
        float totalDistance = 0f;
        List<float> segmentLengths = new();

        for (int i = 0; i < points.Count - 1; i++)
        {
            float segment = Vector3.Distance(points[i], points[i + 1]);
            segmentLengths.Add(segment);
            totalDistance += segment;
        }

        // Helper to get a point along the path
        Vector3 GetPointAlongPath(float targetDistance)
        {
            float accumulated = 0f;
            for (int i = 0; i < segmentLengths.Count; i++)
            {
                if (accumulated + segmentLengths[i] >= targetDistance)
                {
                    float remaining = targetDistance - accumulated;
                    Vector3 dir = (points[i + 1] - points[i]).normalized;
                    return points[i] + dir * remaining;
                }
                accumulated += segmentLengths[i];
            }
            return points[^1]; // Fallback
        }

        // Set object positions 
        Vector3 closePos = GetPointAlongPath(totalDistance * 0.15f);
        Close.position = closePos;

        Vector3 middlePos = GetPointAlongPath(totalDistance * 0.5f);
        Middle.position = middlePos;

        Vector3 farPos = GetPointAlongPath(totalDistance * 0.95f);
        Far.position = farPos;
    }
}
