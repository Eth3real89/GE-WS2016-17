using System.Collections;
using UnityEngine;

public class HealthPotionVisuals : MonoBehaviour, PlayerHealCommand.NumPotionListener {

    public GameObject m_ImagePrefab;

    private GameObject[] m_Images;

    // Use this for initialization
    void Start () {
        m_Images = new GameObject[0];
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void OnNumberOfPotionsUpdated(int num)
    {
        foreach(GameObject image in m_Images)
        {
            GameObject.Destroy(image);
        }

        m_Images = new GameObject[num];

        for(int i = 0; i < num; i++)
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

    private IEnumerator FixPosition(RectTransform rt, Vector3 position)
    {
        yield return new WaitForEndOfFrame();

        rt.position = position;
    }

}
