using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Game
{
	public class GameCamera : MonoBehaviour
	{
        public float speed = 10f;

		public Coroutine MoveTo(Transform target) => MoveTo(target.position);
        public Coroutine MoveTo(Vector3 position)
        {
            return StartCoroutine(Procedure(position));

            IEnumerator Procedure(Vector3 value)
            {
                while(true)
                {
                    transform.position = Vector3.MoveTowards(transform.position, value, speed * Time.deltaTime);

                    if (Vector3.Distance(transform.position, value) == 0f)
                        break;

                    yield return new WaitForEndOfFrame();
                }
            }
        }
	}
}