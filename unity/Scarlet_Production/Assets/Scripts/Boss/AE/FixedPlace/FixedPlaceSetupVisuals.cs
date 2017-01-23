using System.Collections;
using UnityEngine;

public class FixedPlaceSetupVisuals : MonoBehaviour
{

    protected SetupCallback m_Callback;

    public virtual void ShowSetup(SetupCallback callback)
    {
        this.m_Callback = callback;
    }

    public virtual void HideAttack()
    {
    }

    public interface SetupCallback
    {
        void OnSetupOver();

    }

}
