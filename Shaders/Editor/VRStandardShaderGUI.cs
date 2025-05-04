// Need Todos
// Add _PARALLAX, _EMISSION,  static switch in Amplify
// Possibly refactor PackingMode to be BETTER
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class VRStandardShaderGUI : ShaderGUI
{
    public enum BlendMode
    {
        Opaque,
        AlphaTest,
        AlphaBlend,
        Glass,
        Additive,
        Mod2x,
        Multiply
        // TODO: MaskedGlass that will require an additional grayscale texture to act as a standard alpha blend mask
    }

    public enum SpecularMode
    {
        None,
        BlinnPhong,
        Metallic,
        Anisotropic,
        Retroreflective
    }

    public enum VertexMode
    {

        None,
        Tint

    }

    public enum TexturePackingMode
    {
        None,
        MAES,
        RMA,
        MAS,
        MASK,
        MRA,
        ORM,
        Alloy
    }

    public enum DetailBlendMode
    {
        Multiply2x,
        Multiply,
        Add,
        Lerp
    }

    private static class Styles
    {
        public static GUIStyle optionsButton = "PaneOptions";
        public static GUIContent uvSetLabel = new GUIContent("UV Set");
        public static GUIContent[] uvSetOptions = new GUIContent[] { new GUIContent("UV channel 0"), new GUIContent("UV channel 1") };

        public static GUIContent unlitText = new GUIContent("Unlit", "");

        public static string emptyTootip = "";
        public static GUIContent albedoText = new GUIContent("Albedo", "Albedo (RGB) and Transparency (A)");
        public static GUIContent BRDFMapText = new GUIContent("BRDF LUT", "Remaps shading to color ramp. Compared to the normal, X is direction to the light, Y is direction from camera.");
        public static GUIContent colorMaskText = new GUIContent("Color Mask", "Multiplys albedo per color channel");
        public static GUIContent alphaCutoffText = new GUIContent("Alpha Cutoff", "Threshold for alpha cutoff");
        public static GUIContent FluorescenceText = new GUIContent("Fluorescence", "Fluorescence. Absorbs light and re-emits at a longer wavelength. MAX color value of this and albedo");
        public static GUIContent AbsorbanceText = new GUIContent("Absorbance Color", "Absorbance Color");
        public static GUIContent specularMapText = new GUIContent("Specular", "Reflectance (RGB) and Gloss (A)");
        public static GUIContent reflectanceMinText = new GUIContent("Reflectance Min", "");
        public static GUIContent reflectanceMaxText = new GUIContent("Reflectance Max", "");
        public static GUIContent metallicMapText = new GUIContent("Metallic", "Metallic (R), Gloss (A) ");
        public static GUIContent specularModText = new GUIContent("Smoothness Scaler", "Smoothness Multiplier");
        public static GUIContent anisotropicRatioText = new GUIContent("Anisotropic Ratio", "");
        public static GUIContent anisotropicRotationText = new GUIContent("Anisotropic Rotation", "");
        public static GUIContent smoothnessText = new GUIContent("Gloss", "");
        public static GUIContent normalMapText = new GUIContent("Normal", "Normal Map");
        public static GUIContent heightMapText = new GUIContent("Parallax Map", "Parallax Map (R), Float controls depth");
        public static GUIContent heightIterationsText = new GUIContent("Iterations", "More iterations reduces artifacts but makes material heavier. Use with care.");
        public static GUIContent heightOffsetText = new GUIContent("Zero Plane Offset", "");
        public static GUIContent fresnelfallofftext = new GUIContent("Fresnel Falloff Scalar", "Added Fresnel falloff contribution");
        public static GUIContent fresnelExponentText = new GUIContent("Fresnel Exponent", "");
        public static GUIContent cubeMapScalarText = new GUIContent("Cube Map Scalar", "Multiplier for overall reflections");
        public static GUIContent NormalToOcclusionText = new GUIContent("Normal to Occlusion", "Add normal map to Occlusion");
        public static GUIContent occlusionText = new GUIContent("Occlusion", "Occlusion (G)");
        public static GUIContent occlusionStrengthDirectDiffuseText = new GUIContent("Direct Diffuse", "");
        public static GUIContent occlusionStrengthDirectSpecularText = new GUIContent("Direct Specular", "");
        public static GUIContent occlusionStrengthIndirectDiffuseText = new GUIContent("Indirect Diffuse", "");
        public static GUIContent occlusionStrengthIndirectSpecularText = new GUIContent("Indirect Specular", "");
        public static GUIContent emissionText = new GUIContent("Emission", "Emission (RGB)");
        public static GUIContent emissionFalloffText = new GUIContent("Falloff", "Emission Falloff");
        public static GUIContent detailMaskText = new GUIContent("Detail Mask", "Mask for Secondary Maps (A)");
        public static GUIContent detailAlbedoText = new GUIContent("Detail Albedo", "Detail Albedo (RGB) multiplied by 2");
        public static GUIContent detailNormalMapText = new GUIContent("Detail Normal", "Detail Normal Map");
        public static GUIContent castShadowsText = new GUIContent("Cast shadows", "");
        public static GUIContent receiveShadowsText = new GUIContent("Receive Shadows", "");
        public static GUIContent renderBackfacesText = new GUIContent("Render Backfaces", "");
        public static GUIContent emissiveMode = new GUIContent("Multiply Albedo", "Multiply emissive color with albedo");
        public static GUIContent overrideLightmapText = new GUIContent("Override Lightmap", "Requires ValveOverrideLightmap.cs scrip on object");
        public static GUIContent worldAlignedTextureText = new GUIContent("World Aligned Texture", "");
        public static GUIContent worldAlignedTextureSizeText = new GUIContent("Size", "");
        public static GUIContent worldAlignedTextureNormalText = new GUIContent("Normal", "");
        public static GUIContent worldAlignedTexturePositionText = new GUIContent("World Position", "");

        public static string whiteSpaceString = " ";
        public static string primaryMapsText = "Main Maps";
        public static string secondaryMapsText = "Secondary Maps";
        public static string OverridesMapsText = "Override Settings";
        public static string renderingMode = "Rendering Mode";
        public static string specularModeText = "Specular Workflow";
        public static string VertexModeText = "Vertex Mode";
        public static string PackingModeText = "Packing Mode";
        public static string DetailModeText = "Detail Blend Mode";
        public static GUIContent emissiveWarning = new GUIContent("Emissive value is animated but the material has not been configured to support emissive. Please make sure the material itself has some amount of emissive.");
        public static GUIContent emissiveColorWarning = new GUIContent("Ensure emissive color is non-black for emission to have effect.");
        public static readonly string[] blendNames = Enum.GetNames(typeof(BlendMode));
        public static readonly string[] specularNames = Enum.GetNames(typeof(SpecularMode));
        public static readonly string[] vertexNames = Enum.GetNames(typeof(VertexMode));
        public static readonly string[] PackingNames = Enum.GetNames(typeof(TexturePackingMode));
        public static readonly string[] detailNames = Enum.GetNames(typeof(DetailBlendMode));


    }

    MaterialProperty blendMode;
    MaterialProperty emissiveMode;
    MaterialProperty PackingMode;
    MaterialProperty DetailMode;

    MaterialProperty albedoMap;
    MaterialProperty colorMask;
    MaterialProperty albedoColor;
    MaterialProperty colorShift1;
    MaterialProperty colorShift2;
    MaterialProperty colorShift3;
    MaterialProperty alphaCutoff;

    MaterialProperty metallicMap;
    MaterialProperty metallic;
    MaterialProperty smoothness;
    MaterialProperty SpecularMod;

    MaterialProperty bumpScale;
    MaterialProperty bumpMap;
    MaterialProperty heigtMapScale;
    MaterialProperty heightMap;
    MaterialProperty NormalToOcclusion;
    MaterialProperty occlusionStrength;
    MaterialProperty occlusionMap;
    MaterialProperty emissionColorForRendering;
    MaterialProperty emissionMap;
    MaterialProperty emissionFalloff;
    MaterialProperty bakedEmission;

    MaterialProperty detailMask;
    MaterialProperty detailAlbedoMap;
    MaterialProperty detailNormalMapScale;
    MaterialProperty detailNormalMap;
    MaterialProperty renderBackfaces;

    MaterialEditor m_MaterialEditor;

    bool m_FirstTimeApply = true;

    public void FindProperties(MaterialProperty[] props)
    {
        emissiveMode = FindProperty("_EmissiveMode", props);
        PackingMode = FindProperty("_PackingMode", props);
        DetailMode = FindProperty("_DetailMode", props);
        albedoMap = FindProperty("_MainTex", props);
        colorMask = FindProperty("_ColorMask", props);
        albedoColor = FindProperty("_Color", props);
        colorShift1 = FindProperty("_ColorShift1", props);
        colorShift2 = FindProperty("_ColorShift2", props);
        colorShift3 = FindProperty("_ColorShift3", props);
        try
        {
            alphaCutoff = FindProperty("_Cutoff", props);
        }
        catch
        {

        }

        metallicMap = FindProperty("_MetallicGlossMap", props, false);
        metallic = FindProperty("_Metallic", props, false);
        smoothness = FindProperty("_Glossiness", props);
        SpecularMod = FindProperty("_SpecMod", props);
        bumpScale = FindProperty("_BumpScale", props);
        bumpMap = FindProperty("_BumpMap", props);
        heigtMapScale = FindProperty("_Parallax", props);
        heightMap = FindProperty("_ParallaxMap", props);
        NormalToOcclusion = FindProperty("_NormalToOcclusion", props);
        occlusionStrength = FindProperty("_OcclusionStrength", props);
        occlusionMap = FindProperty("_OcclusionMap", props);
        emissionColorForRendering = FindProperty("_EmissionColor", props);
        emissionMap = FindProperty("_EmissionMap", props);
        emissionFalloff = FindProperty("_EmissionFalloff", props);
        bakedEmission = FindProperty("_BakedMutiplier", props);

        detailMask = FindProperty("_DetailMask", props);
        detailAlbedoMap = FindProperty("_DetailAlbedoMap", props);
        detailNormalMapScale = FindProperty("_DetailNormalMapScale", props);
        detailNormalMap = FindProperty("_DetailNormalMap", props);
        renderBackfaces = FindProperty("g_bRenderBackfaces", props);
    }
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        FindProperties(props);
        m_MaterialEditor = materialEditor;
        Material material = materialEditor.target as Material;

        EditorGUI.BeginChangeCheck();
        ShaderPropertiesGUI(material);
        base.OnGUI(materialEditor, props);

        if (EditorGUI.EndChangeCheck())
        {
            MaterialChanged(material);
        }

        if (m_FirstTimeApply)
        {
            SetMaterialKeywords(material);
            m_FirstTimeApply = false;
        }
    }

    public void ShaderPropertiesGUI(Material material)
    {
        EditorGUIUtility.labelWidth = 0f;

        EditorGUI.BeginChangeCheck();

        bool bUnlit = false;

        if (!bUnlit)
        {
            TexturePackingPopup();
        }

        m_MaterialEditor.ShaderProperty(renderBackfaces, Styles.renderBackfacesText.text);

        EditorGUILayout.Space();

        GUILayout.Label(Styles.primaryMapsText, EditorStyles.boldLabel);

        DoAlbedoArea(material);

        if (!bUnlit)
        {
            DoSpecularMetallicArea(material);

            m_MaterialEditor.TexturePropertySingleLine(Styles.normalMapText, bumpMap, bumpMap.textureValue != null ? bumpScale : null);
            if (bumpMap.textureValue != null) m_MaterialEditor.ShaderProperty(NormalToOcclusion, Styles.NormalToOcclusionText.text, 1);

            if ((TexturePackingMode)material.GetInt("_PackingMode") == TexturePackingMode.None)
            {
                m_MaterialEditor.TexturePropertySingleLine(Styles.occlusionText, occlusionMap, occlusionMap.textureValue != null ? occlusionStrength : null);
            }
            else
            {
                m_MaterialEditor.RangeProperty(occlusionStrength, "Occlusion");
            }
        }

        DoEmissionArea(material);

        m_MaterialEditor.TexturePropertySingleLine(Styles.heightMapText, heightMap, heightMap.textureValue != null ? heigtMapScale : null);

        //if (heightMap.textureValue != null)
        //{

        //    float ParaIte = (material.GetFloat("_ParallaxIterations"));
        //    if ((ParaIte % 1) != 0) ParaIte -= ParaIte % 1;
        //    material.SetFloat("_ParallaxIterations", ParaIte);
        //    //m_MaterialEditor.ShaderProperty(heightIterations, Styles.heightIterationsText, 2);
        //}

        m_MaterialEditor.TextureScaleOffsetProperty(albedoMap);

        DoColorShiftArea(material);

        EditorGUILayout.Space();

        DetailModePopup();

        m_MaterialEditor.TexturePropertySingleLine(Styles.detailMaskText, detailMask);
        m_MaterialEditor.TexturePropertySingleLine(Styles.detailAlbedoText, detailAlbedoMap); 
        if (!bUnlit)
        {
            m_MaterialEditor.TexturePropertySingleLine(Styles.detailNormalMapText, detailNormalMap, detailNormalMapScale);
        }
        m_MaterialEditor.TextureScaleOffsetProperty(detailAlbedoMap);
        //m_MaterialEditor.ShaderProperty(uvSetSecondary, Styles.uvSetLabel.text);

        GUILayout.Label(Styles.OverridesMapsText, EditorStyles.boldLabel);

        if (!bUnlit)
        {
            //m_MaterialEditor.TexturePropertySingleLine(Styles.overrideLightmapText, overrideLightmap);
            m_MaterialEditor.ShaderProperty(SpecularMod, Styles.specularModText);
            //m_MaterialEditor.ShaderProperty(cubeMapScalar, Styles.cubeMapScalarText.text, 0);
        }

        if (EditorGUI.EndChangeCheck())
        {
            MaterialChanged(material);
        }
    }

    void TexturePackingPopup()
    {
        EditorGUI.showMixedValue = PackingMode.hasMixedValue;
        var mode = (TexturePackingMode)PackingMode.floatValue;

        EditorGUI.BeginChangeCheck();
        mode = (TexturePackingMode)EditorGUILayout.Popup(Styles.PackingModeText, (int)mode, Styles.PackingNames);
        if (EditorGUI.EndChangeCheck())
        {
            m_MaterialEditor.RegisterPropertyChangeUndo("Packing Mode");
            PackingMode.floatValue = (float)mode;
        }

        EditorGUI.showMixedValue = false;
    }

    void DetailModePopup()
    {
        EditorGUI.showMixedValue = DetailMode.hasMixedValue;
        var mode = (DetailBlendMode)DetailMode.floatValue;

        EditorGUI.BeginChangeCheck();
        mode = (DetailBlendMode)EditorGUILayout.Popup(Styles.DetailModeText, (int)mode, Styles.detailNames);
        if (EditorGUI.EndChangeCheck())
        {
            m_MaterialEditor.RegisterPropertyChangeUndo("Detail Mode");
            DetailMode.floatValue = (float)mode;
        }

        EditorGUI.showMixedValue = false;
    }

    void DoAlbedoArea(Material material)
    {
        m_MaterialEditor.TexturePropertySingleLine(Styles.albedoText, albedoMap, albedoColor);
        if (alphaCutoff != null)
        {
            m_MaterialEditor.ShaderProperty(alphaCutoff, Styles.alphaCutoffText.text, MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);
        }
        //if (((BlendMode)material.GetFloat("_Mode") == BlendMode.AlphaTest || (BlendMode)material.GetFloat("_Mode") == BlendMode.AlphaBlend || (BlendMode)material.GetFloat("_Mode") == BlendMode.Glass))
        //{
        //    if ((BlendMode)material.GetFloat("_Mode") == BlendMode.AlphaTest)
        //    {
        //        m_MaterialEditor.ShaderProperty(alphaCutoff, Styles.alphaCutoffText.text, MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);
        //    }
        //    else
        //    {
        //        m_MaterialEditor.ShaderProperty(alphaCutoff, "Alpha Falloff", MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);
        //    }
        //}
    }

    void DoColorShiftArea(Material material)
    {
        m_MaterialEditor.TexturePropertySingleLine(Styles.colorMaskText, colorMask);

        if (colorMask.textureValue != null)
        {
            m_MaterialEditor.ColorProperty(colorShift1, "Color Tint 1");
            m_MaterialEditor.ColorProperty(colorShift2, "Color Tint 2");
            m_MaterialEditor.ColorProperty(colorShift3, "Color Tint 3");
        }
    }

    void DoEmissionArea(Material material)
    {
        float brightness = emissionColorForRendering.colorValue.maxColorComponent;
        bool showHelpBox = !HasValidEmissiveKeyword(material);
        bool showEmissionColorAndGIControls = brightness > 0.0f;

        bool hadEmissionTexture = emissionMap.textureValue != null;

        // Texture and HDR color controls
        if ((TexturePackingMode)material.GetInt("_PackingMode") != TexturePackingMode.MAES)
        {
            m_MaterialEditor.TexturePropertyWithHDRColor(Styles.emissionText, emissionMap, emissionColorForRendering, false);
        }
        else
        {
            //  GUILayout.BeginHorizontal();              
            m_MaterialEditor.ColorProperty(emissionColorForRendering, "Emissive Color");
            //   GUILayout.EndHorizontal();

        }

        // If texture was assigned and color was black set color to white
        if (emissionMap.textureValue != null && !hadEmissionTexture && brightness <= 0f)
            emissionColorForRendering.colorValue = Color.white;


        // Dynamic Lightmapping mode
        if (showEmissionColorAndGIControls)
        {
            bool shouldEmissionBeEnabled = ShouldEmissionBeEnabled(emissionColorForRendering.colorValue);
            using (new EditorGUI.DisabledScope(!shouldEmissionBeEnabled))
            {
                m_MaterialEditor.ShaderProperty(emissionFalloff, Styles.emissionFalloffText.text, 2);
                m_MaterialEditor.ShaderProperty(emissiveMode, Styles.emissiveMode.text, 2);
                m_MaterialEditor.ShaderProperty(bakedEmission, "Baked Multiplier", 2);
                m_MaterialEditor.LightmapEmissionProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel);

            }
        }

        if (showHelpBox)
        {
            EditorGUILayout.HelpBox(Styles.emissiveWarning.text, MessageType.Warning);
        }
    }

    void DoSpecularMetallicArea(Material material)
    {
        GUIContent MetallicDis;

        if ((TexturePackingMode)material.GetInt("_PackingMode") == TexturePackingMode.None)
        {
            MetallicDis = Styles.metallicMapText;
        }
        else if ((TexturePackingMode)material.GetInt("_PackingMode") == TexturePackingMode.RMA)
        {
            MetallicDis = new GUIContent("RMA Metallic", "Roughness (R), Metallic (G), AO (B) ");
        }
        else if ((TexturePackingMode)material.GetInt("_PackingMode") == TexturePackingMode.MAES)
        {
            MetallicDis = new GUIContent("MAES Metallic", "Metallic (R), AO (G), Gray Emission (B) Gloss (A) ");
        }
        else if ((TexturePackingMode)material.GetInt("_PackingMode") == TexturePackingMode.MAS)
        {
            MetallicDis = new GUIContent("MAS Metallic", "Metallic (R), AO (G), Gloss (B) ");
        }
        else
        {
            MetallicDis = Styles.metallicMapText;
        }

        if (metallicMap.textureValue == null)
            m_MaterialEditor.TexturePropertyTwoLines(MetallicDis, metallicMap, metallic, Styles.smoothnessText, smoothness);
        else
            m_MaterialEditor.TexturePropertySingleLine(MetallicDis, metallicMap);

        if (((TexturePackingMode)material.GetInt("_PackingMode") == TexturePackingMode.RMA) && metallicMap.textureValue != null)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("RMA Texture likely needs sRGB disabled", MessageType.Info);
            GUILayout.EndHorizontal();
        }

        //if(material.GetInt("_MetallicType"))
    }

    public static void SetupTexturePackingMode(Material material, TexturePackingMode packingMode)
    {
        switch (packingMode)
        {
            case TexturePackingMode.None:
                material.EnableKeyword("_PACKING_NONE");
                material.DisableKeyword("_PACKING_MAES");
                material.DisableKeyword("_PACKING_RMA");
                material.DisableKeyword("_PACKING_MAS");
                material.DisableKeyword("_PACKING_MRA");
                material.DisableKeyword("_PACKING_MASK");
                material.DisableKeyword("_PACKING_ALLOY");
                material.DisableKeyword("_PACKING_ORM");
                break;
            case TexturePackingMode.RMA:
                material.DisableKeyword("_PACKING_NONE");
                material.DisableKeyword("_PACKING_MAES");
                material.EnableKeyword("_PACKING_RMA");
                material.DisableKeyword("_PACKING_MAS");
                material.DisableKeyword("_PACKING_MRA");
                material.DisableKeyword("_PACKING_MASK");
                material.DisableKeyword("_PACKING_ALLOY");
                material.DisableKeyword("_PACKING_ORM");
                break;
            case TexturePackingMode.MAES:
                material.DisableKeyword("_PACKING_NONE");
                material.EnableKeyword("_PACKING_MAES");
                material.DisableKeyword("_PACKING_RMA");
                material.DisableKeyword("_PACKING_MAS");
                material.DisableKeyword("_PACKING_MRA");
                material.DisableKeyword("_PACKING_MASK");
                material.DisableKeyword("_PACKING_ALLOY");
                material.DisableKeyword("_PACKING_ORM");
                break;
            case TexturePackingMode.MAS:
                material.DisableKeyword("_PACKING_NONE");
                material.DisableKeyword("_PACKING_MAES");
                material.DisableKeyword("_PACKING_RMA");
                material.EnableKeyword("_PACKING_MAS");
                material.DisableKeyword("_PACKING_MRA");
                material.DisableKeyword("_PACKING_MASK");
                material.DisableKeyword("_PACKING_ALLOY");
                material.DisableKeyword("_PACKING_ORM");
                break;
            case TexturePackingMode.MASK:
                material.DisableKeyword("_PACKING_NONE");
                material.DisableKeyword("_PACKING_MAES");
                material.DisableKeyword("_PACKING_RMA");
                material.DisableKeyword("_PACKING_MAS");
                material.DisableKeyword("_PACKING_MRA");
                material.EnableKeyword("_PACKING_MASK");
                material.DisableKeyword("_PACKING_ALLOY");
                material.DisableKeyword("_PACKING_ORM");
                break;
            case TexturePackingMode.MRA:
                material.DisableKeyword("_PACKING_NONE");
                material.DisableKeyword("_PACKING_MAES");
                material.DisableKeyword("_PACKING_RMA");
                material.DisableKeyword("_PACKING_MAS");
                material.EnableKeyword("_PACKING_MRA");
                material.DisableKeyword("_PACKING_MASK");
                material.DisableKeyword("_PACKING_ALLOY");
                material.DisableKeyword("_PACKING_ORM");
                break;
            case TexturePackingMode.ORM:
                material.DisableKeyword("_PACKING_NONE");
                material.DisableKeyword("_PACKING_MAES");
                material.DisableKeyword("_PACKING_RMA");
                material.DisableKeyword("_PACKING_MAS");
                material.DisableKeyword("_PACKING_MRA");
                material.DisableKeyword("_PACKING_MASK");
                material.DisableKeyword("_PACKING_ALLOY");
                material.EnableKeyword("_PACKING_ORM");
                break;
            case TexturePackingMode.Alloy:
                material.DisableKeyword("_PACKING_NONE");
                material.DisableKeyword("_PACKING_MAES");
                material.DisableKeyword("_PACKING_RMA");
                material.DisableKeyword("_PACKING_MAS");
                material.DisableKeyword("_PACKING_MRA");
                material.DisableKeyword("_PACKING_MASK");
                material.EnableKeyword("_PACKING_ALLOY");
                material.DisableKeyword("_PACKING_ORM");
                break;

        }
    }

    public static void SetupDetailBlendMode(Material material, DetailBlendMode detailMode)
    {
        switch (detailMode)
        {
            case DetailBlendMode.Multiply2x:
                material.EnableKeyword("_DETAIL_MULX2");
                material.DisableKeyword("_DETAIL_MUL");
                material.DisableKeyword("_DETAIL_ADD");
                material.DisableKeyword("_DETAIL_LERP");
                break;
            case DetailBlendMode.Multiply:
                material.DisableKeyword("_DETAIL_MULX2");
                material.EnableKeyword("_DETAIL_MUL");
                material.DisableKeyword("_DETAIL_ADD");
                material.DisableKeyword("_DETAIL_LERP");
                break;
            case DetailBlendMode.Add:
                material.DisableKeyword("_DETAIL_MULX2");
                material.DisableKeyword("_DETAIL_MUL");
                material.EnableKeyword("_DETAIL_ADD");
                material.DisableKeyword("_DETAIL_LERP");
                break;
            case DetailBlendMode.Lerp:
                material.DisableKeyword("_DETAIL_MULX2");
                material.DisableKeyword("_DETAIL_MUL");
                material.DisableKeyword("_DETAIL_ADD");
                material.EnableKeyword("_DETAIL_LERP");
                break;
        }
    }

    static bool ShouldEmissionBeEnabled(Color color)
    {
        return color.maxColorComponent > (0.1f / 255.0f);
    }

    static void SetMaterialKeywords(Material material)
    {
        TextureEnabledKeyword(material, "_MetallicGlossMap", "_METALLICGLOSSMAP");
        TextureEnabledKeyword(material, "_OcclusionMap", "S_OCCLUSION");
        TextureEnabledKeyword(material, "_ParallaxMap", "_PARALLAXMAP");
        TextureEnabledKeyword(material, "g_tBRDFMap", "_BRDFMAP");
        TextureEnabledKeyword(material, "_ColorMask", "_COLORSHIFT");
        TextureEnabledKeyword(material, "_DetailAlbedoMap", "_DETAIL");

        bool shouldEmissionBeEnabled = ShouldEmissionBeEnabled(material.GetColor("_EmissionColor"));
        SetKeyword(material, "_EMISSION", shouldEmissionBeEnabled);

        if (material.IsKeywordEnabled("S_RENDER_BACKFACES"))
        {
            material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        }
        else
        {
            material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Back);
        }
    }

    static void TextureEnabledKeyword(Material material, string textureProperty, string keyword)
    {
        SetKeyword(material, keyword, material.GetTexture(textureProperty) != null);
    }

    bool HasValidEmissiveKeyword(Material material)
    {
        // Material animation might be out of sync with the material keyword.
        // So if the emission support is disabled on the material, but the property blocks have a value that requires it, then we need to show a warning.
        // (note: (Renderer MaterialPropertyBlock applies its values to emissionColorForRendering))
        bool hasEmissionKeyword = material.IsKeywordEnabled("_EMISSION");
        if (!hasEmissionKeyword && ShouldEmissionBeEnabled(emissionColorForRendering.colorValue))
            return false;
        else
            return true;
    }

    static void MaterialChanged(Material material)
    {
        SetupTexturePackingMode(material, (TexturePackingMode)material.GetFloat("_PackingMode"));
        SetupDetailBlendMode(material, (DetailBlendMode)material.GetFloat("_DetailMode"));
        SetMaterialKeywords(material);
    }

    static void SetKeyword(Material m, string keyword, bool state)
    {
        if (state)
            m.EnableKeyword(keyword);
        else
            m.DisableKeyword(keyword);
    }
}