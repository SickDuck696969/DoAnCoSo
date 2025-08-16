using System.IO;
using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class variantchoso : NetworkBehaviour
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
            this.GetComponent<Image>().color = UnityEngine.Color.white; ;
        }else
        {
            float alpha = 114f / 255f;
            UnityEngine.Color customColor = new UnityEngine.Color(0f, 0f, 0f, alpha);
            if(l.pColor == "White") customColor = new UnityEngine.Color(1f, 1f, 1f, alpha);
            this.GetComponent<Image>().color = customColor;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void AssignArmyServerRpc(string color, string name, string skin)
    {
        foreach (var variant in draft.roster)
        {
            if (name == variant.name)
            {
                if (color == "White")
                {
                    Piece a = variant.Clone();
                    a.Skin = Resources.Load<Sprite>(skin);
                    Debug.Log(a.name + " " + a.Skin);
                    foreach (Piece cp in draft.whitearmy)
                    {
                        if (cp.suit == variant.suit)
                        {
                            draft.whitearmy[draft.whitearmy.IndexOf(cp)] = a.Clone();
                            break;
                        }
                    }
                    draft.PrintwhiteArmy();
                }
                else if (color == "Black")
                {
                    Piece a = variant.Clone();
                    a.Skin = Resources.Load<Sprite>(skin);
                    Debug.Log(a.name + " " + a.Skin);
                    foreach (Piece cp in draft.blackarmy)
                    {
                        if (cp.suit == variant.suit)
                        {
                            draft.blackarmy[draft.blackarmy.IndexOf(cp)] = a.Clone();
                            break;
                        }
                    }
                }
            }
        }
        AssignArmyClientRpc(color, name, skin);
    }
    [ClientRpc(RequireOwnership = false)]
    public void AssignArmyClientRpc(string color, string name, string skin)
    {
        foreach (var variant in draft.roster)
        {
            if (name == variant.name)
            {
                if (color == "White")
                {
                    Piece a = variant.Clone();
                    a.Skin = Resources.Load<Sprite>(skin);
                    Debug.Log(a.name + " " + a.Skin);
                    foreach (Piece cp in draft.whitearmy)
                    {
                        if (cp.suit == variant.suit)
                        {
                            draft.whitearmy[draft.whitearmy.IndexOf(cp)] = a.Clone();
                            break;
                        }
                    }
                    draft.PrintwhiteArmy();
                }
                else if (color == "Black")
                {
                    Piece a = variant.Clone();
                    a.Skin = Resources.Load<Sprite>(skin);
                    Debug.Log(a.name + " " + a.Skin);
                    foreach (Piece cp in draft.blackarmy)
                    {
                        if (cp.suit == variant.suit)
                        {
                            draft.blackarmy[draft.blackarmy.IndexOf(cp)] = a.Clone();
                            break;
                        }
                    }
                }
            }
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
                            a.transform.Find("name").GetComponent<TMP_Text>().color  = UnityEngine.Color.white;
                            a.transform.Find("name").GetComponent<TMP_Text>().text = g.name;
                            a.transform.Find("AP").GetComponent<TMP_Text>().color = UnityEngine.Color.white;
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
                            a.transform.Find("Image").GetComponent<Image>().color = UnityEngine.Color.white;
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
            AssignArmyServerRpc(l.pColor, variant.name, variant.Skin.name);
        }
    }
}
