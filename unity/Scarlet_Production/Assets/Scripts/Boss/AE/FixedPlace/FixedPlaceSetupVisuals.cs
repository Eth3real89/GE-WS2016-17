using System.Collections;
using UnityEngine;

public class FixedPlaceSetupVisuals : MonoBehaviour {

    protected SetupCallback m_Callback;

    public virtual void Show(SetupCallback callback)
    {
        this.m_Callback = callback;
    }

    public virtual void Hide()
    {
    }

    public interface SetupCallback
    {
        void OnSetupOver();

    }

}
