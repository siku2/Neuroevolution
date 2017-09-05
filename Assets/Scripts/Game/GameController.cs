using UnityEngine;
using UnityEngine.UI;


namespace Neuroevolution.Game
{
	public class GameController : MonoBehaviour
	{
		[SerializeField] Rodent rodentPrefab;
		[SerializeField] Transform rodentParent;
		[SerializeField] float moveSpeedModifier;
		[Range(0f, 1f)]
		[SerializeField] float jumpTrigger;

		[Header("UI")]
		[SerializeField] Text aliveDisplay;
		[SerializeField] Text fitnessDisplay;

		NeuralNetwork.Manager nnManager = new NeuralNetwork.Manager();
		NeuralNetwork.Network[] generationNetworks;
		Rodent[] rodents;

		float currentX = 0f;

		int generation = 0;
		int stillAlive;
		int currentFitness = 0;
		int highestFitness = 0;

		bool runningSimulation = false;


		void Start()
		{
			StartGen();
		}


		void Update()
		{
			if(!runningSimulation)
			{
				return;
			}

			currentX += Time.deltaTime * moveSpeedModifier;

			for(int i = 0; i < rodents.Length; i++)
			{
				Vector3 oldPos = rodents[i].transform.position;
				rodents[i].transform.position = new Vector3(currentX, oldPos.y, oldPos.z);

				float[] inputs = rodents[i].getInput();
				float[] result = generationNetworks[i].compute(inputs);

				if(result[0] > jumpTrigger)
				{
					rodents[i].jump();
				}
			}

			UpdateUITexts();
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

			for(int i = 0; i < amount; i++)
			{
				Rodent newRodent = Instantiate<Rodent>(rodentPrefab, Vector3.zero, Quaternion.identity, rodentParent);
				newRodent.index = i;

				rodents[i] = newRodent;
			}

			Debug.Log("Instantiated the Rodents");
		}


		void StartGen()
		{
			Debug.Log("Starting Generation " + generation);

			generationNetworks = nnManager.nextGenerationNetworks();

			if(rodents == null)
			{
				InstantiateRodents(generationNetworks.Length);
			}

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