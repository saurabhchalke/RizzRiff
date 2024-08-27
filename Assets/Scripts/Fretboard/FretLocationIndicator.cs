using UnityEngine;
using TMPro;

public class FretLocationIndicator : MonoBehaviour
{
    public TextMeshProUGUI fretNumberText;
    public GameObject outerRing;
    public GameObject innerCircle;

    private Renderer outerRingRenderer;
    private Material outerRingMaterial;

    private void Awake()
    {
        outerRingRenderer = outerRing.GetComponent<Renderer>();
        outerRingMaterial = new Material(outerRingRenderer.material);
        outerRingRenderer.material = outerRingMaterial;
        DisableEmission();
    }

    public void SetFretNumber(int fretNumber)
    {
        fretNumberText.text = fretNumber.ToString();
    }

    public void SetColor(Color color)
    {
        outerRingMaterial.color = color;
        outerRingMaterial.SetColor("_EmissionColor", color);
    }

    public void EnableEmission()
    {
        outerRingMaterial.EnableKeyword("_EMISSION");
    }

    public void DisableEmission()
    {
        outerRingMaterial.DisableKeyword("_EMISSION");
    }
}