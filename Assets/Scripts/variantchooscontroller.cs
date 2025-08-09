using UnityEngine;
using UnityEngine.UI;

public class variantchooscontroller : MonoBehaviour
{
    public GameObject row1col1;
    public GameObject row1col2;
    public GameObject row1col3;

    public GameObject row2col1;
    public GameObject row2col2;
    public GameObject row2col3;

    public GameObject row3col1;
    public GameObject row3col2;
    public GameObject row3col3;

    public GameObject pawnButton;
    public GameObject knightButton;
    public GameObject bishopButton;
    public GameObject rookButton;
    public GameObject queenButton;
    public GameObject kingButton;

    public Army draft;

    public GameObject variantprefab;

    void Start()
    {
        // Add listeners for each button's OnClick event
        pawnButton.GetComponent<Button>().onClick.AddListener(OnPawnClicked);
        knightButton.GetComponent<Button>().onClick.AddListener(OnKnightClicked);
        bishopButton.GetComponent<Button>().onClick.AddListener(OnBishopClicked);
        rookButton.GetComponent<Button>().onClick.AddListener(OnRookClicked);
        queenButton.GetComponent<Button>().onClick.AddListener(OnQueenClicked);
        kingButton.GetComponent<Button>().onClick.AddListener(OnKingClicked);
    }

    // Button click handler functions
    void OnPawnClicked()
    {
        Image sr = variantprefab.GetComponent<Image>();
        if (sr != null)
        {
            sr.sprite = pawnButton.GetComponent<Image>().sprite;
            row1col1.GetComponent<Image>().sprite = sr.sprite;
            row1col2.GetComponent<Image>().sprite = sr.sprite;
            row1col3.GetComponent<Image>().sprite = sr.sprite;

            row2col1.GetComponent<Image>().sprite = sr.sprite;
            row2col2.GetComponent<Image>().sprite = sr.sprite;
            row2col3.GetComponent<Image>().sprite = sr.sprite;

            row3col1.GetComponent<Image>().sprite = sr.sprite;
            row3col2.GetComponent<Image>().sprite = sr.sprite;
            row3col3.GetComponent<Image>().sprite = sr.sprite;
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on target GameObject.");
        }
    }

    void OnKnightClicked()
    {
        Image sr = variantprefab.GetComponent<Image>();
        if (sr != null)
        {
            sr.sprite = pawnButton.GetComponent<Image>().sprite;
            row1col1.GetComponent<Image>().sprite = sr.sprite;
            row1col2.GetComponent<Image>().sprite = sr.sprite;
            row1col3.GetComponent<Image>().sprite = sr.sprite;

            row2col1.GetComponent<Image>().sprite = sr.sprite;
            row2col2.GetComponent<Image>().sprite = sr.sprite;
            row2col3.GetComponent<Image>().sprite = sr.sprite;

            row3col1.GetComponent<Image>().sprite = sr.sprite;
            row3col2.GetComponent<Image>().sprite = sr.sprite;
            row3col3.GetComponent<Image>().sprite = sr.sprite;
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on target GameObject.");
        }
    }

    void OnBishopClicked()
    {
        Image sr = variantprefab.GetComponent<Image>();
        if (sr != null)
        {
            sr.sprite = bishopButton.GetComponent<Image>().sprite;
            row1col1.GetComponent<Image>().sprite = sr.sprite;
            row1col2.GetComponent<Image>().sprite = sr.sprite;
            row1col3.GetComponent<Image>().sprite = sr.sprite;

            row2col1.GetComponent<Image>().sprite = sr.sprite;
            row2col2.GetComponent<Image>().sprite = sr.sprite;
            row2col3.GetComponent<Image>().sprite = sr.sprite;

            row3col1.GetComponent<Image>().sprite = sr.sprite;
            row3col2.GetComponent<Image>().sprite = sr.sprite;
            row3col3.GetComponent<Image>().sprite = sr.sprite;
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on target GameObject.");
        }
    }

    void OnRookClicked()
    {
        Image sr = variantprefab.GetComponent<Image>();
        if (sr != null)
        {
            sr.sprite = rookButton.GetComponent<Image>().sprite;
            row1col1.GetComponent<Image>().sprite = sr.sprite;
            row1col2.GetComponent<Image>().sprite = sr.sprite;
            row1col3.GetComponent<Image>().sprite = sr.sprite;

            row2col1.GetComponent<Image>().sprite = sr.sprite;
            row2col2.GetComponent<Image>().sprite = sr.sprite;
            row2col3.GetComponent<Image>().sprite = sr.sprite;

            row3col1.GetComponent<Image>().sprite = sr.sprite;
            row3col2.GetComponent<Image>().sprite = sr.sprite;
            row3col3.GetComponent<Image>().sprite = sr.sprite;
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on target GameObject.");
        }
    }

    void OnQueenClicked()
    {
        Image sr = variantprefab.GetComponent<Image>();
        if (sr != null)
        {
            sr.sprite = queenButton.GetComponent<Image>().sprite;
            row1col1.GetComponent<Image>().sprite = sr.sprite;
            row1col2.GetComponent<Image>().sprite = sr.sprite;
            row1col3.GetComponent<Image>().sprite = sr.sprite;

            row2col1.GetComponent<Image>().sprite = sr.sprite;
            row2col2.GetComponent<Image>().sprite = sr.sprite;
            row2col3.GetComponent<Image>().sprite = sr.sprite;

            row3col1.GetComponent<Image>().sprite = sr.sprite;
            row3col2.GetComponent<Image>().sprite = sr.sprite;
            row3col3.GetComponent<Image>().sprite = sr.sprite;
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on target GameObject.");
        }
    }

    void OnKingClicked()
    {
        Image sr = variantprefab.GetComponent<Image>();
        if (sr != null)
        {
            sr.sprite = kingButton.GetComponent<Image>().sprite;
            row1col1.GetComponent<Image>().sprite = sr.sprite;
            row1col2.GetComponent<Image>().sprite = sr.sprite;
            row1col3.GetComponent<Image>().sprite = sr.sprite;

            row2col1.GetComponent<Image>().sprite = sr.sprite;
            row2col2.GetComponent<Image>().sprite = sr.sprite;
            row2col3.GetComponent<Image>().sprite = sr.sprite;

            row3col1.GetComponent<Image>().sprite = sr.sprite;
            row3col2.GetComponent<Image>().sprite = sr.sprite;
            row3col3.GetComponent<Image>().sprite = sr.sprite;
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on target GameObject.");
        }
    }

    void Update()
    {

    }
}
