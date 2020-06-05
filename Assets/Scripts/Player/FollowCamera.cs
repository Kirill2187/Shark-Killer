using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        Debug.Log(_mainCamera.transform.position.x);
        transform.Translate(_mainCamera.transform.position.x - transform.position.x, 0, 0);
    }
}