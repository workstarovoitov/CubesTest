using System.Collections;
using UnityEngine;
using System;

public class PrefabController : MonoBehaviour
{
    [Header("Lifetime")]
    private float lifetime = 10f;
    public float Lifetime
    {
        get => lifetime;
        set => lifetime = value > 0 ? value : 10f;
    }
    [SerializeField] [Range(1f, 10f)] private float lifetimeMin = 1f;
    [SerializeField] [Range(1f, 10f)] private float lifetimeMax = 10f;
    [SerializeField] private float fadeSpeed = 10f;


    [Header("Position")]
    [SerializeField] [Range(1f, 10f)] private float scaleMin = 1f;
    [SerializeField] [Range(1f, 10f)] private float scaleMax = 10f;
   
    [SerializeField] private Vector3 angularSpeedMin = Vector3.zero;
    [SerializeField] private Vector3 angularSpeedMax = Vector3.one;

    [Header("Color")]
    [SerializeField] [Range(0f, 1f)] private float hueMin = 0;
    [SerializeField] [Range(0f, 1f)] private float hueMax = 1;
    [SerializeField] [Range(0f, 1f)] private float saturationMin = 1f;
    [SerializeField] [Range(0f, 1f)] private float saturationMax = 1f;
    [SerializeField] [Range(0f, 1f)] private float brightnessMin = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float brightnessMax = 1f;


    private Action<PrefabController> OnObjectDestroy;
    public void Init(Action<PrefabController> onObjectDestroy)
    {
        OnObjectDestroy = onObjectDestroy;
    }

    private void Start()
    {
        SetConditions();
    }
    public void SetConditions()
    {
        //set scale
        float scale = UnityEngine.Random.Range(scaleMin, scaleMax);
        transform.localScale = new Vector3(scale / transform.parent.localScale.x, scale / transform.parent.localScale.y, scale / transform.parent.localScale.z);
        //set angular speed
        GetComponentInChildren<Rigidbody>().angularVelocity = new Vector3(UnityEngine.Random.Range(angularSpeedMin.x, angularSpeedMax.x),
                                                                UnityEngine.Random.Range(angularSpeedMin.y, angularSpeedMax.y),
                                                                UnityEngine.Random.Range(angularSpeedMin.z, angularSpeedMax.z));
        //set color
        GetComponentInChildren<Renderer>().material.color = UnityEngine.Random.ColorHSV(hueMin, hueMax, saturationMin, saturationMax, brightnessMin, brightnessMax, 1f, 1f);
        //set lifetime
        lifetime = UnityEngine.Random.Range(lifetimeMin, lifetimeMax);
        StartCoroutine(DestroyPrefab());

    }

    IEnumerator DestroyPrefab()
    {
        yield return new WaitForSeconds(lifetime);
        Color alpha = GetComponentInChildren<Renderer>().material.color;
        float alphaTarget = 0.25f;
        //fadeout
        while (Mathf.Abs(alpha.a - alphaTarget) > .01f)
        {
            alpha.a = Mathf.Lerp(alpha.a, alphaTarget, Time.deltaTime * fadeSpeed);
            GetComponentInChildren<Renderer>().material.color = alpha;
            yield return new WaitForFixedUpdate();
        }
        OnObjectDestroy(this);
    }
}
