using UnityEngine;
using UnityEditor;

public class AutoColliderTool : EditorWindow
{
    private enum ColliderMode { SmartDetect, Box, Sphere, Capsule, Mesh }
    private ColliderMode defaultMode = ColliderMode.SmartDetect;
    private bool replaceExisting = false;
    private bool useConvexMesh = true;

    [MenuItem("Tools/Auto Collider Generator")]
    public static void ShowWindow()
    {
        GetWindow<AutoColliderTool>("Auto Colliders");
    }

    void OnGUI()
    {
        GUILayout.Label("⚡ Auto Collider Generator", EditorStyles.boldLabel);

        defaultMode = (ColliderMode)EditorGUILayout.EnumPopup("Default Collider Type", defaultMode);
        replaceExisting = EditorGUILayout.Toggle("Replace Existing Colliders", replaceExisting);
        useConvexMesh = EditorGUILayout.Toggle("Convex Mesh (Mobile Friendly)", useConvexMesh);

        GUILayout.Space(10);

        if (GUILayout.Button("Auto-Add Colliders (Selected Objects)"))
        {
            AddCollidersToSelection();
        }

        if (GUILayout.Button("Auto-Add Colliders (Whole Scene)"))
        {
            AddCollidersToScene();
        }
    }

    private void AddCollidersToSelection()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            AddCollidersRecursive(obj);
        }
    }

    private void AddCollidersToScene()
    {
        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (obj.GetComponent<MeshRenderer>() != null || obj.GetComponent<SkinnedMeshRenderer>() != null)
            {
                AddCollidersRecursive(obj);
            }
        }
    }

    private void AddCollidersRecursive(GameObject obj)
    {
        // If object has a visible mesh → give collider
        if (obj.GetComponent<MeshRenderer>() || obj.GetComponent<SkinnedMeshRenderer>())
        {
            AddCollider(obj);
        }

        // Recursively go through children
        foreach (Transform child in obj.transform)
        {
            AddCollidersRecursive(child.gameObject);
        }
    }

    private void AddCollider(GameObject obj)
    {
        if (!replaceExisting && obj.GetComponent<Collider>() != null)
            return;

        if (replaceExisting)
        {
            foreach (Collider c in obj.GetComponents<Collider>())
            {
                DestroyImmediate(c);
            }
        }

        switch (defaultMode)
        {
            case ColliderMode.SmartDetect:
                SmartDetect(obj);
                break;
            case ColliderMode.Box:
                obj.AddComponent<BoxCollider>();
                break;
            case ColliderMode.Sphere:
                obj.AddComponent<SphereCollider>();
                break;
            case ColliderMode.Capsule:
                obj.AddComponent<CapsuleCollider>();
                break;
            case ColliderMode.Mesh:
                MeshCollider mesh = obj.AddComponent<MeshCollider>();
                mesh.convex = useConvexMesh;
                break;
        }
    }

    private void SmartDetect(GameObject obj)
    {
        MeshFilter mf = obj.GetComponent<MeshFilter>();
        SkinnedMeshRenderer smr = obj.GetComponent<SkinnedMeshRenderer>();

        if (mf == null && smr == null)
            return;

        Mesh mesh = (mf != null) ? mf.sharedMesh : smr.sharedMesh;
        if (mesh == null) return;

        Bounds b = mesh.bounds;
        Vector3 size = b.size;
        float avgSize = (size.x + size.y + size.z) / 3f;

        if (Mathf.Approximately(size.x, size.y) && Mathf.Approximately(size.y, size.z))
        {
            obj.AddComponent<SphereCollider>();
        }
        else if (size.y > size.x * 2f && size.y > size.z * 2f)
        {
            obj.AddComponent<CapsuleCollider>();
        }
        else if (size.x > avgSize * 1.5f && size.z > avgSize * 1.5f)
        {
            obj.AddComponent<BoxCollider>();
        }
        else
        {
            MeshCollider mc = obj.AddComponent<MeshCollider>();
            mc.convex = useConvexMesh;
        }
    }
}
