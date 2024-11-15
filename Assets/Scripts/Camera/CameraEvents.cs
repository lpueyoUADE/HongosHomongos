using System;
using UnityEngine;

public class CameraEvents : MonoBehaviour
{
    public static Camera _camera;
    public static Action<GameObject> OnCameraUpdateObjectToFollow;

    public static Camera Cam => _camera;
}
