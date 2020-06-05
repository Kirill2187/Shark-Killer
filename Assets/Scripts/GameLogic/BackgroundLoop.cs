using UnityEngine;

public class BackgroundLoop : MonoBehaviour
{
    private Camera _mainCamera;
    private Vector2 _screenBounds;

    public float choke;
    public GameObject[] levels;
    public float scrollSpeed;

    private void Start()
    {
        _mainCamera = Camera.main;
        _screenBounds =
            _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _mainCamera.transform.position.z));
        foreach (var obj in levels) LoadChildObjects(obj);
    }

    private void LoadChildObjects(GameObject obj)
    {
        var objectWidth = obj.GetComponent<SpriteRenderer>().bounds.size.x - choke;
        var childrenNeeded = (int) Mathf.Ceil(_screenBounds.x * 2 / objectWidth);
        var clone = Instantiate(obj);
        for (var i = 0; i <= childrenNeeded; i++)
        {
            var pos = new Vector3(objectWidth * i, obj.transform.position.y, 0);
            var c = Instantiate(clone, pos, Quaternion.identity, obj.transform);
            c.transform.localScale *= 1f / obj.transform.localScale.x;
            c.name = obj.name + i;
        }

        Destroy(clone);
        Destroy(obj.GetComponent<SpriteRenderer>());
    }

    private void RepositionChildObjects(GameObject obj)
    {
        var children = obj.GetComponentsInChildren<Transform>();
        if (children.Length > 1)
        {
            var firstChild = children[1].gameObject;
            var lastChild = children[children.Length - 1].gameObject;
            var halfObjectWidth = lastChild.GetComponent<SpriteRenderer>().bounds.extents.x - choke;
            if (transform.position.x + _screenBounds.x > lastChild.transform.position.x + halfObjectWidth)
            {
                firstChild.transform.SetAsLastSibling();
                firstChild.transform.position = new Vector3(lastChild.transform.position.x + halfObjectWidth * 2,
                    lastChild.transform.position.y, lastChild.transform.position.z);
            }
            else if (transform.position.x - _screenBounds.x < firstChild.transform.position.x - halfObjectWidth)
            {
                lastChild.transform.SetAsFirstSibling();
                lastChild.transform.position = new Vector3(firstChild.transform.position.x - halfObjectWidth * 2,
                    firstChild.transform.position.y, firstChild.transform.position.z);
            }
        }
    }

    private void Update()
    {
        var velocity = Vector3.zero;
        var desiredPosition = transform.position + new Vector3(scrollSpeed, 0, 0);
        var smoothPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 0.3f);
        transform.position = smoothPosition;
    }

    private void LateUpdate()
    {
        foreach (var obj in levels) RepositionChildObjects(obj);
    }
}