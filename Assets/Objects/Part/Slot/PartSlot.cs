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
	public class PartSlot : MonoBehaviour
	{
        public Part part;

        public Socket[] sockets;

        public AnchorData anchor;
        [Serializable]
        public class AnchorData
        {
            public Element position;
            public Element angles;

            [Serializable]
            public class Element
            {
                public Vector3 value;

                public float speed;
            }
        }

        public bool isAligned;

        private void Start()
        {
            sockets = GetComponentsInChildren<Socket>();
        }

        private void Update()
        {
            if(isAligned)
            {

            }
            else
            {
                isAligned = CheckAlignment();

                if (isAligned)
                    Anchor();
            }
        }

        bool CheckAlignment()
        {
            for (int i = 0; i < sockets.Length; i++)
            {
                var targetSocket = part.Find(sockets[i].index);

                if (targetSocket == null) continue;

                if(sockets[i].InRange(targetSocket))
                {

                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        void Anchor()
        {
            part.rigidbody.isKinematic = true;

            part.transform.SetParent(transform);

            StartCoroutine(Procedure(part.transform, anchor.position, anchor.angles));
            IEnumerator Procedure(Transform target, AnchorData.Element position, AnchorData.Element angles)
            {
                var rotation = Quaternion.Euler(angles.value);

                while(true)
                {
                    target.localPosition = Vector3.MoveTowards(target.localPosition, position.value, position.speed * Time.deltaTime);
                    target.localRotation = Quaternion.RotateTowards(target.localRotation, rotation, angles.speed * Time.deltaTime);

                    if (target.localPosition == position.value && target.localRotation == rotation)
                        break;

                    yield return new WaitForEndOfFrame();
                }

                //target.localPosition = position.value;
                //target.localRotation = rotation;
            }
        }

        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < sockets.Length; i++)
            {
                var targetSocket = part.Find(sockets[i].index);

                if (targetSocket == null) continue;

                Debug.DrawLine(sockets[i].transform.position, targetSocket.transform.position, Socket.IndexToColor(targetSocket.index));
            }
        }
    }
}