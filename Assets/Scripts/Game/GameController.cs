using UnityEngine;


namespace Neuroevolution.Game
{
	public class GameController : MonoBehaviour
	{
		[Header("Manager")]
		[SerializeField] CameraManager cameraManager;
		[SerializeField] MapGenerator mapGenerator;
		[SerializeField] UIManager uiManager;

		[Header("Game Settings")]
		[SerializeField] Rodent rodentPrefab;
		[SerializeField] Transform rodentParent;
		[SerializeField] Transform worldParent;
		[SerializeField] float moveSpeedModifier;
		[SerializeField] float zSpacing;
		[Range(0f, 1f)]
		[SerializeField] float jumpTrigger;
		[Range(0f, 50f)]
		[SerializeField] float timeScale;

		NeuralNetwork.Manager nnManager = new NeuralNetwork.Manager();
		NeuralNetwork.Network[] generationNetworks;
		Rodent[] rodents;

		Rodent _cameraTarget;


		Rodent cameraTarget {
			get
			{
				return _cameraTarget;
			}
			set
			{
				_cameraTarget = value;
				cameraManager.target = _cameraTarget.transform;
			}
		}


		int generation = 0;
		int stillAlive;
		int currentFitness = 0;
		int highestFitness = 0;

		bool runningSimulation = false;


		void OnValidate()
		{
			Time.timeScale = timeScale;
		}


		void Start()
		{
			StartGen();
		}


		void FixedUpdate()
		{
			if(!runningSimulation)
			{
				return;
			}

			float highestCurrentX = 0;

			for(int i = 0; i < rodents.Length; i++)
			{
				if(rodents[i].alive)
				{
					if(!cameraTarget.alive)
					{
						cameraTarget = rodents[i];
					}

					float beforeX = rodents[i].transform.position.x;
					highestCurrentX = (beforeX > highestCurrentX) ? beforeX : highestCurrentX;

					rodents[i].move(Time.fixedDeltaTime * moveSpeedModifier);

					float[] inputs = rodents[i].getInput();
					float[] result = generationNetworks[i].compute(inputs);

//					if(i == 0)
//					{
//						Debug.Log("<color=green>NN: " + string.Join(", ", System.Array.ConvertAll(result, val => val.ToString())) + "</color>");
//					}

					if(result[0] > jumpTrigger)
					{
						rodents[i].jump();
					}

					if(!rodents[i].alive)
					{
//						Debug.Log(i + " died");
						nnManager.score(generationNetworks[i], beforeX);
						stillAlive--;
					}
				}
			}

			mapGenerator.currentPosition = highestCurrentX;

			currentFitness = (int) highestCurrentX;
			highestFitness = (currentFitness > highestFitness) ? currentFitness : highestFitness;

			uiManager.fitness = Mathf.RoundToInt(currentFitness);
			uiManager.highestFitness = Mathf.RoundToInt(highestFitness);
			uiManager.alive = stillAlive;

			if(stillAlive <= 0)
			{
				Debug.Log("ALL DEAD!");
				runningSimulation = false;

				StartGen();
			}
		}


		void InstantiateRodents(int amount)
		{
			rodents = new Rodent[amount];
			Debug.Log("Instantiating " + amount + " Rodents");

			Vector3 oldScale = worldParent.transform.localScale;
			worldParent.transform.localScale = new Vector3(oldScale.x, oldScale.y, (amount + 1) * zSpacing);
			Vector3 oldPos = worldParent.transform.position;
			worldParent.transform.position = new Vector3(oldPos.x, oldPos.y, .5f * amount * zSpacing);

			for(int i = 0; i < amount; i++)
			{
				Rodent newRodent = Instantiate<Rodent>(rodentPrefab, Vector3.zero, Quaternion.identity, rodentParent);
				newRodent.index = i;

				newRodent.transform.position = new Vector3(0f, 0f, i * zSpacing);

				rodents[i] = newRodent;
			}

			Debug.Log("Instantiated the Rodents");

			cameraTarget = rodents[0];
		}


		void StartGen()
		{
			Debug.Log("Starting Generation " + generation);

			uiManager.generation = generation + 1;

			generationNetworks = nnManager.nextGenerationNetworks();

			if(rodents == null)
			{
				InstantiateRodents(generationNetworks.Length);
			}

			mapGenerator.reset();
			mapGenerator.seed = Random.Range(-10000, 10000);

			for(int i = 0; i < generationNetworks.Length; i++)
			{
				rodents[i].reset();
			}

			stillAlive = rodents.Length;
			generation++;

			runningSimulation = true;
		}
	}
}