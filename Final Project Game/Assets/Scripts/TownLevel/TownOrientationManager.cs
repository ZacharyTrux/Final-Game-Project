using UnityEngine;

public class TownOrientationManager : MonoBehaviour
{
    [Header("Town Goal")]
    public int totalHousesToFix = 4;
    private int fixedHouses = 0;

    [Header("Exit")]
    public GameObject lockedPortal;
    public GameObject openPortal;

    private bool townFixed = false;

    private void Start()
    {
        if (lockedPortal != null)
            lockedPortal.SetActive(true);

        if (openPortal != null)
            openPortal.SetActive(false);
    }

    public void HouseFixed()
    {
        if (townFixed)
            return;

        fixedHouses++;

        Debug.Log("Fixed houses: " + fixedHouses + "/" + totalHousesToFix);

        if (fixedHouses >= totalHousesToFix)
        {
            FixTown();
        }
    }

    private void FixTown()
    {
        townFixed = true;

        if (lockedPortal != null)
            lockedPortal.SetActive(false);

        if (openPortal != null)
            openPortal.SetActive(true);

        Debug.Log("The town orientation is fixed. Portal opened.");
    }
}