using UnityEngine;

public class Mine : MonoBehaviour
{
    private Camera _camera;
    public Sprite chain1, chain2, bomb;
    public float chainOffset = 0.1f;
    private int chainsCount;
    public Color color;
    public GameObject explosion;

    public float explosionRange = 5f;

    [HideInInspector] public bool isDestroyed;
    public LayerMask mask;
    public float mineOffset = 0.2f;
    public float size = 0.3f;

    private void Awake()
    {
        _camera = Camera.main;
    }

    public void Init(int chainsCount)
    {
        this.chainsCount = chainsCount;
        var type = (chainsCount + 0) % 2;
        var scale = new Vector3(size, size, size);

        for (var i = 0; i < chainsCount; i++)
        {
            var chain = new GameObject("chain" + i);
            chain.transform.localScale = scale;
            chain.transform.position = new Vector3(transform.position.x, transform.position.y + i * chainOffset, 0);
            chain.transform.parent = transform;

            var spriteRenderer = chain.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "Mine";
            spriteRenderer.sortingOrder = type;
            spriteRenderer.sprite = type == 0 ? chain1 : chain2;
            spriteRenderer.color = color;

            type++;
            type %= 2;
        }

        var mine = new GameObject("body");
        mine.transform.localScale = scale;
        mine.transform.position = new Vector3(transform.position.x,
            transform.position.y + chainsCount * chainOffset + mineOffset, 0);
        mine.transform.parent = transform;

        var mineSpriteRenderer = mine.AddComponent<SpriteRenderer>();
        mineSpriteRenderer.sortingLayerName = "Mine";
        mineSpriteRenderer.sortingOrder = 0;
        mineSpriteRenderer.sprite = bomb;
        mineSpriteRenderer.flipY = true;
        mineSpriteRenderer.color = color;

        var body = mine.AddComponent<CircleCollider2D>();
        body.offset = new Vector2(0, mineOffset);
        body.radius = size * 14f;
    }

    public void Explode()
    {
        Destroy(Instantiate(explosion, GetCenter(), Quaternion.identity), 0.5f);
        Destroy();

        var center = GetCenter();
        var colliders = Physics2D.OverlapCircleAll(center, explosionRange, mask);
        foreach (var collider in colliders)
        {
            if (!collider.attachedRigidbody) continue;
            if (collider.attachedRigidbody.bodyType != RigidbodyType2D.Dynamic) continue;

            if (collider.transform.parent && collider.transform.parent.gameObject.CompareTag("fish"))
                collider.transform.parent.gameObject.GetComponent<Fish>().isShocked = true;

            var range = (Vector2) collider.transform.position - center;
            collider.attachedRigidbody.AddForce(range.normalized / range.sqrMagnitude * 8f, ForceMode2D.Impulse);
        }
    }

    private void Update()
    {
        if (transform.position.x < _camera.transform.position.x - 15) Destroy();
    }

    private void Destroy()
    {
        isDestroyed = true;
        Destroy(gameObject);
        GameObject.Find("MinesManager").GetComponent<MinesManager>().mines.Remove(this);
    }

    public Vector2 GetCenter()
    {
        return new Vector2(transform.position.x, transform.position.y + chainsCount * chainOffset + mineOffset);
    }
}