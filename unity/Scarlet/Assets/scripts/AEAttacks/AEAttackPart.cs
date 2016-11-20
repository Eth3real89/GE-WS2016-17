using UnityEngine;
using System.Collections;

/**
 * Area Effect Attacks are always Series of Setups and Attacks.
 * 
 * This script here provides a common interface for both AEAttackSetup and AEAttack.
 */
public abstract class AEAttackPart : Attack {

    public float delay;

    public abstract void LaunchPart();

    public abstract GameObject GetObject();

}
