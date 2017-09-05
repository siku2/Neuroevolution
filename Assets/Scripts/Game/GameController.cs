using UnityEngine;
using UnityEngine.UI;


namespace Neuroevolution.Game
{
	public class GameController : MonoBehaviour
	{
		[SerializeField] Rodent rodentPrefab;
		[SerializeField] Transform rodentParent;
		[SerializeField] float moveSpeedModifier;
		[SerializeField] Text aliveDisplay;
		[SerializeField] Text fitnessDisplay;

		NeuralNetwork.Manager nnManager = new NeuralNetwork.Manager();
		NeuralNetwork.Network[] generationNetworks;
		Rodent[] rodents;

		float currentY = 0f;

		int generation = 0;
		int stillAlive;
		int currentFitness = 0;
		int highestFitness = 0;


		void Start()
		{
			StartGen();
		}


		void Update()
		{
			currentY += Time.deltaTime * moveSpeedModifier;

			for(int i = 0; i < rodents.Length; i++)
			{
				
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
				newRodent.transform.name = "Rodent " + i;

				rodents[i] = newRodent;
			}

			Debug.Log("Instantiated the Rodents");
		}


		void StartGen()
		{
			Debug.Log("Getting next generation");
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
		}
	}
}