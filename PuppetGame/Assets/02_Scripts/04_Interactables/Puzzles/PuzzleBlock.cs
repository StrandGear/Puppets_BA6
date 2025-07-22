using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PuzzleBlock : MonoBehaviour
{
    [SerializeField] Transform m_checkPoint;
    [SerializeField] List<ResetConfiguration> m_puzzlesToReset = new List<ResetConfiguration>();

    //private List<GameObject> m_objectsToDuplicate = new List<GameObject>();

    private void Start()
    {

        if (!m_checkPoint)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                m_checkPoint = transform.GetChild(i).CompareTag("Checkpoint") ? transform.GetChild(i).transform : null;
            }
        }    

        var allPuzzles = GetComponentsInChildren<Puzzle>(includeInactive: false);
        foreach (var puzzle in allPuzzles)
        {
            ResetConfiguration newResetConfig = new ResetConfiguration(puzzle.gameObject);
            m_puzzlesToReset.Add(newResetConfig);
        }


        foreach (ResetConfiguration elem in m_puzzlesToReset)
        {
            elem.startPosition = elem.ObjectToReset.transform.position;
            elem.currentInstance = elem.ObjectToReset;

            if (elem.WayToReset == WayToReset.Respawn)
            {
                // create hidden copy (keeps scale, rotation, all settings)
                elem.originalPrefab = Instantiate(elem.ObjectToReset, transform);
                elem.originalPrefab.SetActive(false);
            }
        }

    }


    public void ResetPuzzle()
    {
        foreach (ResetConfiguration elem in m_puzzlesToReset)
        {
            if (elem.WayToReset == WayToReset.Reactivate)
            {
                elem.currentInstance.SetActive(false);
                elem.currentInstance.SetActive(true);
            }
            else if (elem.WayToReset == WayToReset.Respawn)
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
    public GameObject ObjectToReset;      // initial object in scene
    public WayToReset WayToReset;

    [HideInInspector] public Vector3 startPosition; // original position
    [HideInInspector] public GameObject originalPrefab; // prefab to clone
    [HideInInspector] public GameObject currentInstance; // current active instance

    public ResetConfiguration(GameObject objectToReset)
    {
        ObjectToReset = objectToReset;
        WayToReset = WayToReset.Respawn;
    }
    public ResetConfiguration()
    {
        WayToReset = WayToReset.Respawn;
    }
    public ResetConfiguration(GameObject objectToReset, WayToReset wayToReset)
    {
        ObjectToReset = objectToReset;
        WayToReset = wayToReset;
    }
}
