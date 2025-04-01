using UnityEngine;

public class Bullet : MonoBehaviour

{
    private MapController mapController;
    private int maxBounces = 3;
    private int currentBounces = 0;

    private Rigidbody rb;

    private bool hasCollided = false;

    public void Initialize(float speed, Vector3 direction, float lifetime, int maxBounces)
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = direction.normalized * speed;

        this.maxBounces = maxBounces;
        Destroy(gameObject, lifetime);
    }

    public void Start()
    {
        // if (AudioManager.instance != null)
        // {
        //     AudioManager.instance.PlaySound(AudioManager.instance.shootClip);
        // }
         mapController = FindObjectOfType<MapController>();
    }
    public void OnCollisionEnter(Collision collision)
    {
        // if (AudioManager.instance != null)
        // {
        //     AudioManager.instance.PlaySound(AudioManager.instance.explosionClip);
        // }
    
        // Instantiate(explosionParticle, transform.position, transform.rotation);
        Debug.Log(collision.gameObject);

        if (hasCollided) return;
        hasCollided = true;

        if (collision.gameObject.CompareTag("Tank"))
        {
                Debug.Log("Collided with TANK: " + collision.gameObject.name + ", bullet: " + gameObject.name);
            if (mapController != null)
            {
                mapController.OnJamokeDone();
            }
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            Destroy(collision.gameObject);
            Debug.Log("Player hit by bullet! GAME OVER!");

            if (mapController != null)
            {
                mapController.OnPlayerDestroyed();
            }
        }
        else
        {
            currentBounces++;

            if (currentBounces > maxBounces)
            {
                Destroy(gameObject);
            }else {
                hasCollided = false;
            }
        }
    }

    // handling bullet shooting

    public static void FireBullet(GameObject bulletPrefab, Vector3 spawnPosition, Vector3 shootDirection, float speed, float lifetime, int bulletMaxBounce)
    {
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.Initialize(speed, shootDirection, lifetime, bulletMaxBounce);
        }
    }
}