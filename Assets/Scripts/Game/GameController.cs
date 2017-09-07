using UnityEngine;
using UnityEngine.UI;


namespace Neuroevolution.Game
{
	public class GameController : MonoBehaviour
	{
		[SerializeField] CameraManager cameraManager;
		[SerializeField] MapGenerator mapGenerator;
		[SerializeField] Rodent rodentPrefab;
		[SerializeField] Transform rodentParent;
		[SerializeField] Transform floorParent;
		[SerializeField] float moveSpeedModifier;
		[SerializeField] float zSpacing;
		[Range(0f, 1f)]
		[SerializeField] float jumpTrigger;
		[SerializeField] float timeScale;

		[Header("UI")]
		[SerializeField] Text aliveDisplay;
		[SerializeField] Text fitnessDisplay;
		[SerializeField] Text generationDisplay;

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


		void Start()
		{
			Time.timeScale = timeScale;
			StartGen();
		}


		void Update()
		{
			UpdateUITexts();
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

			if(stillAlive <= 0)
			{
				Debug.Log("ALL DEAD!");
				runningSimulation = false;

				StartGen();
			}
		}


		void UpdateUITexts()
		{
			aliveDisplay.text = string.Format("{0}/{1}", stillAlive, rodents.Length);
			fitnessDisplay.text = string.Format("{0}/{1}", currentFitness, highestFitness);
		}


		void InstantiateRodents(int amount)
		{
			rodents = new Rodent[amount];
			Debug.Log("Instantiating " + amount + " Rodents");

			Vector3 oldScale = floorParent.transform.localScale;
			floorParent.transform.localScale = new Vector3(oldScale.x, oldScale.y, amount * zSpacing);
			Vector3 oldPos = floorParent.transform.position;
			floorParent.transform.position = new Vector3(oldPos.x, oldPos.y, .5f * amount * zSpacing);

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

			generationNetworks = nnManager.nextGenerationNetworks();

			if(rodents == null)
			{
				InstantiateRodents(generationNetworks.Length);
			}

			mapGenerator.reset();

			for(int i = 0; i < generationNetworks.Length; i++)
			{
				rodents[i].reset();
			}

			generationDisplay.text = generation.ToString();

			stillAlive = rodents.Length;
			generation++;

			runningSimulation = true;
		}
	}
}