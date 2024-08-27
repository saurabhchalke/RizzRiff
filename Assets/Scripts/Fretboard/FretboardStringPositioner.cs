using UnityEngine;

public class FretboardStringPositioner : MonoBehaviour
{
    public GameObject eString; // e (high E string)
    public GameObject BString; // B string
    public GameObject GString; // G string
    public GameObject DString; // D string
    public GameObject AString; // A string
    public GameObject EString; // E (low E string)

    public GameObject topLeftPoint; // Top-left corner GameObject
    public GameObject bottomLeftPoint; // Bottom-left corner GameObject
    [Range(0, 1)]
    public float marginPercentage = 0.12f; // Margin percentage

    void Start()
    {
        AdjustStrings();
    }

    void AdjustStrings()
    {
        // Get the Z positions of the top left and bottom left points
        float topLeftZ = topLeftPoint.transform.position.z;
        float bottomLeftZ = bottomLeftPoint.transform.position.z;

        // Calculate the total distance along the Z-axis between the top and bottom points
        float totalZDistance = bottomLeftZ - topLeftZ;

        // Calculate the margin
        float margin = totalZDistance * marginPercentage;

        // Calculate the usable Z distance after applying the margin
        float usableZDistance = totalZDistance - (2 * margin);

        // Calculate the spacing between each string along the Z-axis
        float spacing = usableZDistance / 5f; // 5 intervals between 6 strings

        // Adjust the position of each string
        eString.transform.position = new Vector3(eString.transform.position.x, eString.transform.position.y, topLeftZ + margin + (0 * spacing));
        BString.transform.position = new Vector3(BString.transform.position.x, BString.transform.position.y, topLeftZ + margin + (1 * spacing));
        GString.transform.position = new Vector3(GString.transform.position.x, GString.transform.position.y, topLeftZ + margin + (2 * spacing));
        DString.transform.position = new Vector3(DString.transform.position.x, DString.transform.position.y, topLeftZ + margin + (3 * spacing));
        AString.transform.position = new Vector3(AString.transform.position.x, AString.transform.position.y, topLeftZ + margin + (4 * spacing));
        EString.transform.position = new Vector3(EString.transform.position.x, EString.transform.position.y, topLeftZ + margin + (5 * spacing));
    }
}