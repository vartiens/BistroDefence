using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEditor;

public class AnimatorTextureBaker : MonoBehaviour
{
    public ComputeShader infoTexGen;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BakeAnimation()); // Start coroutine 
    }

    IEnumerator BakeAnimation()
    {
        var animator = GetComponent<Animator>();
        var clips = animator.runtimeAnimatorController.animationClips;
        var skin = GetComponentInChildren<SkinnedMeshRenderer>();
        var vCount = skin.sharedMesh.vertexCount;

        var mesh = new Mesh();
        animator.speed = 0;
        var textWidth = Mathf.NextPowerOfTwo(vCount);
        foreach(var c in clips )
        {
            var frames = Mathf.NextPowerOfTwo((int)(c.length / .05f));
            var info = new List<VertInfo>();

            var pRt = new RenderTexture(textWidth, frames, 0, RenderTextureFormat.ARGBHalf);
            var nRt = new RenderTexture(textWidth, frames, 0, RenderTextureFormat.ARGBHalf);
            pRt.name = string.Format("{0}.{1}.posTex", name, c.name);
            nRt.name = string.Format("{0}.{1}.normTex", name, c.name);

            foreach (var rt in new[] {pRt, nRt}) 
            {
                rt.enableRandomWrite = true;
                rt.Create();
                RenderTexture.active = rt;
                GL.Clear(true, true, Color.clear);
            }
            animator.Play(c.name);
            yield return 0;
            for (var i = 0; i < frames; i++)
            {
                animator.Play(c.name, 0, (float)i / frames);
                yield return 0;
                skin.BakeMesh(mesh);
                info.AddRange(Enumerable.Range(0, vCount).Select(idx => new VertInfo()
                {
                    position = mesh.vertices[idx],
                    normal = mesh.normals[idx]
                }));
            }
            var buffer = new ComputeBuffer(info.Count, System.Runtime.InteropServices.Marshal.SizeOf(typeof(VertInfo))); // Allocation
            buffer.SetData(info); // Sync

            var kernel = infoTexGen.FindKernel("CSMain");
            uint x, y, z;
            infoTexGen.GetKernelThreadGroupSizes(kernel, out x, out y, out z);

            infoTexGen.SetInt("VertCount", vCount);
            infoTexGen.SetBuffer(kernel, "meshInfo", buffer);
            infoTexGen.SetTexture(kernel, "OutPosition", pRt);
            infoTexGen.SetTexture(kernel, "OutNormal", nRt);

            infoTexGen.Dispatch(kernel, vCount / (int)x + 1, frames / (int)y + 1, (int)z);
            buffer.Release();

#if UNITY_EDITOR
          var posTex = Convert(pRt);
          var normTex = Convert(nRt);
    
          Graphics.CopyTexture(pRt, posTex);
          Graphics.CopyTexture(nRt, normTex);

         AssetDatabase.CreateAsset(posTex, Path.Combine("Assets", pRt.name + ".asset"));
         AssetDatabase.CreateAsset(normTex, Path.Combine("Assets", nRt.name + ".asset"));

          AssetDatabase.SaveAssets();
          AssetDatabase.Refresh();
#endif

        }

        yield return null;
    }

    public Texture2D Convert(RenderTexture rt)
    {
        var texture = new Texture2D(rt.width, rt.height, TextureFormat.RGBAHalf, false);
        RenderTexture.active = rt;
        texture.ReadPixels(Rect.MinMaxRect(0, 0, rt.width, rt.height), 0, 0);
        return texture;
    }

    public struct VertInfo
    {
        public Vector3 position;
        public Vector3 normal;
    }
}
