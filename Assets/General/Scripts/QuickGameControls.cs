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

using XInputDotNetPure;

namespace Game
{
	public class QuickGameControls : MonoBehaviour
	{
        List<PlayerIndex> indexes = new List<PlayerIndex>()
        {
            PlayerIndex.One, PlayerIndex.Two
        };

        public bool reload = false;

        public bool exit = false;

        public bool controls = false;
        public ControlsScreen controlsScreen;
        public GameObject guide;

        private void Update()
        {
            if(Check(CheckReload) && reload)
            {
                SceneManager.LoadScene(gameObject.scene.name);
            }

            if (Check(CheckExit) && exit)
            {
                Application.Quit();
            }

            if (Check(CheckControls) && controls)
            {
                guide.SetActive(false);

                controlsScreen.Alpha = Mathf.MoveTowards(controlsScreen.Alpha, 1f, controlsScreen.fadeSpeed * Time.deltaTime);
            }
            else
            {
                controlsScreen.Alpha = Mathf.MoveTowards(controlsScreen.Alpha, 0f, controlsScreen.fadeSpeed * Time.deltaTime);
            }
        }

        bool CheckReload(GamePadState state) => state.Buttons.A == ButtonState.Pressed;
        bool CheckExit(GamePadState state) => state.Buttons.B == ButtonState.Pressed;
        bool CheckControls(GamePadState state) => state.Buttons.Start == ButtonState.Pressed;

        bool Check(Func<GamePadState, bool> func)
        {
            foreach (var index in indexes)
            {
                var state = GamePad.GetState(index);

                if (func(state))
                    return true;
            }

            return false;
        }
    }
}