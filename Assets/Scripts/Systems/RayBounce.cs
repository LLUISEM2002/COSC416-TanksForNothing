using UnityEngine;

public class RayBounce : MonoBehaviour
{
    [SerializeField] private int rayCount = 2;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private LayerMask collisionMask = ~0; // Everything by default

    private void Update()
    {
        CastRay(transform.position, transform.forward);
    }

    private void CastRay(Vector3 position, Vector3 direction)
    {
        for (int i = 0; i < rayCount; i++)
        {
            Ray ray = new Ray(position, direction);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, collisionMask))
            {
                Debug.DrawLine(position, hit.point, Color.red);
                position = hit.point;

                // Reflect the direction off the surface
                direction = Vector3.Reflect(direction, hit.normal);
            }
            else
            {
                Debug.DrawRay(position, direction * maxDistance, Color.blue);
                break;
            }
        }
    }
}
