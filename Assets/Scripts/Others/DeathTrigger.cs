using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
        if (!other.CompareTag("Character")) return;
        other.TryGetComponent(out IDamageable damageable);
        damageable?.AnyDamage(9999);
    }
}
