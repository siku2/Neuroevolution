using UnityEngine;


namespace Neuroevolution.Game
{
	public class Rodent : MonoBehaviour
	{
		[SerializeField] Rigidbody2D rb;
		[SerializeField] MeshRenderer meshRenderer;
		[SerializeField] LayerMask obstacle;
		[SerializeField] LayerMask floorLayer;
		[SerializeField] float minJumpDelay;
		[SerializeField] float jumpForce;
		[SerializeField] float yInputDivider;
		[SerializeField] float rayScanRange;

		float nextJumpTime;
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
			//TODO don't hard-code height
			Vector2 startPos = new Vector2(transform.position.x, .5f);

			#if UNITY_EDITOR
			if(index == 0)
			{
				Debug.DrawRay(startPos, Vector2.right * rayScanRange, Color.blue);
			}
			#endif

			RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, Vector2.right, rayScanRange, obstacle.value);

			float[] inputs = new float[Settings.inputNeurons];

			for(int i = 0; i < Settings.inputNeurons - 1; i++)
			{
				if(i < hits.Length)
				{
					float obstacleValue = hits[i].distance / rayScanRange;
					inputs[i] = 1 - 2 * obstacleValue;
				}
				else
				{
					inputs[i] = -1;
				}

//				#if UNITY_EDITOR
//				if(index == 0)
//				{
//					Debug.Log(string.Format("Obstacle {0} = {1}", i, inputs[i]));
//				}
//				#endif
			}

			inputs[inputs.Length - 1] = transform.position.y / yInputDivider;

//			#if UNITY_EDITOR
//			if(index == 0)
//			{
//				Debug.Log(string.Format("Height = {0}", inputs[inputs.Length - 1]));
//			}
//			#endif
				
			return inputs;
		}


		public void reset()
		{
			alive = true;
			transform.position = new Vector3(0f, 0f, transform.position.z);

			rb.isKinematic = false;
			rb.WakeUp();
			meshRenderer.material.color = Random.ColorHSV(0, 1, 1, 1);

			gameObject.SetActive(true);
		}


		public void move(float amount)
		{
			transform.Translate(Vector3.right * amount);

			if(rb.IsTouchingLayers(obstacle.value))
			{
				alive = false;

				rb.isKinematic = true;
				rb.Sleep();
				meshRenderer.material.color = Color.red;
			}
		}


		public void jump()
		{
			if(Time.time > nextJumpTime && rb.IsTouchingLayers(floorLayer.value))
			{
				rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
			
				nextJumpTime = Time.time + minJumpDelay;
			}
		}
	}
}