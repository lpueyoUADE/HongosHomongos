using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    public Vector3 _offset = Vector3.zero;
    [Range(0.1f, 2)] public float _tolerance = 0.25f;
    [Range(0.1f, 5)] public float _transitionSpeed = 3;

    [Header("Following object")]
    public GameObject _currentTarget;
    public Vector3 _lastTargetPos = Vector3.zero;

    [Header("Status")]
    public bool _cameraArrivedToPos;

    private void Awake()
    {
        CameraEvents._camera = GetComponent<Camera>();
        CameraEvents.OnCameraUpdateObjectToFollow += UpdateObjectToFollow;
    }

    private void FixedUpdate()
    {
        if (_currentTarget != null || _lastTargetPos != Vector3.zero)
        {
            Vector3 positionResult;
            if (_currentTarget != null)
            {
                _lastTargetPos = _currentTarget.transform.position;
                positionResult = _currentTarget.transform.position + _offset;
            }

            else positionResult = _lastTargetPos + _offset;                

            _cameraArrivedToPos = Vector3.Distance(transform.position, positionResult) < _tolerance;
            if (!_cameraArrivedToPos) transform.position = Vector3.LerpUnclamped(transform.position, positionResult, _transitionSpeed * 0.05f);
        }
    }

    private void OnDestroy()
    {
        CameraEvents.OnCameraUpdateObjectToFollow -= UpdateObjectToFollow;
    }

    private void UpdateObjectToFollow(GameObject newObject)
    {
        if (newObject == null) return;
        _currentTarget = newObject;
    }
}
