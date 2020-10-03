using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class OLDTVScreen : FilterBehavior {
    public float screenSaturation = 0.0f;

    public Texture chromaticAberrationPattern;
    public float chromaticAberrationMagnetude = 0.015f;

    public Texture noisePattern;
    public float noiseMagnetude = 0.1f;

    public Texture staticPattern;
    public Texture staticMask;
	public float staticVertical = 0.0f;
	public float staticMagnetude = 0.015f;
    public float staticVerticalScroll = 0f;

    void OnRenderImage( RenderTexture source, RenderTexture destination ) {
        this.material.SetFloat( "_Saturation", screenSaturation );

		this.material.SetTexture( "_ChromaticAberrationTex", chromaticAberrationPattern );
		this.material.SetFloat( "_ChromaticAberrationMagnetude", chromaticAberrationMagnetude );

		this.material.SetTexture( "_NoiseTex", noisePattern );
		this.material.SetTextureOffset( "_NoiseTex", new Vector2( Random.Range( 0.0f, 1.0f ), Random.Range( 0.0f, 1.0f ) ) );
		this.material.SetFloat( "_NoiseMagnetude", noiseMagnetude );

		this.material.SetTexture( "_StaticTex", staticPattern );
		this.material.SetTextureOffset( "_StaticTex", new Vector2( Random.Range( 0.0f, 1.0f ), Random.Range( 0.0f, 1.0f ) ) );
		this.material.SetTexture( "_StaticMask", staticMask );
		this.material.SetTextureOffset( "_StaticMask", new Vector2( 0.0f, staticVertical ) );
		this.material.SetFloat( "_StaticMagnetude", staticMagnetude );

        if (staticVerticalScroll > 0f)
        {
            staticVertical += Time.deltaTime * staticVerticalScroll;
        }

        Graphics.Blit( source, destination, this.material );
    }
}
