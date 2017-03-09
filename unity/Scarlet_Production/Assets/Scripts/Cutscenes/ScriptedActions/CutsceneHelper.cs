using UnityEngine;

public class CutsceneHelper : MonoBehaviour
{
    public static CutsceneHelper Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void StartAtmosphericMusic()
    {
        AudioController.Instance.FadeIn("Atmosphere", 0.6f, 4f);
    }

    public void StopAtmosphericMusic()
    {
        AudioController.Instance.StopSound("Atmosphere");
    }
}
