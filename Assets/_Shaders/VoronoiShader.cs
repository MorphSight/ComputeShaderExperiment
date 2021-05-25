using System;
using JetBrains.Annotations;
using UnityEngine;
using Object = System.Object;

namespace _Shaders
{
    public struct Node
    {
        public Vector3 Position;
        public Color Color;

        public static int SizeOf()
        {
            return sizeof(float) * 7;
        }
    }

    public abstract class ShaderHandler
    {
        protected ComputeShader Shader;

        public ShaderHandler(string path)
        {
            Shader = Resources.Load<ComputeShader>(path);
        }

        public static RenderTexture GetRenderTexture(int width, int height)
        {
            return new RenderTexture(width, height, 24) {enableRandomWrite = true};
        }
    }
    
    public class VoronoiShader: ShaderHandler
    {
        private ComputeBuffer _nodesIn;
        private ComputeBuffer _nodesOut;

        private int _kernelIndexVoronoi;
        private int _kernelIndexVoronoiIndex;
        private int _kernelIndexNodes;

        public VoronoiShader(): base("Voronoi")
        {
            _kernelIndexVoronoi = Shader.FindKernel("Voronoi");
            _kernelIndexVoronoiIndex = Shader.FindKernel("VoronoiIndex");
            _kernelIndexNodes = Shader.FindKernel("VoronoiNode");
        }

        public int[] GetVoronoiIndices(Node[] nodesIn, int mapSize)
        {
            //Prepare Uniforms
            _nodesIn = new ComputeBuffer(nodesIn.Length, Node.SizeOf());
            _nodesIn.SetData(nodesIn);
            //var texture = new RenderTexture(mapSize, mapSize, 24) {enableRandomWrite = true};
            //texture.Create();

            var resultBuffer = new ComputeBuffer(mapSize * mapSize, sizeof(int));

            //Set Uniforms
            Shader.SetFloat("resolution", mapSize);
            Shader.SetBuffer(_kernelIndexVoronoiIndex, "nodes", _nodesIn);
            Shader.SetInt("node_count", nodesIn.Length);
            /*Shader.SetTexture(_kernelIndexVoronoi, "result", texture);*/
            Shader.SetBuffer(_kernelIndexVoronoiIndex, "result_map", resultBuffer);
            
            Shader.Dispatch(_kernelIndexVoronoiIndex,
                mapSize/8,
                mapSize/8,
                1);

            var data = new int[mapSize * mapSize];
            resultBuffer.GetData(data);
            return data;
        }
        
        public Color[] GetVoronoiColor(Node[] nodesIn, int mapSize)
        {
            //Prepare Uniforms
            _nodesIn = new ComputeBuffer(nodesIn.Length, Node.SizeOf());
            _nodesIn.SetData(nodesIn);
            var texture = new RenderTexture(mapSize, mapSize, 24) {enableRandomWrite = true};
            texture.Create();
            
            //Set Uniforms
            Shader.SetFloat("resolution", mapSize);
            Shader.SetBuffer(_kernelIndexVoronoi, "nodes", _nodesIn);
            Shader.SetInt("node_count", nodesIn.Length);
            Shader.SetTexture(_kernelIndexVoronoi, "result", texture);
            
            Shader.Dispatch(_kernelIndexVoronoi,
                texture.width/8,
                texture.height/8,
                1);

            var tex = new Texture2D(mapSize, mapSize);
            RenderTexture.active = texture;
            tex.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0, false);
            tex.Apply();
            RenderTexture.active = null;
            
            _nodesIn.Dispose();
            var data = tex.GetPixels(0, 0, tex.width, tex.height);
            _nodesIn.Dispose();
            
            return data;
        }

        public Node[] FindNearest(Node[] nodesIn, Node[] nodesOut)
        {
            _nodesIn = new ComputeBuffer(nodesIn.Length, Node.SizeOf());
            _nodesIn.SetData(nodesIn);

            _nodesOut = new ComputeBuffer(nodesOut.Length, Node.SizeOf());
            _nodesOut.SetData(nodesOut);
            
            Shader.SetBuffer(_kernelIndexNodes, "nodes", _nodesIn);
            Shader.SetBuffer(_kernelIndexNodes, "result_nodes", _nodesOut);
            Shader.SetInt("node_count", nodesIn.Length);
            
            Shader.Dispatch(_kernelIndexNodes,
                nodesOut.Length/8, 
                1, 
                1);

            var data = new Node[nodesOut.Length];
            _nodesOut.GetData(data);
            return data;
        }
    }
}