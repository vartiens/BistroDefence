using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ctrl : MonoBehaviour
{
    [SerializeField] private Material _material_dissolve;
    [SerializeField] private Material _material_phase;
    [SerializeField] private Material _material_original;
    [SerializeField] private float phaseTime = 1f;
    [SerializeField] private float dissolveTime = 1f;
    [SerializeField] private float height = 1f;
    [SerializeField] private float Hp = 5f;

    void Start()
    {
        StartCoroutine(do_phase());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(--Hp == 0)
            StartCoroutine(do_disssolve());
    } 

    void change_materials(Transform parent, Material _material)
    {
        foreach(Transform child in parent)
        {
            Renderer _renderer = child.GetComponent<Renderer>();
            _renderer.material = _material;
            change_materials(child, _material);
        }
    }

    IEnumerator do_phase()
    {
        change_materials(transform, _material_phase);

        float start_time = Time.time;
        float t, value;

        while (Time.time <= start_time + phaseTime)
        {
            t = (Time.time - start_time) / phaseTime;
            value = Mathf.Lerp(0, height, t);
            _material_phase.SetFloat("_Split_Value", value);
            yield return null;
        }

        change_materials(transform, _material_original);
    }

    IEnumerator do_disssolve()
    {
        change_materials(transform, _material_dissolve);

        float start_time = Time.time;
        float t, value;

        while (Time.time <= start_time + dissolveTime)
        {
            t = (Time.time - start_time) / dissolveTime;
            value = Mathf.Lerp(1.1f, 0f, t);
            _material_dissolve.SetFloat("_Split_Value", value);
            yield return null;
        }

        Destroy(transform.parent.gameObject);
    }
}



