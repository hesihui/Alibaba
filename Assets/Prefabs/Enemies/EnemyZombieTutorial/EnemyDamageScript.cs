using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyDamageScript : MonoBehaviour
{
    private List<Material[]> originalMaterials = new List<Material[]>();
    private List<Color[]> originalColors = new List<Color[]>();
    private Coroutine changeCoroutine;
    private Coroutine revertCoroutine;
    private List<Material> createdMaterials = new List<Material>();

    protected virtual void Start()
    {
        CollectOriginalMaterials();

    }


    private void CollectOriginalMaterials()
    {
        // Collect materials from MeshRenderers
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in meshRenderers)
        {
            originalMaterials.Add(renderer.materials);
            List<Color> colors = new List<Color>();
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].HasProperty("_Color"))
                {
                    colors.Add(renderer.materials[i].color);
                }
                else
                {
                    colors.Add(Color.black); // Dummy color for materials without _Color property
                }
            }
            originalColors.Add(colors.ToArray());
        }

        // Collect materials from SkinnedMeshRenderers
        SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
        {
            originalMaterials.Add(renderer.materials);
            List<Color> colors = new List<Color>();
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].HasProperty("_Color"))
                {
                    colors.Add(renderer.materials[i].color);
                }
                else
                {
                    colors.Add(Color.black); // Dummy color for materials without _Color property
                }
            }
            originalColors.Add(colors.ToArray());
        }
    }

    private void CleanUpCreatedMaterials()
    {
        foreach (var material in createdMaterials)
        {
            if (material != null)
            {
                Destroy(material);
            }
        }
        createdMaterials.Clear();
    }

    public void TriggerMaterialChange(Vector3 HitPosition = default)
    {
        if (changeCoroutine != null)
        {
            StopCoroutine(changeCoroutine);
            CleanUpCreatedMaterials();
        }

        if (revertCoroutine != null)
        {
            StopCoroutine(revertCoroutine);
        }
        changeCoroutine = StartCoroutine(ChangeMaterialsTemporarily(HitPosition));
    }

    private IEnumerator ChangeMaterialsTemporarily(Vector3 HitPosition)
    {
        ChangeMaterialsToColor(new Color(2, 0, 0), HitPosition); // Adjust color as needed
        yield return new WaitForSeconds(0.2f);
        revertCoroutine = StartCoroutine(RevertMaterialsSmoothly(.5f));
    }

    private void ChangeMaterialsToColor(Color color, Vector3 HitPosition)
    {
        // Change materials for MeshRenderers
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in meshRenderers)
        {
            Material[] newMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                // Create a new temporary material instance and track it
                newMaterials[i] = new Material(renderer.materials[i]);
                createdMaterials.Add(newMaterials[i]);

                if (newMaterials[i].HasProperty("_Color"))
                {
                    newMaterials[i].color = color; // Ensure color change
                }
            }
            renderer.materials = newMaterials;
        }

        // Change materials for SkinnedMeshRenderers
        SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
        {
            Material[] newMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                // Create a new temporary material instance and track it
                newMaterials[i] = new Material(renderer.materials[i]);
                createdMaterials.Add(newMaterials[i]);

                if (newMaterials[i].HasProperty("_Color"))
                {
                    newMaterials[i].color = color; // Ensure color change
                }
            }
            renderer.materials = newMaterials;
        }
    }

    private IEnumerator RevertMaterialsSmoothly(float duration)
    {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        // Collect current colors
        List<Color[]> currentColors = new List<Color[]>();
        foreach (MeshRenderer renderer in meshRenderers)
        {
            List<Color> colors = new List<Color>();
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].HasProperty("_Color"))
                {
                    colors.Add(renderer.materials[i].color);
                }
                else
                {
                    colors.Add(Color.black); // Dummy color for materials without _Color property
                }
            }
            currentColors.Add(colors.ToArray());
        }

        foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
        {
            List<Color> colors = new List<Color>();
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].HasProperty("_Color"))
                {
                    colors.Add(renderer.materials[i].color);
                }
                else
                {
                    colors.Add(Color.black); // Dummy color for materials without _Color property
                }
            }
            currentColors.Add(colors.ToArray());
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Update colors for MeshRenderers
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                for (int j = 0; j < meshRenderers[i].materials.Length; j++)
                {
                    if (meshRenderers[i].materials[j].HasProperty("_Color"))
                    {
                            meshRenderers[i].materials[j].color = Color.Lerp(new Color(2,0,0), originalColors[i][j], t);
                    }
                }
            }

            // Update colors for SkinnedMeshRenderers
            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                for (int j = 0; j < skinnedMeshRenderers[i].materials.Length; j++)
                {
                    if (skinnedMeshRenderers[i].materials[j].HasProperty("_Color"))
                    {
                        skinnedMeshRenderers[i].materials[j].color = Color.Lerp(new Color(2, 0, 0), originalColors[meshRenderers.Length + i][j], t);
                    }
                }
            }

            yield return null;
        }

        // Reassign original materials for MeshRenderers
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].materials = originalMaterials[i];
        }

        // Reassign original materials for SkinnedMeshRenderers
        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {
            skinnedMeshRenderers[i].materials = originalMaterials[meshRenderers.Length + i];
        }

        // Destroy the temporary materials after reverting
        CleanUpCreatedMaterials();
    }
}
