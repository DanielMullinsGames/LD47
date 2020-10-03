using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class LightCameraRenderer : MonoBehaviour
{
	public Material Material;
	[SerializeField][HideInInspector]
	RenderTexture _texture = default;

	void Awake()
	{
		GenerateRenderTexture();
	}

	void GenerateRenderTexture()
	{
		if (_texture != null)
		{
			_texture.Release();
			DestroyImmediate(_texture);
		}

		_texture = new RenderTexture(Screen.width, Screen.height, 0);
		_texture.filterMode = FilterMode.Bilinear;
		_texture.antiAliasing = 2;
	}

	#region Editor Update
#if UNITY_EDITOR
	void Update()
	{
		// Constantly refreshes the render texture in editor, in case you're resizing or something, keeps things looking nice.
		if (!UnityEditor.EditorApplication.isPlaying && (Screen.width != _texture.width || Screen.height != _texture.height))
			GenerateRenderTexture();
	}
#endif
	#endregion

	void OnPreRender()
	{
		GetComponent<Camera>().targetTexture = _texture;
	}

	void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		GetComponent<Camera>().targetTexture = null;
		Graphics.Blit(src, dst);
		Graphics.Blit(dst, null, Material);
	}
}
