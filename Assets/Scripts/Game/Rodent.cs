using UnityEngine;


namespace Neuroevolution.Game
{
	public class Rodent : MonoBehaviour
	{
		int _index;


		public int index {
			get
			{
				return _index;
			}
			set
			{
				_index = value;
				transform.name = "Rodent " + _index;
			}
		}


		public bool alive = true;


		public float[] getInput()
		{
			return new float[] {0, 0, 0 ,0};
		}


		public void reset()
		{
			alive = true;
			gameObject.SetActive(true);
		}


		public void jump()
		{
			Debug.Log(index + " jumped");
			//TODO jump...
		}
	}
}