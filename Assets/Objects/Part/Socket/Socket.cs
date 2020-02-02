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
	public class Socket : MonoBehaviour
	{
        public int index => transform.GetSiblingIndex();

        public float radius = 2f;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IndexToColor(index);
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public float DistanceTo(Vector3 position)
        {
            return Vector3.Distance(transform.position, position);
        }

        public bool InRange(Socket target)
        {
            return DistanceTo(target.transform.position) <= this.radius + target.radius;
        }

        public static Color IndexToColor(int index)
        {
            switch (index)
            {
                case 0: return Color.magenta;
                case 1: return Color.blue;
                case 2: return Color.yellow;
                case 3: return Color.cyan;
                case 4: return Color.red;

                default: return Color.black;
            }

            throw new NotImplementedException();
        }
    }
}