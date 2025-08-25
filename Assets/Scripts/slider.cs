using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class slider : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    public float swipeThreshold = 5f;
    public float requiredDistance = 5f;
    private Vector3 startPosition;
    private Vector3 lastPosition;
    private Vector3 swipeDirection;
    public Ability ability;
    public GameObject Track;
    private bool activated = false;
    public List<Ability> queue = new List<Ability>();
    public float distanceMoved;
    public bool bgon = true;
    public AudioSource speaker;
    public GameObject controller;
    public Army army;
    public Player player;

    void Start()
    {
        speaker = transform.parent.parent.GetComponent<AudioSource>();
        controller = GameObject.FindGameObjectWithTag("GameController");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            Track = eventData.pointerDrag;
            Track.GetComponent<TrackCard>().snapped = true;
            startPosition = Track.transform.position;
            lastPosition = Track.transform.position;
            distanceMoved = 0;
            activated = false;
            bgon = true;
            AudioClip clip = Resources.Load<AudioClip>("Sounds/cardplace");
            speaker.PlayOneShot(clip, 0.7f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Track != null)
        {
            Track.GetComponent<TrackCard>().snapped = false;
            if (Track.GetComponent<TrackCard>().sliding == false)
            {
                bgon = false;
            }
        } 
    }

    public void OnDrop(PointerEventData eventData)
    {
        Track = eventData.pointerDrag;
        startPosition = Track.GetComponent<RectTransform>().localPosition;
        Track.GetComponent<TrackCard>().sliding = true;
        bgon = false;
    }
 
    void queuetrack(Ability track)
    {
        int APgauge = (player.pColor == "White") ? controller.GetComponent<Game>().white_AP.Value : controller.GetComponent<Game>().black_AP.Value;
        Debug.Log($"{track.type}/{controller.GetComponent<Game>().phase.Value}");
        if (track.AP <= APgauge && track.type == controller.GetComponent<Game>().phase.Value)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Piece");
            foreach (GameObject piece in objs)
            {
                Chessman cm = piece.GetComponent<Chessman>();
                if(cm.player.Value == player.pColor)
                {
                    foreach (Ability ab in cm.variant.kit){
                        if(ab.name == track.name)
                        {
                            cm.queue.Add(ab);
                        }
                    }
                }
            }
            Debug.Log(track);
            AudioClip clip = Resources.Load<AudioClip>($"Sounds/{track}");
            controller.GetComponent<Game>().APloseServerRpc(player.pColor, track.AP);
            speaker.PlayOneShot(clip);
            Debug.Log($"{ability.name}/{ability.type}\nAP:{ability.AP}\n{ability.desc}");
        }
        else
        {
            AudioClip clip = Resources.Load<AudioClip>($"Sounds/Deny");
            speaker.PlayOneShot(clip);
            Debug.Log($"Deny");
        }
    }

    void Update()
    {
        SpriteRenderer sr = transform.Find("Barr").GetComponent<SpriteRenderer>();
        Color c = sr.color;

        float targetAlpha = bgon ? 0.709f : 0f;

        c.a = Mathf.MoveTowards(c.a, targetAlpha, Time.deltaTime / 0.5f);

        sr.color = c;
        if (Track != null)
        {
            Vector3 movement = Track.transform.position - lastPosition;
            float speed = movement.magnitude / Time.deltaTime;
            distanceMoved = Vector3.Distance(startPosition, Track.transform.position);
                if (speed >= swipeThreshold)
                {
                    swipeDirection = movement.normalized;
                    if (swipeDirection.y < 0)
                    {
                    speed = 0;
                        if (!activated)
                        {
                            if (distanceMoved >= requiredDistance)
                            {
                            distanceMoved = 0;
                            if (!Track.GetComponent<TrackCard>().sliding)
                            {
                                AudioClip clip = Resources.Load<AudioClip>($"Sounds/baby");
                                speaker.PlayOneShot(clip);
                            }
                            else
                            {
                                AudioClip clip = Resources.Load<AudioClip>($"Sounds/babylong");
                                speaker.PlayOneShot(clip, 0.6f);
                            }
                            AudioClip clipafter = Resources.Load<AudioClip>($"Sounds/scancard");
                            speaker.PlayOneShot(clipafter, 0.2f);
                            if (Track.GetComponent<TrackCard>().ability != null)
                                {
                                    ability = Track.GetComponent<TrackCard>().ability;
                                    queuetrack(ability);
                                }
                                else
                                {
                                    Debug.Log("Blank");
                                }
                                activated = true;
                                distanceMoved = 0;
                            }
                        }
                    }
                }
                lastPosition = Track.transform.position;
        }
    }
}
