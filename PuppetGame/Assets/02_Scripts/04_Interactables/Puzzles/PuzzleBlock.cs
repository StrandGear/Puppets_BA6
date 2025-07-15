using System.Collections.Generic;
using UnityEngine;

public class PuzzleBlock : MonoBehaviour
{
    [SerializeField] Transform m_checkPoint;
    [SerializeField] List<ResetConfiguration> m_puzzlesToReset;

    //private List<GameObject> m_objectsToDuplicate = new List<GameObject>();

    private void Start()
    {
        foreach (ResetConfiguration elem in m_puzzlesToReset)
        {
            elem.startPosition = elem.objectToReset.transform.position;
            elem.currentInstance = elem.objectToReset;

            if (elem.wayToReset == WayToReset.Respawn)
            {
                // create hidden copy (keeps scale, rotation, all settings)
                elem.originalPrefab = Instantiate(elem.objectToReset, transform);
                elem.originalPrefab.SetActive(false);
            }
        }

        if (!m_checkPoint)
            m_checkPoint = transform.Find("Checkpoint");
    }


    public void ResetPuzzle()
    {
        foreach (ResetConfiguration elem in m_puzzlesToReset)
        {
            if (elem.wayToReset == WayToReset.Reactivate)
            {
                elem.currentInstance.SetActive(false);
                elem.currentInstance.SetActive(true);
            }
            else if (elem.wayToReset == WayToReset.Respawn)
            {
                if (elem.currentInstance != null)
                    Destroy(elem.currentInstance);

                elem.currentInstance = Instantiate(elem.originalPrefab, elem.startPosition, Quaternion.identity, transform);
                elem.currentInstance.SetActive(true);
            }
        }
    }
}

public enum WayToReset
{
    Reactivate,
    Respawn
}

[System.Serializable]
public class ResetConfiguration
{
    public GameObject objectToReset;      // initial object in scene
    public WayToReset wayToReset;

    [HideInInspector] public Vector3 startPosition; // original position
    [HideInInspector] public GameObject originalPrefab; // prefab to clone
    [HideInInspector] public GameObject currentInstance; // current active instance
}
