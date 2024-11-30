using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLandMine : BaseProjectile
{
    public override void OnCharacterHit(Collision character)
    {
        if (_resSpawned) return;
        SpawnTrap();
    }

    public override void OnWorldHit()
    {
        if (_resSpawned) return;
        SpawnTrap();
    }

    private void SpawnTrap()
    {
        _resSpawned = true;
        GameObject mineRef = Instantiate(AbilityData.AbilityResidualPrefab, transform.position, transform.rotation);
        mineRef.TryGetComponent(out Residual_Landmine mineScript);
        if (mineScript) mineScript.UpdateTrapData(AbilityData.AbilityProjectileBaseDamage, AreaAbilityData.AbilityAreaRadius, AreaAbilityData.AbilityAreaDecayDamageByDistance);
        Destroy(gameObject);
    }
}
