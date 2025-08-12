using TMPro;
using UnityEngine;

public class descf : MonoBehaviour
{
    public string desc;
    public GameObject a;
    public Player l;
    public void OnMouseUp()
    {
        if(desc != null)
        {
            GameObject dfg = GameObject.Find("Image (2)");
            if(l.pColor == "White")
            {
                dfg.transform.Find("Text (TMP)").GetComponent<TMP_Text>().color = Color.white;
            }
            dfg.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text = desc;
        }
    }
}
