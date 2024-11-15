using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    public Vector3 _offset = Vector3.zero;
    [Range(0.1f, 2)] public float _tolerance = 0.25f;
    [Range(0.1f, 5)] public float _transitionSpeed = 3;
    public Vector3 _freeLookOffset = Vector3.zero;

    [Header("Following object")]
    public GameObject _currentTarget;
    public Vector3 _lastTargetPos = Vector3.zero;

    [Header("Status")]
    public bool _cameraArrivedToPos;
    public bool _onFreeLookMode = false;

    private void Awake()
    {
        CameraEvents._camera = GetComponent<Camera>();
        CameraEvents.OnCameraUpdateObjectToFollow += UpdateObjectToFollow;
        CameraEvents.OnStartFreeLookMode += PortraitLookStart;
        CameraEvents.OnEndFreeLookMode += PortraitLookEnd;
    }

    private void FixedUpdate()
    {
        if (_currentTarget != null || _lastTargetPos != Vector3.zero)
        {
            Vector3 positionResult;
            if (_currentTarget != null)
            {
                _lastTargetPos = _currentTarget.transform.position;
                positionResult = _lastTargetPos + _offset + _freeLookOffset;
            }

            else positionResult = _lastTargetPos + _offset + _freeLookOffset;                

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

    private void PortraitLookStart(Vector3 offset)
    {
        if (!_onFreeLookMode) InGameUIEvents.OnPlayUISound(GameManagerEvents.ModeSettings.PortraitLookStartSound);
        _onFreeLookMode = true;

        _freeLookOffset += offset * GameManagerEvents.UserConfig.FreeLookSpeed;
        InGameUIEvents.OnFreeLookMode?.Invoke(true);
    }

    private void PortraitLookEnd()
    {
        if (_onFreeLookMode) InGameUIEvents.OnPlayUISound(GameManagerEvents.ModeSettings.PortraitLookEndSound);
        _onFreeLookMode = false;

        _freeLookOffset = Vector3.zero;
        InGameUIEvents.OnFreeLookMode?.Invoke(false);
    }
}
