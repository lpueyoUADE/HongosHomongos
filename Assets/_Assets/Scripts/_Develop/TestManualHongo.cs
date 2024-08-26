using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestManualHongo : MonoBehaviour
{
    private BaseHongo _baseScript;
    private CapsuleCollider _baseCollider;
    private float chargeValue;

    public GameObject weapon;
    public GameObject projectile;
    public GameObject pOut;


    public Image fillBar;
    public bool charge;
    public bool triggerProjectile;
    public float startTime;
    public float lastProjectileForce;

    public Vector3 inputDir => new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    public bool isCharging => Input.GetButton("Jump");

    private void Awake()
    {
        _baseScript = GetComponent<BaseHongo>();
        _baseCollider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        charge = isCharging;

        if (!isCharging && triggerProjectile)
        {
            Debug.Log("Spawn projectile");
            lastProjectileForce = fillBar.fillAmount;

            var spawnedProjectile = Instantiate(projectile, weapon.transform.position, new Quaternion());
            SphereCollider collider = spawnedProjectile.GetComponent<SphereCollider>();
            Physics.IgnoreCollision(_baseCollider, collider, true);

            IProjectile proData = spawnedProjectile.GetComponent<IProjectile>();
            proData?.UpdateDirection(pOut.transform.right);
            proData?.UpdateSpeedMultiplier(lastProjectileForce);
            
            triggerProjectile = false;
            chargeValue = 0;
        }


        if (isCharging)
        {
            if (!triggerProjectile) startTime = Time.time;

            triggerProjectile = true;
            chargeValue = (Time.time - startTime) * 2;
            fillBar.fillAmount = Mathf.PingPong(chargeValue, 1);
        }

        else fillBar.fillAmount = 0;
    }

    private void FixedUpdate()
    {
        if (inputDir.y != 0) weapon.transform.Rotate(Vector3.forward, inputDir.y);
        if (inputDir.x != 0) _baseScript.Move(new Vector3(inputDir.x, 0));
    }
}
