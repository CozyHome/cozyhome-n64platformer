using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleWriter : MonoBehaviour
{
    [SerializeField] private MeshRenderer Renderer;
    [SerializeField] private Transform PointLight0;
    [SerializeField] private Transform PointLight1;

    private Vector3 LastPosition0, LastPosition1;

    void Update()
    {
        UpdatePoint("_PointLight0", PointLight0, ref LastPosition0);
        UpdatePoint("_PointLight1", PointLight1, ref LastPosition1);
    }

    void UpdatePoint(string name, Transform point, ref Vector3 LastPosition)
    {
        if ((point.position - LastPosition).sqrMagnitude > 0.05F)
        {
            LastPosition = point.position;
            //upd
            Material mat = Renderer.material;
            mat.SetVector(name, transform.position - point.position);
            Renderer.material = mat;
        }
    }
}
