using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Profiling;

namespace Jobs.DoD
{
    //Burst�����־
    [BurstCompile]
    struct WaveCubeJob : IJobParallelForTransform
    {
        [ReadOnly] public float deltaTime;


        //��ԭ����Update�����߼��ŵ�����
        public void Execute(int index, TransformAccess transform)
        {
            var distance = Vector3.Distance(transform.position, Vector3.zero);
            transform.localPosition += Vector3.up * math.sin(deltaTime * 3f + distance * 0.2f);
        }
    }
    public class CubeWaveWithJobs : MonoBehaviour
    {
        public GameObject cubeAchetype;
        [Range(10, 100)] public int XHalfCount = 40;
        [Range(10, 100)] public int YHalfCount = 40;

        //��ΪJobsֻ��ʹ��BlittableType���ݺͷ��й��ڴ�����ݣ���Transform����������Unity�ṩTransformAccessArray��TransformAccess
        public TransformAccessArray cubeList;

        static readonly ProfilerMarker<int> Profiler = new ProfilerMarker<int>("Wave Cube", "Object");

        void Start()
        {
            cubeList = new TransformAccessArray(4 * XHalfCount * XHalfCount);

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
        private void Update()
        {
            using (Profiler.Auto(cubeList.length))
            {
                var waveCubeJob = new WaveCubeJob
                {
                    deltaTime = Time.time,
                };
                //ͨ��Schedule�������е���
                var waveCubeJobhandle = waveCubeJob.Schedule(cubeList);
                //����Job�����Completeͬ�������߳���
                waveCubeJobhandle.Complete();
            }
        }
        private void OnDestroy()
        {
            cubeList.Dispose();
        }
    }
}