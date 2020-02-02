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

using TMPro;

namespace Game
{
	public class LoseScreen : UIScreen
    {
        public TMP_Text reason;

        public void Show(string reason)
        {
            IsOn = true;

            Alpha = 0f;

            Fade(1f);

            this.reason.text = reason;
        }
    }
}