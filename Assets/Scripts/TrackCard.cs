using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class TrackCard : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent<TrackCard> BeginDragEvent;
    public UnityEvent<TrackCard> EndDragEvent;
    public Image image;
    public bool isDragging = false;
    public bool isDropped = false;
    public Ability ability = new KnightKick();
    public RectTransform rectTransform;
    public Quaternion targetRotation;
    public float tiltStrength = 2f;
    public float smoothSpeed = 5f;
    public float maxTilt = 20f;
    public bool isOn = false;
    public float tiltX;
    public float tiltY;
    public bool snapped = false;
    public bool sliding = false;
    public GameObject infobox;
    public Player player;
    public Game controller;
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
    }
    void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (player.pColor == controller.currentPlayer.Value)
        {
            isDragging = true;
            BeginDragEvent?.Invoke(this);
            image.raycastTarget = false;
            transform.SetAsLastSibling();
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        EndDragEvent?.Invoke(this);
        image.raycastTarget = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        
            RectTransform rectTransform = GetComponent<RectTransform>();
            isDragging = true;

            targetRotation = Quaternion.Euler(0, 0, 0f);

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out Vector3 globalMousePos))
            {
                rectTransform.position = globalMousePos;
            }

            if (snapped)
            {
                image.sprite = Resources.Load<Sprite>($"Cards/{ability.name}");
                Vector3 pos = rectTransform.localPosition;
                float poo = 13.156358f - transform.parent.GetComponent<RectTransform>().localPosition.x;
                Debug.Log($"{transform.parent.GetComponent<RectTransform>().localPosition.x}/{poo}");
                pos.x = poo;
                Debug.Log(pos.x);
                pos.z = 0.06f;
                rectTransform.localPosition = pos;
            }
            else
            {
                Vector3 pos = rectTransform.localPosition;
                pos.z = 0f;
                rectTransform.localPosition = pos;
            }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOn = true;
        AudioClip clip = Resources.Load<AudioClip>("Sounds/cardputback");
        transform.parent.parent.parent.parent.GetComponent<AudioSource>().PlayOneShot(clip, 0.7f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOn = false;
    }

    private void OnMouseUp()
    {
        GameObject[] plates = GameObject.FindGameObjectsWithTag("infobox");
        foreach (GameObject plate in plates)
        {
            if (plate.GetComponent<NetworkObject>().IsSpawned)
            {
                plate.GetComponent<NetworkObject>().Despawn();
            }
            Destroy(plate);
        }
        GameObject info = Instantiate(infobox, transform.parent.parent.parent.parent.parent);
        info.transform.Find("Blank").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Cards/{ability.name}");
        info.transform.Find("color/Title").GetComponent<TMP_Text>().text = ability.type;
        info.transform.Find("Name").GetComponent<TMP_Text>().text = ability.name;
        info.transform.Find("Scroll View/Viewport/Content/Text (TMP)").GetComponent<TMP_Text>().text = ability.desc;
        info.transform.Find("Cost").GetComponent<TMP_Text>().text = ability.AP.ToString(); if (ability.type == "premove")
        {
            info.transform.Find("color").GetComponent<Image>().color = Color.yellow;
        }
        else if (ability.type == "postmove")
        {
            info.transform.Find("color").GetComponent<Image>().color = new Color(0f, 1f, 117f / 255f, 1f);
        }
        StartCoroutine(DestroyInfoboxAfterTime(info, 3f));
    }

    // Coroutine to destroy the infobox after a delay
    private System.Collections.IEnumerator DestroyInfoboxAfterTime(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
        {
            if (obj.GetComponent<NetworkObject>() != null && obj.GetComponent<NetworkObject>().IsSpawned)
            {
                obj.GetComponent<NetworkObject>().Despawn();
            }
            Destroy(obj);
        }
    }

    void Update()
    {
        image.sprite = Resources.Load<Sprite>($"Cards/{ability.name}");
        rectTransform = GetComponent<RectTransform>();
        if (!isDragging)
        {
            if (sliding)
            {
                rectTransform.localPosition = Vector3.Lerp(
                rectTransform.localPosition,
                new Vector3(rectTransform.localPosition.x, -24.02f, 0.06f),
                7f * Time.deltaTime
                );
                if(rectTransform.localPosition.y <= -23.02f) sliding = false;
            }
            else
            {
                if (isOn)
                {
                    RectTransform parentrect = transform.parent.GetComponent<RectTransform>();
                    Vector2 pos = parentrect.localPosition;
                    rectTransform.localPosition = Vector2.Lerp(
                        rectTransform.localPosition,
                        new Vector2(0f, pos.y+0.3f),
                        30f * Time.deltaTime
                    );
                    // Get mouse position in screen space
                    Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

                    // Get card center in screen space
                    Vector3 cardScreenPos = Camera.main.WorldToScreenPoint(transform.position);

                    // Offset from card center to mouse (in screen space)
                    Vector2 offset = new Vector2(cardScreenPos.x, cardScreenPos.y) - mouseScreenPos;

                    // Normalize to [-1, 1] based on a fixed screen size or card size
                    Vector2 normalizedOffset = new Vector2(
                        Mathf.Clamp(offset.x / 100f, -1f, 1f),
                        Mathf.Clamp(offset.y / 100f, -1f, 1f)
                    );

                    // Calculate tilt angles
                    tiltX = Mathf.Clamp(offset.y * maxTilt, -maxTilt, maxTilt);
                    tiltY = Mathf.Clamp(-offset.x * maxTilt, -maxTilt, maxTilt);

                    // Smoothly rotate the card toward the mouse
                    targetRotation = Quaternion.Euler(tiltX, tiltY, 0f);
                }
                else
                {
                    targetRotation = Quaternion.Euler(0, 0, 0f);
                    rectTransform.localPosition = Vector3.Lerp(
                        rectTransform.localPosition,
                        new Vector3(0f, 0f, 0f),
                        30f * Time.deltaTime
                    );
                    targetRotation = Quaternion.Euler(0, 0, 0f);
                }
            }
        }
    }
}
