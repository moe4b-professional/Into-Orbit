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
    [DefaultExecutionOrder(-200)]
	public class Level : MonoBehaviour
	{
        public static Level Instance { get; protected set; }

        public TitleScreen titleScreen;
        public HUDScreen HUDScreen;

        public Player player1;
        public Player player2;

        public GameCamera gameCamera;

        public LevelState state = LevelState.Idle;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            titleScreen.IsOn = true;
        }

        public void Play()
        {
            state = LevelState.Playing;

            StartCoroutine(Procedure());

            IEnumerator Procedure()
            {
                gameCamera.PlayPanDown();

                yield return new WaitForSeconds(3.1f);

                HUDScreen.IsOn = true;
                HUDScreen.Alpha = 0f;
                HUDScreen.Fade(1f);
            }
        }
    }

    public enum LevelState
    {
        Playing, Idle
    }
}