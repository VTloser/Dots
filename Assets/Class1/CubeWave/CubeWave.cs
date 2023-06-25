using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
public class CubeWave : MonoBehaviour
{
    public GameObject cubeAchetype;
    [Range(10, 100)] public int XHalfCount = 40;
    [Range(10, 100)] public int YHalfCount = 40;
    public List<Transform> cubeList;

    //使用新 Unity.Profiling 接口 对位置更新逻辑代码进行标记
    static readonly ProfilerMarker<int> Profiler = new ProfilerMarker<int>("Wave Cube", "Object");

    void Start()
    {
        cubeList = new List<Transform>();

        for (int i = -XHalfCount; i <= XHalfCount; i++)
        {
            for (int j = -YHalfCount; j <= YHalfCount; j++)
            {
                var cube = Instantiate(cubeAchetype);
                cube.transform.position = new Vector3(i * 1.1f, 0, j * 1.1f);
                cubeList.Add(cube.transform);
            }
        }
    }
    void Update()
    {
        //对位置更新逻辑代码进行标记
        using (Profiler.Auto(cubeList.Count))
        {
            for (int i = 0; i < cubeList.Count; i++)
            {
                var distance = Vector3.Distance(cubeList[i].position, Vector3.zero);
                cubeList[i].localPosition += Vector3.up * Mathf.Sin(Time.time * 3f + distance * 0.2f);
            }
        }
    }
}
