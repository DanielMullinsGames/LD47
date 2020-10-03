using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UT.UEditor;


[CustomEditor( typeof( OLDTVScreen ) )]
[CanEditMultipleObjects]

public class OLDTVScreenEditor : Editor {
    private Texture2D _logo = null;

    private bool _foldChromaticAberration = true;
    private bool _foldNoise = true;
    private bool _foldStatic = true;

    private SerializedProperty _material;

    private SerializedProperty _screenSaturation;

    private SerializedProperty _chromaticAberrationPattern;
    private SerializedProperty _chromaticAberrationMagnetude;

    private SerializedProperty _noisePattern;
    private SerializedProperty _noiseMagnetude;

    private SerializedProperty _staticPattern;
    private SerializedProperty _staticMask;
	private SerializedProperty _staticVertical;
	private SerializedProperty _staticMagnetude;
    private SerializedProperty _staticVerticalScroll;

    private OLDTVScreen Target {
        get { return ( OLDTVScreen )target; }
    }

    void OnEnable() {
        if ( _logo == null ) {
            _logo = ( Texture2D )AssetDatabase.LoadAssetAtPath( "Assets/Vortex Game Studios/OLD TV Filter 2/Gizmos/logo_screen.png", typeof( Texture2D ) );
        }

        _material = serializedObject.FindProperty( "tvMaterialScreen" );
        _screenSaturation = serializedObject.FindProperty( "screenSaturation" );

        _chromaticAberrationPattern = serializedObject.FindProperty( "chromaticAberrationPattern" );
        _chromaticAberrationMagnetude = serializedObject.FindProperty( "chromaticAberrationMagnetude" );

        _noisePattern = serializedObject.FindProperty( "noisePattern" );
        _noiseMagnetude = serializedObject.FindProperty( "noiseMagnetude" );

        _staticPattern = serializedObject.FindProperty( "staticPattern" );
        _staticMask = serializedObject.FindProperty( "staticMask" );
		_staticVertical = serializedObject.FindProperty( "staticVertical" );
		_staticMagnetude = serializedObject.FindProperty( "staticMagnetude" );
        _staticVerticalScroll = serializedObject.FindProperty( "staticVerticalScroll" );

    }
	
    public override void OnInspectorGUI() {      
        GUILayout.Box( _logo, GUIStyle.none );
		EditorGUILayout.LabelField( "Ver. 2.1" );

		//_material.objectReferenceValue = EditorGUILayout.ObjectField( "Material", Target.tvMaterialScreen, typeof( Material ), false ) as Material;

        _screenSaturation.floatValue = EditorGUILayout.Slider( "Saturation", Target.screenSaturation, 0.0f, 1.0f );

        _foldChromaticAberration = UEditor.BeginGroup( null, "Chromatic Aberration", _foldChromaticAberration, Color.Lerp( Color.gray, Color.white, 0.4f ) );
        if ( _foldChromaticAberration ) {
            _chromaticAberrationPattern.objectReferenceValue = EditorGUILayout.ObjectField( "Pattern", Target.chromaticAberrationPattern, typeof( Texture ), false ) as Texture;
            _chromaticAberrationMagnetude.floatValue = EditorGUILayout.Slider( "Magnetude", Target.chromaticAberrationMagnetude, 0.0f, 1.0f );
        }
        UEditor.EndGroup();

        _foldNoise = UEditor.BeginGroup( null, "Noise", _foldNoise, Color.Lerp( Color.gray, Color.white, 0.4f ) );
        if ( _foldNoise ) {
            _noisePattern.objectReferenceValue = EditorGUILayout.ObjectField( "Pattern", Target.noisePattern, typeof( Texture ), false ) as Texture;
            _noiseMagnetude.floatValue = EditorGUILayout.Slider( "Magnetude", Target.noiseMagnetude, -1.0f, 1.0f );
        }
        UEditor.EndGroup();

        _foldStatic = UEditor.BeginGroup( null, "Static", _foldStatic, Color.Lerp( Color.gray, Color.white, 0.4f ) );
        if ( _foldStatic ) {
            _staticPattern.objectReferenceValue = EditorGUILayout.ObjectField( "Pattern", Target.staticPattern, typeof( Texture ), false ) as Texture;
            _staticMask.objectReferenceValue = EditorGUILayout.ObjectField( "Mask", Target.staticMask, typeof( Texture ), false ) as Texture;
			_staticVertical.floatValue = EditorGUILayout.Slider( "Vertical", Target.staticVertical, 0.0f, 1.0f );
			_staticMagnetude.floatValue = EditorGUILayout.Slider( "Magnetude", Target.staticMagnetude, 0.0f, 1.0f );
            _staticVerticalScroll.floatValue = EditorGUILayout.Slider("Vertical Scroll", Target.staticVerticalScroll, 0.0f, 1.0f);
        }
        UEditor.EndGroup();

        serializedObject.ApplyModifiedProperties();
	}
}
