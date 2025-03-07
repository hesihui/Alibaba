using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ChestScript : MonoBehaviour
{
    public VisualEffect Stars;
    public Color flashColor = Color.yellow;
    public float flashDuration = 0.1f;
    public float fadeDuration = 2f;

    private Renderer[] renderers;
    private Material[] materials;
    

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        List<Material> materialList = new List<Material>();
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] currentMaterials = renderers[i].materials;
            materialList.AddRange(currentMaterials);
        }
        materials = materialList.ToArray();
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i].HasProperty("_ZTest"))
            {
                materials[i].SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
            }
        }

    }

    public IEnumerator FlashAndFadeCoroutine()
    {
        SetObjectColor(flashColor);
        yield return new WaitForSeconds(flashDuration);
        float elapsedTime = 0f;
        while (elapsedTime <= fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            SetObjectAlpha(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetObjectAlpha(0f);
        
        GameObject.Destroy(gameObject);
    }

    private void SetObjectColor(Color color)
    {
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i].HasProperty("_BaseColor"))
            {
                materials[i].SetColor("_BaseColor", color);
            }
            else if (materials[i].HasProperty("_Color"))
            {
                materials[i].SetColor("_Color", color);
            }
        }
    }

    private void SetObjectAlpha(float alpha)
    {
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i].HasProperty("_Alpha"))
            {
                materials[i].SetFloat("_Alpha", alpha);
                materials[i].SetFloat("_Reflective", 1);
                materials[i].SetFloat("_fresnelstrength", 0.1f);


            }
            else if (materials[i].HasProperty("_OutlineWidth"))
            {
                materials[i].SetFloat("_OutlineWidth", 0);
            }
           
        }
    }


    public IEnumerator MoveObjectUpwards(Transform objectTransform, float targetHeight, float duration)
    {
        Vector3 startPosition = objectTransform.position;
        Vector3 endPosition = startPosition + new Vector3(0, targetHeight, 0);
        Quaternion startRotation = objectTransform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(-120, 0, 0); 
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            objectTransform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            objectTransform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        objectTransform.position = endPosition;
        objectTransform.rotation = endRotation;
    }
}
