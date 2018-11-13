using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KlimaErlebnis.InputSystem;
using UnityEngine;

public class HoverableObject : MonoBehaviour, IHoverable
{
    public RenderTexture TargetTexture;

    private Mesh[] _meshes;
    private Transform[] _transforms;
    private Material[] _materials;

    private bool _hovered;

    // Start is called before the first frame update
    void Start()
    {
        _transforms = GetComponentsInChildren<MeshFilter>().Select(r => r.transform).ToArray();
        _meshes = _transforms.Select(t => t.GetComponent<MeshFilter>().mesh).ToArray();
        _materials = _transforms.Select(t => t.GetComponent<Renderer>().material).ToArray();
    }

    /// <summary>
    /// LateUpdate is called every frame, if the Behaviour is enabled.
    /// It is called after all Update functions have been called.
    /// </summary>
    void LateUpdate()
    {
        if (_hovered)
        {
            Graphics.SetRenderTarget(TargetTexture);

            for (int i = 0; i < _meshes.Length; i++)
            {
                _materials[i].SetPass(0);
                Graphics.DrawMeshNow(_meshes[i], _transforms[i].position, _transforms[i].rotation);
            }
        }
    }

    public void CursorEnter(InputManager manager)
    {
        _hovered = true;
    }

    public void CursorLeave(InputManager manager)
    {
        _hovered = false;
    }
}