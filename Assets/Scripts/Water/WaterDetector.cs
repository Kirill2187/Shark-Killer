using UnityEngine;

public class WaterDetector : MonoBehaviour
{
    private float _lastSplash;
    private float _lastSplashSound;
    private Water _water;

    public float timeBetweenSplashes = 0.1f;
    public float timeBetweenSplashesSound = 0.3f;

    private void Awake()
    {
        _water = transform.parent.GetComponent<Water>();
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.isTrigger) return;
        if (hit.attachedRigidbody != null)
        {
            if (hit.attachedRigidbody.bodyType != RigidbodyType2D.Dynamic) return;
            _water.Splash(hit.transform.position.x, hit.attachedRigidbody.velocity * hit.attachedRigidbody.mass / 24f);
            if (Time.time > _lastSplashSound + timeBetweenSplashesSound &&
                hit.attachedRigidbody.mass >= 1 && hit.attachedRigidbody.velocity.y < -5f)
            {
                _lastSplashSound = Time.time;
                AudioManager.instance.Play("BigSplash");
            }
        }
    }

    private void OnTriggerStay2D(Collider2D hit)
    {
        if (hit.isTrigger) return;
        if (hit.attachedRigidbody != null && Time.time > _lastSplash + timeBetweenSplashes)
        {
            if (hit.attachedRigidbody.bodyType != RigidbodyType2D.Dynamic) return;
            _lastSplash = Time.time;
            _water.Splash(hit.transform.position.x, hit.attachedRigidbody.velocity * hit.attachedRigidbody.mass / 24f);
        }
    }
}