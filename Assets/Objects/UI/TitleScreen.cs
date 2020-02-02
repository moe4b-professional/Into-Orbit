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
	public class TitleScreen : UIScreen
    {
        private void Start()
        {
            StartCoroutine(Procedure());
        }

        IEnumerator Procedure()
        {
            yield return WaitForInput();

            yield return Fade(0f);

            gameObject.SetActive(false);

            Level.Instance.Play();
        }

        IEnumerator WaitForInput()
        {
            while(true)
            {
                if (Input.anyKey)
                    break;

                yield return new WaitForEndOfFrame();
            }
        }
    }

    public class UIElement : MonoBehaviour
    {
        public bool IsOn
        {
            get => gameObject.activeInHierarchy;
            set => gameObject.SetActive(value);
        }
    }

    public class UIScreen : UIElement
    {
        public CanvasGroup CanvasGroup { get; protected set; }

        public float Alpha
        {
            get => CanvasGroup.alpha;
            set => CanvasGroup.alpha = value;
        }

        public float fadeSpeed = 2;

        protected virtual void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
            if (CanvasGroup == null) CanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        public Coroutine Fade(float target)
        {
            return StartCoroutine(Procedure());

            IEnumerator Procedure()
            {
                while (CanvasGroup.alpha != target)
                {
                    Alpha = Mathf.MoveTowards(Alpha, target, fadeSpeed * Time.deltaTime);

                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }
}