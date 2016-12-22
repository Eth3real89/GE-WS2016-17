using System.Collections;
using UnityEngine;

public class HealthPotionVisuals : MonoBehaviour, PlayerHealCommand.NumPotionListener {

    public GameObject m_ImagePrefab;

    private GameObject[] m_Images;

    public void OnNumberOfPotionsUpdated(int num)
    {
        if (m_ImagePrefab == null)
            return;

        ClearExistingImages();

        m_Images = new GameObject[num];

        ShowPotionIcons(num);
    }

    private void ShowPotionIcons(int num)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject singleImg = GameObject.Instantiate(m_ImagePrefab);
            singleImg.transform.SetParent(this.transform);

            m_Images[i] = singleImg;

            RectTransform rt = singleImg.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = new Vector2(60 + i * 60, -60);
            }
        }
    }

    private void ClearExistingImages()
    {
        if (m_Images != null)
        {
            foreach (GameObject image in m_Images)
            {
                GameObject.Destroy(image);
            }
        }
    }

}
