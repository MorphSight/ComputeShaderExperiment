using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using _Shaders;
using UnityEngine;
using Random = System.Random;

namespace DefaultNamespace
{
    
    public class TestVoronoi : MonoBehaviour
    {
        public Texture2D Tex;
        public int MapSize;
        public int BiomeNodeCount;
        public int NodeCount;
        public int BiomeTypeCount;
        
        private void Start()
        {
            var random = new Random();
            var timer = new Timer("Voronoi Algorithm");
            var mapSize = MapSize;

            var biomeTypes = new Color[BiomeTypeCount];

            for (int i = 0; i < biomeTypes.Length; i++)
            {
                biomeTypes[i] = new Color(
                    RandomFloat(random),
                    RandomFloat(random),
                    RandomFloat(random),
                    1);
            }
            
            var biomes = GetNodes(BiomeNodeCount, random, mapSize);
            var nodes = GetNodes(NodeCount, random, mapSize);

            for (int i = 0; i < biomes.Length; i++)
            {
                biomes[i].Color = biomeTypes[random.Next(biomeTypes.Length)];
            }

            var timerTwo = new Timer("Biomes");
            timerTwo.StartTime();
            nodes = Shaders.VoronoiShader.FindNearest(biomes, nodes);
            timerTwo.EndTime();
            timerTwo.LogUnity();

            timer.StartTime();
            var data = Shaders.VoronoiShader.GetVoronoiIndices(nodes, mapSize);
            timer.EndTime();
            timer.LogUnity();

            /*StringBuilder stringBuilder = new StringBuilder();
            
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    stringBuilder.Append(data[x * mapSize + y]);
                }

                stringBuilder.Append("\n");
            }

            var path = Application.persistentDataPath + "/Map.txt";
            
            File.WriteAllText(path, stringBuilder.ToString());
            Debug.Log("Written result to " + path);*/

            var colors = new Color[data.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = nodes[data[i]].Color;
            }
            
            Tex = new Texture2D(mapSize, mapSize);
            Tex.SetPixels(colors);
            Tex.Apply();

            GetComponent<Renderer>().material.mainTexture = Tex;
        }

        private Node[] GetNodes(int count, Random random, float mapSize)
        {
            var retVal = new Node[count];

            for (int i = 0; i < count; i++)
            {
                retVal[i].Position = new Vector3((float) random.NextDouble() * mapSize,
                    (float) random.NextDouble() * mapSize, 0);
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
}