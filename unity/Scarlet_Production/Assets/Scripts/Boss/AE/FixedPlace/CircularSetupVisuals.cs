using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircularSetupVisuals : FixedPlaceSetupVisuals {

    public Canvas m_Canvas;
    public Slider m_Slider;

    public float m_TimeShown;
    public float m_Size;
    public float m_Angle;

    public override void Show(SetupCallback callback)
    {
        base.Show(callback);

        MLog.Log(LogType.BattleLog, 2, "Showing Circular Setup Visuals");

        this.m_Canvas.gameObject.SetActive(true);
        this.m_Canvas.transform.localScale = new Vector3(m_Size, m_Size, m_Size);


        this.m_Slider.value = m_Angle;

        this.m_Canvas.transform.rotation = Quaternion.Euler(90, this.transform.eulerAngles.y - m_Angle / 2, 0);

        StartCoroutine(StopAfter(m_TimeShown));

    }

    private IEnumerator StopAfter(float time)
    {
        yield return new WaitForSeconds(time);

        this.m_Callback.OnSetupOver();
    }

    public override void Hide()
    {
        base.Hide();

        this.m_Canvas.gameObject.SetActive(false);
    }

}
