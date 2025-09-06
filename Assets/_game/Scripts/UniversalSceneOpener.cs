using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UniversalSceneOpener : MonoBehaviour
{
    [Header("Scene Options")]
    [Tooltip("Name of the scene to load (must be added in Build Settings).")]
    [SerializeField] private string sceneName;

    [Tooltip("If true, loads the scene by build index instead of name.")]
    [SerializeField] private bool useBuildIndex = false;

    [Tooltip("Build index to load (only used if Use Build Index is checked).")]
    [SerializeField] private int sceneBuildIndex = 0;

    [Header("Loading Settings")]
    [SerializeField] private bool loadAdditively = false;

    private Button button;

    private void Awake()
    {
        // Ensure there’s a Button component
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("UniversalSceneOpener requires a Button component on the same GameObject.");
            return;
        }

        // Hook automatically
        button.onClick.AddListener(OpenScene);
    }

    public void OpenScene()
    {
        if (useBuildIndex)
        {
            if (loadAdditively)
                SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Additive);
            else
                SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Single);
        }
        else
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("UniversalSceneOpener: No scene name set!");
                return;
            }

            if (loadAdditively)
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            else
                SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}
