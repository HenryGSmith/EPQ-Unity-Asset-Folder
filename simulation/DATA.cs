using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewData", menuName = "Species Data")]
public class DATA : ScriptableObject
{
    public new string name;

    public AgentSetUp.SpawnMode spawnMode;

    [Header("--Input Parameters--")]
    //range 1 to 65535
    public int NumberOfAgents;
    public float AgentMovementSpeed;
    public float DecayRate;
    public float DiffuseRate;

    [Header("--Sensor Settings--")]
    public bool RunSense;
    public float SenseOffsetDist;
    public float SensorSize;
    public float sensorAngleSpacing;
    public float TurnSpeed;

    [Header("--Output Dimensions--")]
    public int width;
    public int height;

    [Header("--Colour--")]
    public float red;
    public float blue;
    public float green;
    public float alpha;
}
