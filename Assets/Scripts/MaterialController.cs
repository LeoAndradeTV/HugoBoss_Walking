/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MaterialController : MonoBehaviour
{
    public Material redMat;
    public Material greenMat;
    public GameObject model;
    public SkinnedMeshRenderer skinnedMeshRenderer;

    //singleton instance of the class
    public static MaterialController instance;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        if (model != null)
        {
            var smr = model.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
            if (smr != null)
            {
                instance.skinnedMeshRenderer = smr;
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ColorChanger();

        }
    }
    public static bool isAnimating = false;
    public static void ColorChanger()
    {
        if (!isAnimating)
        {
            isAnimating = true;

            var currentMaterial = instance.skinnedMeshRenderer.sharedMaterial;
            if (currentMaterial == instance.redMat)
            {
                //instance.skinnedMeshRenderer.sharedMaterial = instance.greenMat;

                // rotate model clockwise by 15 degrees over 1 second
               // instance.model.transform.DORotate(new Vector3(0f, -15f, 0f), 1f, RotateMode.FastBeyond360);
               // instance.StartCoroutine(instance.ScaleModel(Vector3.one * 1.2f, 1f)); // scale up model by 20% over 1 second
            }
            else
            {
                //instance.skinnedMeshRenderer.sharedMaterial = instance.redMat;

                // rotate model clockwise by -15 degrees over 1 second
                //instance.model.transform.DORotate(new Vector3(0f, 15f, 0f), 1f, RotateMode.FastBeyond360);
               // instance.StartCoroutine(instance.ScaleModel(Vector3.one / 1.2f, 1f)); // scale down model by 20% over 1 second
            }
        }
    }

    private IEnumerator ScaleModel(Vector3 targetScale, float duration)
    {
        Vector3 startScale = instance.skinnedMeshRenderer.rootBone.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            instance.skinnedMeshRenderer.rootBone.localScale = Vector3.Lerp(startScale, targetScale, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        instance.skinnedMeshRenderer.rootBone.localScale = targetScale;
        isAnimating = false;
    }
}
*/