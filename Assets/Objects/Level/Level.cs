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

        public SpaceShip ship;

        public GameCamera gameCamera;

        public WinScreen winScreen;

        public LoseScreen loseScreen;

        public LevelState state = LevelState.Idle;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            titleScreen.IsOn = true;

            ship.OnAllPartsAligned.AddListener(AllPartsAlignedCallback);

            player1.oxygen.onValueChange.AddListener(PlayerOxygenChange);
            player2.oxygen.onValueChange.AddListener(PlayerOxygenChange);

            ship.oxygen.onSupplyChange.AddListener(ShipOxygenChangeCallback);
        }

        private void Update()
        {
            if(Input.GetKey(KeyCode.L) && Application.isEditor)
            {
                Lose("Editor Ended Game");
            }
        }

        public void Play()
        {
            state = LevelState.Playing;

            StartCoroutine(Procedure());

            IEnumerator Procedure()
            {
                gameCamera.PlayPanDown();

                yield return new WaitForSeconds(2.3f);

                HUDScreen.IsOn = true;
                HUDScreen.Alpha = 0f;
                HUDScreen.Fade(1f);
            }
        }

        void AllPartsAlignedCallback()
        {
            Win();
        }

        void Win()
        {
            Debug.Log("Win");

            winScreen.Show();
        }

        void PlayerOxygenChange(float newValue)
        {
            if (player1.oxygen.value == 0f && player2.oxygen.value == 0f)
                Lose("Oxygen Supply Depleted");
        }
        void ShipOxygenChangeCallback(float newValue)
        {
            if (ship.oxygen.Supply == 0f)
                Lose("All Oxygen Lost");
        }

        void Lose(string reason)
        {
            loseScreen.Show(reason);
        }
    }

    public enum LevelState
    {
        Playing, Idle
    }
}