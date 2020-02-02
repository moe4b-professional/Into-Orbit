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
	public class SliderColorShift : MonoBehaviour
	{
        public Slider slider;

        public Graphic graphic;

        public Gradient gradient;

        private void Start()
        {
            slider.onValueChanged.AddListener(ValueChange);
        }

        void ValueChange(float newValue)
        {
            graphic.color = gradient.Evaluate(newValue);
        }
    }
}