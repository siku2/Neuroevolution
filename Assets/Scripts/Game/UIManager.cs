using UnityEngine;
using UnityEngine.UI;


namespace Neuroevolution.Game
{
	public class UIManager : MonoBehaviour
	{
		[HideInInspector] public int highestFitness = 0;

		[SerializeField] Text aliveDisplay;
		[SerializeField] Text fitnessDisplay;
		[SerializeField] Text generationDisplay;


		string aliveText = "{0} / {1} alive";
		string fitnessText = "Current Fitness: {0}\nHighest Fitness: {1}";
		string generationText = "Generation {0}";


		public int alive {
			set
			{
				aliveDisplay.text = string.Format(aliveText, value, Settings.populationSize);
			}
		}


		public int fitness {
			set
			{
				fitnessDisplay.text = string.Format(fitnessText, value, highestFitness);
			}
		}


		public int generation {
			set
			{
				generationDisplay.text = string.Format(generationText, value);
			}
		}
	}
}