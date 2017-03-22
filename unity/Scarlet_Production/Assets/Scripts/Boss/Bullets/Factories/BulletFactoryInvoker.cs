using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFactoryInvoker : MonoBehaviour {

    public enum PlaySpawnAudio {None, All, PerIteration };

    public BulletFactory[] m_Factories;
    public Transform m_Base;

    public int m_Iterations;
    public float m_TimeBetweenIterations;

    protected int m_CurrentIteration;

    protected IEnumerator m_LaunchIterationTimer;

    public PlaySpawnAudio m_PlayAudio = PlaySpawnAudio.None;

    public virtual void Launch(BulletSwarm bs, IEnumerator onFinish = null)
    {
        m_CurrentIteration = 0;
        SpawnIteration(bs, onFinish);
    }

    protected virtual void SpawnIteration(BulletSwarm bs, IEnumerator onFinish = null)
    {
        if (m_PlayAudio == PlaySpawnAudio.PerIteration)
            FancyAudioEffectsSoundPlayer.Instance.PlayBulletSpawnSound(m_Base.transform, 1f);

        for (int i = 0; i < m_Factories.Length; i++)
        {
            BulletBehaviour b = CreateBullet(i);

            if(m_PlayAudio == PlaySpawnAudio.All)
                FancyAudioEffectsSoundPlayer.Instance.PlayBulletSpawnSound(b.transform, .3f);

            b.Launch(bs);
            bs.m_Instances.Add(b);
        }

        m_CurrentIteration++;

        if (m_CurrentIteration >= m_Iterations)
        {
            if (onFinish != null)
                StartCoroutine(onFinish);

            EventManager.TriggerEvent(BulletAttack.END_EVENT_NAME);
            return;
        }

        m_LaunchIterationTimer = BetweenIterations(bs, onFinish);
        StartCoroutine(m_LaunchIterationTimer);
    }

    protected virtual BulletBehaviour CreateBullet(int factoryIndex)
    {
        BulletBehaviour b = m_Factories[factoryIndex].CreateBullet();

        b.transform.position = m_Base.transform.position;
        b.transform.rotation = m_Base.transform.rotation;

        b.transform.rotation = Quaternion.Euler(0, b.transform.rotation.eulerAngles.y, 0);

        return b;
    }

    protected virtual IEnumerator BetweenIterations(BulletSwarm bs, IEnumerator onFinish = null)
    {
        yield return new WaitForSeconds(m_TimeBetweenIterations);

        SpawnIteration(bs, onFinish);
    }

    public void Cancel()
    {
        if (m_LaunchIterationTimer != null)
            StopCoroutine(m_LaunchIterationTimer);
    }

}
