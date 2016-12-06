using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChangePerks : MonoBehaviour {

    public Image perk1;
    public Image perk2;
    public Image perk3;

    private float tLeft;
    private float tRight;
    private float speed = 4f;
    private bool perkChangeLeft = false;
    private bool perkChangeRight = false;


    // Use this for initialization
    void Start () {
	
	}
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("PerkLeft"))
        {
            if(!perkChangeLeft && !perkChangeRight)
                perkChangeLeft = true;
        }
        if (Input.GetButtonDown("PerkRight"))
        {
            if (!perkChangeLeft && !perkChangeRight)
                perkChangeRight = true;
        }
        if (perkChangeLeft)
        {
            if (tLeft <= 1)
            {
                tLeft += Time.deltaTime * speed;
                if (perk3.fillAmount != 0 && perk1.fillAmount == 0)
                {
                    perk3.fillOrigin = (int)Image.OriginHorizontal.Right;
                    perk2.fillOrigin = (int)Image.OriginHorizontal.Left;
                    perk3.fillAmount = Mathf.Lerp(1, 0, tLeft);
                    perk2.fillAmount = Mathf.Lerp(0, 1, tLeft);
                    if (perk3.fillAmount == 0)
                    {
                        perkChangeLeft = false;
                        tLeft = 0;
                    }
                }
                else if (perk1.fillAmount != 0 && perk2.fillAmount == 0)
                {
                    perk1.fillOrigin = (int)Image.OriginHorizontal.Right;
                    perk3.fillOrigin = (int)Image.OriginHorizontal.Left;
                    perk1.fillAmount = Mathf.Lerp(1, 0, tLeft);
                    perk3.fillAmount = Mathf.Lerp(0, 1, tLeft);
                    if (perk1.fillAmount == 0)
                    {
                        perkChangeLeft = false;
                        tLeft = 0;
                    }
                }
                else if (perk2.fillAmount != 0 && perk3.fillAmount == 0)
                {
                    perk2.fillOrigin = (int)Image.OriginHorizontal.Right;
                    perk1.fillOrigin = (int)Image.OriginHorizontal.Left;
                    perk2.fillAmount = Mathf.Lerp(1, 0, tLeft);
                    perk1.fillAmount = Mathf.Lerp(0, 1, tLeft);
                    if (perk2.fillAmount == 0)
                    {
                        perkChangeLeft = false;
                        tLeft = 0;
                    }
                }
            }

        }


        if (perkChangeRight)
        {
            if (tRight <= 1)
            {
                tRight += Time.deltaTime * speed;
                if (perk1.fillAmount != 0 && perk3.fillAmount == 0)
                {
                    perk1.fillOrigin = (int)Image.OriginHorizontal.Left;
                    perk2.fillOrigin = (int)Image.OriginHorizontal.Right;
                    perk1.fillAmount = Mathf.Lerp(1, 0, tRight);
                    perk2.fillAmount = Mathf.Lerp(0, 1, tRight);
                    if (perk1.fillAmount == 0)
                    {
                        perkChangeRight = false;
                        tRight = 0;
                    }
                }
                else if (perk2.fillAmount != 0 && perk1.fillAmount == 0)
                {
                    perk2.fillOrigin = (int)Image.OriginHorizontal.Left;
                    perk3.fillOrigin = (int)Image.OriginHorizontal.Right;
                    perk2.fillAmount = Mathf.Lerp(1, 0, tRight);
                    perk3.fillAmount = Mathf.Lerp(0, 1, tRight);
                    if (perk2.fillAmount == 0)
                    {
                        perkChangeRight = false;
                        tRight = 0;
                    }
                }
                else if (perk3.fillAmount != 0 && perk2.fillAmount == 0)
                {
                    perk3.fillOrigin = (int)Image.OriginHorizontal.Left;
                    perk1.fillOrigin = (int)Image.OriginHorizontal.Right;
                    perk3.fillAmount = Mathf.Lerp(1, 0, tRight);
                    perk1.fillAmount = Mathf.Lerp(0, 1, tRight);
                    if (perk3.fillAmount == 0)
                    {
                        perkChangeRight = false;
                        tRight = 0;
                    }
                }
            }

        }

    }

    public void OnChangePerk()
        
    {
        if(perk1.enabled)
        {
            perk1.fillAmount = Mathf.Lerp(1, 0, 1000);
            perk2.fillAmount = Mathf.Lerp(0, 1, 1000);
        }
        else if(perk2.enabled)
        {
            perk2.fillAmount = Mathf.Lerp(1, 0, 1000);
            perk3.fillAmount = Mathf.Lerp(0, 1, 1000);
        }
        else if(perk3.enabled)
        {
            perk3.fillAmount = Mathf.Lerp(1, 0, 1000);
            perk1.fillAmount = Mathf.Lerp(0, 1, 1000);
        }
    }
}
