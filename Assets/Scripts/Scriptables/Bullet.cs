using UnityEngine;

public class Bullet : MonoBehaviour

{
      private MapController mapController;


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

        if (collision.gameObject.CompareTag("Tank"))
        {
            if (mapController != null)
            {
                mapController.OnJamokeDone();
            }
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    // handling bullet shooting

    public static void FireBullet(GameObject bulletPrefab, Vector3 spawnPosition, Vector3 shootDirection, float speed, float lifetime)
    {
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

        bulletRigidbody.linearVelocity = shootDirection.normalized * speed;

        Destroy(bullet, lifetime);
    }
}