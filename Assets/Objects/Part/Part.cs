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
    [RequireComponent(typeof(Rigidbody))]
	public class Part : MonoBehaviour
	{
        public Socket[] sockets;

        public Rigidbody rigidbody { get; protected set; }

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        public Socket Find(int index)
        {
            for (int i = 0; i < sockets.Length; i++)
                if (sockets[i].index == index)
                    return sockets[i];

            return null;
        }

        private void Start()
        {
            sockets = GetComponentsInChildren<Socket>();
        }
    }
}