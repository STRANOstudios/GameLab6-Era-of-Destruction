using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    void Awake()
    {
        ResetPlayerPrefsIfNecessary();
    }

    void ResetPlayerPrefsIfNecessary()
    {
        // Check if PlayerPrefs need to be reset (e.g., first run, debug reset)
        bool resetPlayerPrefs = true;

        if (resetPlayerPrefs)
        {
            PlayerPrefs.DeleteAll(); // Reset all PlayerPrefs
            PlayerPrefs.Save(); // Save changes (optional)
        }
    }
}
