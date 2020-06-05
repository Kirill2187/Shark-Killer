using UnityEngine;

public class CollisionWithPlayer : MonoBehaviour
{
    private bool _isUnderEffect;
    private bool _isUnderKillerEffect;
    private GameManager _manager;

    private SpriteRenderer _playerRenderer;

    private void Awake()
    {
        _manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _playerRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("fish") &&
            (collision.otherCollider.name.Equals("mouth") || _isUnderKillerEffect))
        {
            collision.gameObject.GetComponent<Fish>().killedByPlayer = true;
            collision.gameObject.GetComponent<Fish>().Destroy();
            _manager.EatFish();
            AudioManager.instance.Play("Eat");
        }

        if (collision.gameObject.CompareTag("bird") &&
            (collision.otherCollider.name.Equals("mouth") || _isUnderKillerEffect))
        {
            collision.gameObject.GetComponent<BirdMovement>().Destroy();
            _manager.EatBird();
            AudioManager.instance.Play("Eat");
        }

        if (collision.gameObject.transform.root.gameObject.CompareTag("mine"))
        {
            var mine = collision.gameObject.transform.root.gameObject.GetComponent<Mine>();
            if (mine.isDestroyed)
            {
                return;
            }

            mine.Explode();
            _manager.BombDamage();
            AudioManager.instance.Play("Explosion");
        }

        if (collision.gameObject.CompareTag("chest"))
        {
            var chest = collision.gameObject.GetComponent<Chest>();
            var effect = chest.ownEffect;
            if (chest.isOpen)
            {
                return;
            }

            AudioManager.instance.Play("ChestOpen");

            if (effect == Chest.Effects.Invisible)
            {
                if (_isUnderEffect)
                {
                    _manager.AddScores(chest.scoreCount);
                    chest.Open();
                    return;
                }

                Boids.IsPlayerInvisible = true;
                _isUnderEffect = true;
                _playerRenderer.color = new Color(1, 1, 1, 0.4f);

                Invoke("MakePlayerVisible", Random.Range(chest.effectTimeBounds.x, chest.effectTimeBounds.y));
            }

            if (effect == Chest.Effects.AccelerationBoost)
            {
                if (_isUnderEffect)
                {
                    _manager.AddScores(chest.scoreCount);
                    chest.Open();
                    return;
                }

                GetComponent<PlayerMovement>().acceleration = 1000f;
                GetComponent<PlayerMovement>().turnSpeed = 1000f;
                _isUnderEffect = true;
                _playerRenderer.color = new Color(0.7f, 0.7f, 1f, 1);
                Invoke("ReturnAcceleration", Random.Range(chest.effectTimeBounds.x, chest.effectTimeBounds.y));
            }

            if (chest.ownEffect == Chest.Effects.HungerFill)
            {
                _manager.FillHunger();
            }

            if (chest.ownEffect == Chest.Effects.Scores)
            {
                _manager.AddScores(chest.scoreCount);
            }

            if (chest.ownEffect == Chest.Effects.Killer)
            {
                if (_isUnderEffect)
                {
                    _manager.AddScores(chest.scoreCount);
                    chest.Open();
                    return;
                }

                _isUnderEffect = true;
                _isUnderKillerEffect = true;
                _playerRenderer.color = new Color(1f, 0.7f, 0.7f, 1);
                Invoke("RemoveKillerEffect", Random.Range(chest.effectTimeBounds.x, chest.effectTimeBounds.y));
            }

            chest.Open();
        }
    }

    private void MakePlayerVisible()
    {
        _isUnderEffect = false;
        Boids.IsPlayerInvisible = false;
        _playerRenderer.color = Color.white;
    }

    private void ReturnAcceleration()
    {
        _isUnderEffect = false;
        GetComponent<PlayerMovement>().acceleration = GetComponent<PlayerMovement>().defaultAcceleration;
        GetComponent<PlayerMovement>().turnSpeed = GetComponent<PlayerMovement>().defaultTurnSpeed;
        _playerRenderer.color = Color.white;
    }

    private void RemoveKillerEffect()
    {
        _playerRenderer.color = Color.white;
        _isUnderEffect = false;
        _isUnderKillerEffect = false;
    }
}