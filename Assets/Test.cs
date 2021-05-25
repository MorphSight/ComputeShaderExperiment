using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = System.Random;

public struct NodeTest
{
    public Vector3 Position;
    public Color Color;

    public static int SizeOf()
    {
        return sizeof(float) * 7;
    }
}

public class Test : MonoBehaviour
{
    public ComputeShader Shader;
    public RenderTexture Output;
    public ComputeBuffer Nodes;
    public int Count;

    private void Start()
    {
        if (Output == null)
        {
            Output = new RenderTexture(1024, 1024, 24) {enableRandomWrite = true};
            Output.Create();
        }

        if (Nodes == null)
        {
            var data = GetNodes(5000, new Random());

            Count = data.Length;
            Nodes = new ComputeBuffer(data.Length, NodeTest.SizeOf());
            Nodes.SetData(data);
        }
        
        //Create nodes
        
        
        Shader.SetFloat("Resolution", Output.width);
        Shader.SetBuffer(0, "nodes", Nodes);
        Shader.SetInt("NodeCount", Count);
        Shader.SetTexture(0, "Result", Output);

        var dispatchTimer = new Timer("Dispatch");

        dispatchTimer.StartTime();
        Shader.Dispatch(0, Output.width/8, Output.height/8, 1);
        dispatchTimer.EndTime();
        dispatchTimer.LogUnity();
    }

    /*private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        
        
        Graphics.Blit(Output, dest);
        
        
    }*/

    
    private NodeTest[] GetNodes(int count, Random random)
    {
        var retVal = new NodeTest[count];

        for (int i = 0; i < count; i++)
        {
            retVal[i].Position = new Vector3((float) random.NextDouble(),
                (float) random.NextDouble(), 0);
            retVal[i].Color = new Color(
                RandomFloat(random),
                RandomFloat(random), 
                RandomFloat(random), 
                1);
        }

        return retVal;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private float RandomFloat(Random random)
    {
        return (float) random.NextDouble();
    }
}
