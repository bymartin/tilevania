using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePersist : MonoBehaviour
{
    void Awake() {
        int numScenePersists = FindObjectsOfType<ScenePersist>().Length;
        if (numScenePersists > 1) {
            // we would create a new one after dying or going to a new
            // level, we want to keep using the 1st one created
            Destroy(gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ResetScenePersist() {
        Destroy(gameObject);
    }
}
