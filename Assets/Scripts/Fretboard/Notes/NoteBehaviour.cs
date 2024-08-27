using UnityEngine;
using System.Collections;

public class NoteBehaviour : MonoBehaviour
{
    public float fallSpeed = 10f;
    public float fadeOutDuration = 0.3f;

    public Material correctNoteMaterial;
    public Material incorrectNoteMaterial;
    private Renderer noteRenderer;
    public bool isPlayed = false;
    private Material originalMaterial;
    private bool isMissed = false;

    private FretLocationIndicator fretIndicator;
    private NoteResultDisplay resultDisplay;

    private void Start()
    {
        noteRenderer = GetComponent<Renderer>();
        if (noteRenderer != null && noteRenderer.material != null)
        {
            originalMaterial = new Material(noteRenderer.material);
            DisableEmission();
        }
        else
        {
            Debug.LogWarning("Renderer or material is null on NoteBehaviour Start");
        }

        resultDisplay = FindObjectOfType<NoteResultDisplay>();
        if (resultDisplay == null)
        {
            Debug.LogWarning("NoteResultDisplay not found in the scene");
        }
    }

    private void Update()
    {
        if (isMissed)
        {
            NoteMover noteMover = GetComponent<NoteMover>();
            if (noteMover != null && transform.position.x < noteMover.endPoint.x)
            {
                StartCoroutine(DestroyNoteCoroutine());
            }
        }
    }

    public void PlayNote(bool isCorrect)
    {
        if (!isPlayed && noteRenderer != null)
        {
            isPlayed = true;
            if (isCorrect && correctNoteMaterial != null)
            {
                noteRenderer.material = new Material(correctNoteMaterial);
                StartCoroutine(FadeOutNote());
            }
            else if (!isCorrect && incorrectNoteMaterial != null)
            {
                noteRenderer.material = new Material(incorrectNoteMaterial);
                isMissed = true;
            }

            if (resultDisplay != null)
            {
                resultDisplay.DisplayResult(isCorrect);
            }
        }
    }

    public void MissNote()
    {
        if (!isPlayed && noteRenderer != null && incorrectNoteMaterial != null)
        {
            isPlayed = true;
            isMissed = true;
            noteRenderer.material = new Material(incorrectNoteMaterial);

            if (resultDisplay != null)
            {
                resultDisplay.DisplayResult(false);
            }
        }
    }

    public void SetFretLocationIndicator(FretLocationIndicator indicator)
    {
        fretIndicator = indicator;
    }

    public void EnableEmission()
    {
        if (noteRenderer != null && noteRenderer.material != null)
        {
            noteRenderer.material.EnableKeyword("_EMISSION");
        }
        if (fretIndicator != null)
        {
            fretIndicator.EnableEmission();
        }
    }

    public void DisableEmission()
    {
        if (noteRenderer != null && noteRenderer.material != null)
        {
            noteRenderer.material.DisableKeyword("_EMISSION");
        }
        if (fretIndicator != null)
        {
            fretIndicator.DisableEmission();
        }
    }

    private IEnumerator FadeOutNote()
    {
        if (noteRenderer == null || noteRenderer.material == null) yield break;

        float elapsedTime = 0f;
        Color startColor = noteRenderer.material.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            noteRenderer.material.color = Color.Lerp(startColor, endColor, alpha);
            if (fretIndicator != null)
            {
                fretIndicator.SetColor(Color.Lerp(startColor, endColor, alpha));
            }
            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator DestroyNoteCoroutine()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = true;
        rb.velocity = Vector3.down * fallSpeed;

        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}