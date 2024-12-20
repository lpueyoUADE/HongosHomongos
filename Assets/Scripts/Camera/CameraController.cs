using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    public float _cameraOgSize;
    public Vector3 _offset = Vector3.zero;
    [Range(0.1f, 2)] public float _tolerance = 0.25f;
    [Range(0.1f, 5)] public float _transitionSpeed = 3;
    public Vector3 _freeLookOffset = Vector3.zero;
    public float _freeLookDistace = 5;

    [Header("Following object")]
    public GameObject _currentTarget;
    public Vector3 _lastTargetPos = Vector3.zero;

    [Header("Status")]
    public Vector3 _ogCamPos;
    public bool _updateHeight = true;
    public bool _cameraArrivedToPos;
    public bool _onFreeLookMode = false;

    private void Awake()
    {
        _ogCamPos = transform.position;
        CameraEvents._camera = GetComponent<Camera>();
        CameraEvents.OnCameraUpdateObjectToFollow += UpdateObjectToFollow;
        CameraEvents.OnStartFreeLookMode += PortraitLookStart;
        CameraEvents.OnEndFreeLookMode += PortraitLookEnd;
    }

    private void OnDestroy()
    {
        CameraEvents.OnCameraUpdateObjectToFollow -= UpdateObjectToFollow;
        CameraEvents.OnStartFreeLookMode -= PortraitLookStart;
        CameraEvents.OnEndFreeLookMode -= PortraitLookEnd;
    }
    
    private void Start() 
    {
        _cameraOgSize = CameraEvents.Cam.orthographicSize;
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

            if (!_updateHeight) positionResult.y = _ogCamPos.y;
            _cameraArrivedToPos = Vector3.Distance(transform.position, positionResult) < _tolerance;

            if (!_cameraArrivedToPos) transform.position = Vector3.LerpUnclamped(transform.position, positionResult, _transitionSpeed * 0.05f);
        }

        if (_onFreeLookMode && CameraEvents.Cam.orthographicSize < _cameraOgSize + _freeLookDistace) 
            CameraEvents.Cam.orthographicSize = Mathf.LerpUnclamped(CameraEvents.Cam.orthographicSize, _cameraOgSize + _freeLookDistace, Time.fixedDeltaTime * 2);
            
        else
        {
            if (CameraEvents.Cam.orthographicSize > _cameraOgSize)
                CameraEvents.Cam.orthographicSize = Mathf.LerpUnclamped(CameraEvents.Cam.orthographicSize, _cameraOgSize, Time.fixedDeltaTime * 2);
        }
    }

    private void UpdateObjectToFollow(GameObject newObject, bool updateZ = true)
    {
        if (newObject == null) return;
        _currentTarget = newObject;
        _updateHeight = updateZ;
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
