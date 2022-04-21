using System;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PostEffectsBase : MonoBehaviour
{
	protected bool supportHDRTextures;

	protected bool supportDX11;

	protected bool isSupported;

	public PostEffectsBase()
	{
		supportHDRTextures = true;
		isSupported = true;
	}

	public Material CheckShaderAndCreateMaterial(Shader s, Material m2Create) // should be override
	{
		object result;
		if (!s)
		{
			Debug.Log("Missing shader in " + ToString());
			enabled = false;
			result = null;
		}
		else if (s.isSupported && (bool)m2Create && m2Create.shader == s)
		{
			result = m2Create;
		}
		else if (!s.isSupported)
		{
			NotSupported();
			Debug.Log("The shader " + s.ToString() + " on effect " + ToString() + " is not supported on this platform!");
			result = null;
		}
		else
		{
			m2Create = new Material(s);
			m2Create.hideFlags = HideFlags.DontSave;
			result = ((!m2Create) ? null : m2Create);
		}
		return (Material)result;
	}

	public Material CreateMaterial(Shader s, Material m2Create) // should be override
	{
		object result;
		if (!s)
		{
			Debug.Log("Missing shader in " + ToString());
			result = null;
		}
		else if ((bool)m2Create && m2Create.shader == s && s.isSupported)
		{
			result = m2Create;
		}
		else if (!s.isSupported)
		{
			result = null;
		}
		else
		{
			m2Create = new Material(s);
			m2Create.hideFlags = HideFlags.DontSave;
			result = ((!m2Create) ? null : m2Create);
		}
		return (Material)result;
	}

	public void OnEnable() // should be override
	{
		isSupported = true;
	}

	public bool CheckSupport() // should be override
	{
		return CheckSupport(false);
	}

	public bool CheckResources() // should be override
	{
		Debug.LogWarning("CheckResources () for " + ToString() + " should be overwritten.");
		return isSupported;
	}

	public void Start() // should be override
	{
		CheckResources();
	}

	public bool CheckSupport(bool needDepth) // should be override
	{
		isSupported = true;
		supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
		bool num = SystemInfo.graphicsShaderLevel >= 50;
		if (num)
		{
			num = SystemInfo.supportsComputeShaders;
		}
		supportDX11 = num;
		int result;
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			NotSupported();
			result = 0;
		}
		else if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			NotSupported();
			result = 0;
		}
		else
		{
			if (needDepth)
			{
				GetComponent<Camera>().depthTextureMode = GetComponent<Camera>().depthTextureMode | DepthTextureMode.Depth;
			}
			result = 1;
		}
		return (byte)result != 0;
	}

	public bool CheckSupport(bool needDepth, bool needHdr) // should be override
	{
		int result;
		if (!CheckSupport(needDepth))
		{
			result = 0;
		}
		else if (needHdr && !supportHDRTextures)
		{
			NotSupported();
			result = 0;
		}
		else
		{
			result = 1;
		}
		return (byte)result != 0;
	}

	public bool Dx11Support() // should be override
	{
		return supportDX11;
	}

	public void ReportAutoDisable() // should be override
	{
		Debug.LogWarning("The image effect " + ToString() + " has been disabled as it's not supported on the current platform.");
	}

	public bool CheckShader(Shader s) // should be override
	{
		Debug.Log("The shader " + s.ToString() + " on effect " + ToString() + " is not part of the Unity 3.2+ effects suite anymore. For best performance and quality, please ensure you are using the latest Standard Assets Image Effects (Pro only) package.");
		int result;
		if (!s.isSupported)
		{
			NotSupported();
			result = 0;
		}
		else
		{
			result = 0;
		}
		return (byte)result != 0;
	}

	public void NotSupported() // should be override
	{
		enabled = false;
		isSupported = false;
	}

	public void DrawBorder(RenderTexture dest, Material material) // should be override
	{
		float num = default(float);
		float num2 = default(float);
		float num3 = default(float);
		float num4 = default(float);
		RenderTexture.active = dest;
		bool flag = true;
		GL.PushMatrix();
		GL.LoadOrtho();
		for (int i = 0; i < material.passCount; i++)
		{
			material.SetPass(i);
			float num5 = default(float);
			float num6 = default(float);
			if (flag)
			{
				num5 = 1f;
				num6 = 0f;
			}
			else
			{
				num5 = 0f;
				num6 = 1f;
			}
			num = 0f;
			num2 = 0f + 1f / ((float)dest.width * 1f);
			num3 = 0f;
			num4 = 1f;
			GL.Begin(7);
			GL.TexCoord2(0f, num5);
			GL.Vertex3(num, num3, 0.1f);
			GL.TexCoord2(1f, num5);
			GL.Vertex3(num2, num3, 0.1f);
			GL.TexCoord2(1f, num6);
			GL.Vertex3(num2, num4, 0.1f);
			GL.TexCoord2(0f, num6);
			GL.Vertex3(num, num4, 0.1f);
			num = 1f - 1f / ((float)dest.width * 1f);
			num2 = 1f;
			num3 = 0f;
			num4 = 1f;
			GL.TexCoord2(0f, num5);
			GL.Vertex3(num, num3, 0.1f);
			GL.TexCoord2(1f, num5);
			GL.Vertex3(num2, num3, 0.1f);
			GL.TexCoord2(1f, num6);
			GL.Vertex3(num2, num4, 0.1f);
			GL.TexCoord2(0f, num6);
			GL.Vertex3(num, num4, 0.1f);
			num = 0f;
			num2 = 1f;
			num3 = 0f;
			num4 = 0f + 1f / ((float)dest.height * 1f);
			GL.TexCoord2(0f, num5);
			GL.Vertex3(num, num3, 0.1f);
			GL.TexCoord2(1f, num5);
			GL.Vertex3(num2, num3, 0.1f);
			GL.TexCoord2(1f, num6);
			GL.Vertex3(num2, num4, 0.1f);
			GL.TexCoord2(0f, num6);
			GL.Vertex3(num, num4, 0.1f);
			num = 0f;
			num2 = 1f;
			num3 = 1f - 1f / ((float)dest.height * 1f);
			num4 = 1f;
			GL.TexCoord2(0f, num5);
			GL.Vertex3(num, num3, 0.1f);
			GL.TexCoord2(1f, num5);
			GL.Vertex3(num2, num3, 0.1f);
			GL.TexCoord2(1f, num6);
			GL.Vertex3(num2, num4, 0.1f);
			GL.TexCoord2(0f, num6);
			GL.Vertex3(num, num4, 0.1f);
			GL.End();
		}
		GL.PopMatrix();
	}

	public void Main() // should be override
	{
	}
}
