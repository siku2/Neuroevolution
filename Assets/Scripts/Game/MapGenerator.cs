using UnityEngine;


namespace Neuroevolution.Game
{
	public class MapGenerator : MonoBehaviour
	{
		[HideInInspector] public int seed;

		[Tooltip("x: how far away from the currentPosition an object has to be in order to be deleted, y: how far ahead to build the map")]
		[SerializeField] Vector2 mapRange;
		[SerializeField] Transform obstacleParent;
		[SerializeField] Transform obstaclePrefab;
		[SerializeField] Transform floor;
		[SerializeField] float lineLength;

		[Header("Perlin Noise")]
		[SerializeField] int octaves;
		[Range(0f, 1f)]
		[SerializeField] float persistence;
		[Range(0f, 1f)]
		[SerializeField] float spawnTrigger;
		[SerializeField] float posScale;

		Pool<Transform> obstaclePool;
		float _currentPosition;
		float floorOffset;


		float farRight {
			get
			{
				return _currentPosition + mapRange.y;
			}
		}


		float farLeft {
			get
			{
				return _currentPosition - mapRange.x;
			}
		}


		Transform spawnObstacle()
		{
			Transform newObstacle = Instantiate<Transform>(obstaclePrefab, obstacleParent, false);

			return newObstacle;
		}


		void Awake()
		{
			obstaclePool = new Pool<Transform>(spawnObstacle);
			floor.localScale = new Vector3(mapRange.x + mapRange.y, floor.localScale.y, floor.localScale.z);

			floorOffset = (mapRange.x + mapRange.y) / 2;
		}


		void MoveFloor()
		{
			floor.position = new Vector3(farLeft + floorOffset, floor.position.y, floor.position.z);
		}


		void PlaceObstacles()
		{
			float value = Utils.octavePerlinNoise(farRight * posScale + seed, octaves, persistence);
			if(value >= spawnTrigger)
			{
				Transform newObstacle = obstaclePool.GrabItem();

				newObstacle.localPosition = new Vector3(farRight, 0, 0);
				newObstacle.gameObject.SetActive(true);
			}
		}


		void RemoveOldObstacles()
		{
			float minPosition = farLeft;

			for(int i = 0; i < obstaclePool.Size; i++)
			{
				if(!obstaclePool.freeItems.Contains(obstaclePool[i]) && minPosition > obstaclePool[i].position.x)
				{
//					Debug.Log("releasing");
					obstaclePool[i].gameObject.SetActive(false);
					obstaclePool.ReleaseItem(obstaclePool[i]);
				}
			}
		}


		public float currentPosition {
			get
			{
				return _currentPosition;
			}
			set
			{
				_currentPosition = value;

				#if UNITY_EDITOR
				Vector3 topLeft = new Vector3(_currentPosition - mapRange.x, lineLength / 2);
				Vector3 bottomLeft = new Vector3(_currentPosition - mapRange.x, -lineLength / 2);

				Vector3 topRight = new Vector3(_currentPosition + mapRange.y, lineLength / 2);
				Vector3 bottomRight = new Vector3(_currentPosition + mapRange.y, -lineLength / 2);

				Debug.DrawLine(topLeft, bottomLeft);
				Debug.DrawLine(topRight, bottomRight);
				#endif

				PlaceObstacles();
				RemoveOldObstacles();
				MoveFloor();
			}
		}


		public void reset()
		{
			for(int i = 0; i < obstaclePool.Size; i++)
			{
				obstaclePool[i].gameObject.SetActive(false);
			}

			obstaclePool.ReleaseAll();
			currentPosition = 0;

		}
	}
}