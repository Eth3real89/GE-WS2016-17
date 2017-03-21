using UnityEngine;

public class PreDHCutscene : MonoBehaviour
{
    public void DHVoiceline1()
    {
        new FARQ().ClipName("dh").StartTime(0f).EndTime(5.6f).Location(Camera.main.transform).Play();
    }

    public void DHVoiceline2()
    {
        new FARQ().ClipName("dh").StartTime(8.6f).EndTime(17.6f).Location(Camera.main.transform).Play();
    }

    public void DHVoiceline3()
    {
        new FARQ().ClipName("dh").StartTime(21.7f).EndTime(33.6f).Location(Camera.main.transform).Play();
    }

    public void DHVoiceline4()
    {
        new FARQ().ClipName("dh").StartTime(36.0f).EndTime(44.2f).Location(Camera.main.transform).Play();
    }

    public void DHVoiceline5()
    {
        new FARQ().ClipName("dh").StartTime(48.4f).EndTime(57.8f).Location(Camera.main.transform).Play();
    }
}