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
	public class Thruster : MonoBehaviour
	{
        public ParticleSystem particles;

        public float minLifeTime;
        public float maxLifeTime = 0.5f;

		public float rate
        {
            set
            {
                var main = particles.main;

                main.startLifetime = Mathf.Lerp(minLifeTime, maxLifeTime, value);
            }
        }
	}
}