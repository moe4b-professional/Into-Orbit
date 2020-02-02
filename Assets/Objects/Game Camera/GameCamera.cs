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

        public Transform target1;
        public Transform target2;

        public Vector2 idleRange;

        Vector3 target;

        Animator animator;

        private void Start()
        {
            transform.position = GetCenterPosition();

            animator = GetComponent<Animator>();
        }

        private void LateUpdate()
        {
            var targetPosition = GetCenterPosition();

            var distance = Vector3.Distance(transform.position, targetPosition);

            var xDistance = Mathf.Abs(transform.position.x - targetPosition.x);
            var zDistance = Mathf.Abs(transform.position.z - targetPosition.z);

            if (xDistance > idleRange.x || zDistance > idleRange.y)
            {
                var rate = distance / ((idleRange.x + idleRange.y) / 2f);
                transform.position = Vector3.Lerp(transform.position, targetPosition, speed * rate * Time.deltaTime);
            }
        }

        public void PlayPanDown()
        {
            animator.Play("Pan Down");
        }

        Vector3 GetCenterPosition()
        {
            var direction = target1.position - target2.position;

            target = target1.position + -direction.normalized * direction.magnitude / 2f;

            target.y = transform.position.y;

            return target;
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var vec = new Vector3(1f, 0f, 1f);

            Handles.DrawWireCube(Vector3.Scale(transform.position, vec), new Vector3(idleRange.x, 0f, idleRange.y));

            Gizmos.DrawWireSphere(Vector3.Scale(target, vec), 0.2f);
#endif
        }

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