using _Shaders;
using UnityEngine;

public static class Shaders
{
    public static VoronoiShader VoronoiShader;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Load()
    {
        VoronoiShader = new VoronoiShader();
    }
}