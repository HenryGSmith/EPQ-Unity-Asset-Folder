using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSetUp : MonoBehaviour
{
    public struct Agent
	{
		public Vector2 position;
		public float angle;
    }

	public enum SpawnMode { Point, Random, InwardCircle, RandomCircle, InwardCircle_Small}

    public static Agent[] SetUpAgents(Agent[] agents, SpawnMode spawnMode, int width, int height)
	{
		for (int i = 0; i < agents.Length; i++)
		{

			Vector2 centre = new Vector2(width / 2, height / 2);
			Vector2 startPos = Vector2.zero;
			float randomAngle = Random.value * Mathf.PI * 2;
			float angle = 0;

			if (spawnMode == SpawnMode.Point)
			{
				startPos = centre;
				angle = randomAngle;
			}
			else if (spawnMode == SpawnMode.Random)
			{
				startPos = new Vector2(Random.Range(0, width), Random.Range(0, height));
				angle = randomAngle;
			}
			else if (spawnMode == SpawnMode.InwardCircle)
			{
				startPos = centre + Random.insideUnitCircle * height * 0.5f;
				angle = Mathf.Atan2((centre - startPos).normalized.y, (centre - startPos).normalized.x);
			}
			else if (spawnMode == SpawnMode.RandomCircle)
			{
				startPos = centre + Random.insideUnitCircle * height * 0.15f;
				angle = randomAngle;
			}
			else if (spawnMode == SpawnMode.InwardCircle_Small)
			{
				startPos = centre + Random.insideUnitCircle * height * 0.15f;
				angle = Mathf.Atan2((centre - startPos).normalized.y, (centre - startPos).normalized.x);
			}
			agents[i].position = startPos;
			agents[i].angle = angle;
		}

		return agents;
	}
}
