using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
	[Header("References")]
	public Transform player;
	public GameObject pathSegmentPrefab;

	[Header("Path Settings")]
	public float segmentLength = 30f;
	public int segmentsAhead = 8;
	public float laneWidth = 3f;
	public float maxShiftPerSegment = 2f;

	[Header("Difficulty")]
	public float straightChance = 0.5f;

	private List<SegmentInfo> segments = new List<SegmentInfo>();
	private float nextSpawnZ = 0f;
	private float currentXOffset = 0f;

	private class SegmentInfo
	{
		public float zStart;
		public float zEnd;
		public float xOffset;
		public GameObject obj;
	}

	void Start()
	{
		for (int i = 0; i < segmentsAhead + 2; i++)
			SpawnSegment();
	}

	void Update()
	{
		if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;
		if (player == null) return;

		float playerZ = player.position.z;

		while (nextSpawnZ < playerZ + segmentsAhead * segmentLength)
			SpawnSegment();

		CleanupBehind(playerZ - 60f);
	}

	void SpawnSegment()
	{
		float shift = 0f;
		if (Random.value > straightChance)
		{
			float rawShift = Random.Range(-maxShiftPerSegment, maxShiftPerSegment);
			shift = Mathf.Round(rawShift);

			if (Mathf.Abs(currentXOffset + shift) > maxShiftPerSegment * 2)
				shift = -shift;
		}

		currentXOffset += shift * laneWidth;

		Vector3 spawnPos = new Vector3(
			currentXOffset,
			0f,
			nextSpawnZ + segmentLength / 2f
		);

		GameObject seg = Instantiate(pathSegmentPrefab, spawnPos, Quaternion.identity);
		seg.tag = "Path";
		seg.transform.SetParent(transform);

		SegmentInfo newSegment = new SegmentInfo();
		newSegment.zStart = nextSpawnZ;
		newSegment.zEnd = nextSpawnZ + segmentLength;
		newSegment.xOffset = currentXOffset;
		newSegment.obj = seg;

		segments.Add(newSegment);
		nextSpawnZ += segmentLength;
	}

	void CleanupBehind(float cutoffZ)
	{
		for (int i = segments.Count - 1; i >= 0; i--)
		{
			if (segments[i].zEnd < cutoffZ)
			{
				Destroy(segments[i].obj);
				segments.RemoveAt(i);
			}
		}
	}

	public float GetPathXAtZ(float z)
	{
		foreach (SegmentInfo seg in segments)
		{
			if (z >= seg.zStart && z < seg.zEnd)
				return seg.xOffset;
		}
		return 0f;
	}
}