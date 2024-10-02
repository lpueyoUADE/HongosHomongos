using UnityEngine;

public class CharacterWidgetCollision : MonoBehaviour
{
    public float _speed = 5;
    public float _collisionSpeedDivisor = 2;
    public float _tolerance = 0.25f;
    public Vector3 _localPos;
    public Quaternion _rotation;
    public bool _doLerp = false;

    private void Awake()
    {
        _localPos = transform.localPosition;
        _rotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        float speed = Time.deltaTime * _speed;

        if (!_doLerp) speed /= _collisionSpeedDivisor;
        transform.localPosition = Vector3.LerpUnclamped(transform.localPosition, _localPos, speed);
        transform.rotation = Quaternion.LerpUnclamped(transform.rotation, _rotation, speed);
    }

    private void OnCollisionStay(Collision collision)
    {
        _doLerp = false;
    }

    private void OnCollisionExit(Collision collision)
    {
        _doLerp = true;
    }
}
