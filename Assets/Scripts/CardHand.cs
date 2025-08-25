using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardHand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform rectTransform;
    public float curveStrength = 29f;
    public float startcurve;
    public float spacing = 3f;
    public float xpadding = 0f;
    public float ypadding = 0f;
    public Army army;
    public Player player;
    public List<Ability> playlist= new List<Ability>();
    public GameObject slot;
    public bool isOn;
    public float startpoint;
    public float startstartcurve;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startpoint = ypadding;
        startcurve = curveStrength;
        startstartcurve = startcurve;

        if (player != null)
        {
            if (player.pColor == "Black")
            {
                foreach (Piece variant in army.blackarmy)
                {
                    foreach (Ability ab in variant.kit)
                    {
                        if (ab.name != "Move" && ab.name != "Pass" && ab.type != "passive")
                        {
                            playlist.Add(ab);
                        }
                    }
                }
            }
            else
            {
                foreach (Piece variant in army.whitearmy)
                {
                    foreach (Ability ab in variant.kit)
                    {
                        if (ab.name != "Move" && ab.name != "Pass" && ab.type != "passive")
                        {
                            playlist.Add(ab);
                        }
                    }
                }
            }
        }

        if(playlist.Count > 0)
        {
            foreach (Ability ab in playlist)
            {
                Debug.Log(ab);
                GameObject TrackSlot = Instantiate(slot, transform);
                TrackSlot.GetComponentInChildren<TrackCard>().ability = ab;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOn = true;
        curveStrength = 0.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOn = false;
    }

    private void Update()
    {
        rectTransform = GetComponent<RectTransform>();
        int childCount = transform.childCount;
        if (childCount == 0) return;

        float centerIndex = (childCount - 1) / 2f;
        if (isOn)
        {
            ypadding = Mathf.Lerp(
                ypadding,
                startpoint,
                30f * Time.deltaTime
            );
            startcurve = Mathf.Lerp(
                startcurve,
                startstartcurve,
                30f * Time.deltaTime
            );
        }
        else
        {
            ypadding = Mathf.Lerp(
                ypadding,
                startpoint + 5f,
                30f * Time.deltaTime
            );
            startcurve = Mathf.Lerp(
                startcurve,
                0,
                30f * Time.deltaTime
            );
        }
        if (curveStrength > 0)
        {
            curveStrength = startcurve * (1f - childCount * 0.1f);
        }
        for (int i = 0; i < childCount; i++)
        {
            RectTransform child = transform.GetChild(i).GetComponent<RectTransform>();
            if (child == null) continue;
            TrackCard grandchild = transform.GetChild(i).GetChild(0).GetComponent<TrackCard>();
            if (grandchild == null) continue;

            float x = (i - centerIndex) * spacing;

            float y = -Mathf.Pow(i - centerIndex, 2) * (curveStrength / 100f);

            child.anchoredPosition = new Vector2(x+xpadding, y-ypadding);


            if (grandchild.isOn || grandchild.isDragging)
            {
                float angle = 0f;
                child.localRotation = Quaternion.Euler(0, 0, angle);
            }
            else
            {
                float angle = -(i - centerIndex) * curveStrength * 0.1f;
                child.localRotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}
