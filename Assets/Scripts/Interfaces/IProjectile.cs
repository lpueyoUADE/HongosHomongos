using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile
{
    void UpdateData(CharacterAbilityData abilityData, Vector3 direction, float speedMultiplier, GameObject owner, CapsuleCollider ownerCollider);
    void OnDeath();
    void OnCharacterHit(Collision character);
    void DamageCharacter(IDamageable objetive);
    void OnWorldHit();


    public GameObject Projectile { get; }
}
