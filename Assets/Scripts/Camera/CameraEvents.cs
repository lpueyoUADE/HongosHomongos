using System;
using UnityEngine;

public class CameraEvents : MonoBehaviour
{
    public static Camera _camera;
    public static Action<GameObject, bool> OnCameraUpdateObjectToFollow;
    public static Camera Cam => _camera;

    public static Action<Vector3> OnStartFreeLookMode;
    public static Action OnEndFreeLookMode;
}
