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

using UnityEngine.Events;

namespace Game
{
	public class Utility
	{
        public static float Vector2Angle(Vector2 vector2)
        {
            return Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg;
        }
    }

    [Serializable]
    public class FloatUnityEvent : UnityEvent<float>
    {

    }
}