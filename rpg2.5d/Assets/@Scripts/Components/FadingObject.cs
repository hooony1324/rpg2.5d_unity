using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.XR.LegacyInputHelpers;

public class FadingObject : InitBase, IEquatable<FadingObject>
{

    public Vector3 Position;
    private MeshRenderer _renderer;
    private List<Material> _materials = new List<Material>();

    private bool _retainShadows = true;

    public float InitialAlpha = 1.0f;
    public float Alpha => _materials[0].color.a;
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        _renderer = GetComponent<MeshRenderer>();
        Position = transform.position;
        _materials.AddRange(_renderer.materials);

        return true;
    }

    bool IEquatable<FadingObject>.Equals(FadingObject other)
    {
        return Position.Equals(other.Position);
    }

    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }

    public void EnableMaterials()
    {
        foreach (Material mat in _materials)
        {
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.SetInt("_Surface", 1);

            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            mat.SetShaderPassEnabled("DepthOnly", false);
            mat.SetShaderPassEnabled("SHADOWCASTER", _retainShadows);

            mat.SetOverrideTag("RenderType", "Transparent");

            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        }
    }

    public void DisableMaterials()
    {
        foreach (Material mat in _materials)
        {
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetInt("_ZWrite", 1);
            mat.SetInt("_Surface", 0);

            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

            mat.SetShaderPassEnabled("DepthOnly", true);
            mat.SetShaderPassEnabled("SHADOWCASTER", true);

            mat.SetOverrideTag("RenderType", "Opaque");

            mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }
    }

    public void SetAlpha(float alpha)
    {
        foreach (Material mat in _materials)
        {
            if (mat.HasProperty("_BaseColor"))
            {
                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);
            }
        }
    }


}
