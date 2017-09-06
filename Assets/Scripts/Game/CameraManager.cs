using UnityEngine;


namespace Neuroevolution.Game
{
	public class CameraManager : MonoBehaviour
	{

		public Transform target;

		[SerializeField] Vector3 cameraOffset;


		void LateUpdate()
		{
			if(target == null)
				return;
		
			transform.position = new Vector3(target.position.x + cameraOffset.x, cameraOffset.y, cameraOffset.z);
		}
	}
}