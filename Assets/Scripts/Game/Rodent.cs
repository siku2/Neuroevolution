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

			RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.right, rayScanRange, obstacle.value);

			float obstacleInput = 2 * hit.distance / rayScanRange - 1;

			return new float[] { obstacleInput, transform.position.y / jumpForce };
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