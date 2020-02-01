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
	public class SpaceShipOxygenUI : MonoBehaviour
	{
        public SpaceShip target;

        public Slider slider;

        private void Start()
        {
            slider.minValue = 0f;
            slider.maxValue = 1f;

            UpdateState();

            target.oxygen.onSupplyChange.AddListener(ChangeCallback);
        }

        void ChangeCallback(float newValue)
        {
            UpdateState();
        }

        void UpdateState()
        {
            slider.value = target.oxygen.supplyRate;
        }
    }
}