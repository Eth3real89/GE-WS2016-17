using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGrenadeFactoryInvoker : BulletFactoryInvoker {

    protected override void SpawnIteration(BulletSwarm bs, IEnumerator onFinish = null)
    {
        base.SpawnIteration(bs, onFinish);
    }

    protected override BulletBehaviour CreateBullet(int factoryIndex)
    {
        return base.CreateBullet(factoryIndex);
    }

}
