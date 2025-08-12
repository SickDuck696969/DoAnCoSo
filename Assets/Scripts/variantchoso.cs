using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class variantchoso : MonoBehaviour
{
    public Piece variant;
    public Army draft;
    public GameObject paper;
    public GameObject abi;
    public GameObject da;
    public Player l;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        variant = new Pawn();
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Image>().sprite = variant.Skin;
        if(this.GetComponent<Image>().sprite != null)
        {
            this.GetComponent<Image>().color = Color.white; ;
        }else
        {
            float alpha = 114f / 255f;
            Color customColor = new Color(0f, 0f, 0f, alpha);
            if(l.pColor == "White") customColor = new Color(1f, 1f, 1f, alpha);
            this.GetComponent<Image>().color = customColor;
        }
    }

    private void OnMouseUp()
    {
        if (this.GetComponent<Image>().sprite != null)
        {
            GameObject[] butts = GameObject.FindGameObjectsWithTag("skillbutt");
            Debug.Log(butts);
            foreach (GameObject butt in butts)
            {
                Destroy(butt);
            }
            if (variant.kit.Count > 2)
            {
                GameObject p = GameObject.Instantiate(paper);
                foreach (var g in variant.kit)
                {
                    if (variant.kit.IndexOf(g) != 0 && variant.kit.IndexOf(g) != 1)
                    {
                        GameObject a = GameObject.Instantiate(abi, p.transform.Find("Canvas/Image"));
                        if(l.pColor == "White")
                        {
                            a.transform.Find("name").GetComponent<TMP_Text>().color  = Color.white;
                            a.transform.Find("name").GetComponent<TMP_Text>().text = g.name;
                            a.transform.Find("AP").GetComponent<TMP_Text>().color = Color.white;
                            if (g.AP != 0)
                            {
                                a.transform.Find("AP").GetComponent<TMP_Text>().text = g.AP.ToString();
                                a.GetComponent<descf>().desc = $"<b>{variant.name}/{g.name}/{g.AP}</b>\n{g.desc}";
                            }
                            else
                            {
                                a.transform.Find("AP").GetComponent<TMP_Text>().text = "FREE";
                                a.GetComponent<descf>().desc = $"<b>{variant.name}/{g.name}</b>\n{g.desc}";
                            }
                            a.transform.Find("type").GetComponent<TMP_Text>().color = new Color32(0, 150, 254, 255);
                            a.transform.Find("type").GetComponent<TMP_Text>().text = g.type;
                            a.transform.Find("Image").GetComponent<Image>().color = Color.white;
                        }
                        else
                        {
                            a.transform.Find("name").GetComponent<TMP_Text>().text = g.name;

                            if (g.AP != 0)
                            {
                                a.transform.Find("AP").GetComponent<TMP_Text>().text = g.AP.ToString();
                                a.GetComponent<descf>().desc = $"<b>{variant.name}/{g.name}/{g.AP}</b>\n{g.desc}";
                            }
                            else
                            {
                                a.transform.Find("AP").GetComponent<TMP_Text>().text = "FREE";
                                a.GetComponent<descf>().desc = $"<b>{variant.name}/{g.name}</b>\n{g.desc}";
                                a.transform.Find("type").GetComponent<TMP_Text>().text = g.type;
                            }
                        }
                        
                    }

                }
            }
            foreach (Piece cp in draft.army)
            {
                if (cp.suit == variant.suit)
                {
                    draft.army[draft.army.IndexOf(cp)] = variant.Clone();
                    draft.Mu();
                    break;
                }
            }
        }
    }
}
