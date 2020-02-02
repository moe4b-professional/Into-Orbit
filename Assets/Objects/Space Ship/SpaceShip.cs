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
	public class SpaceShip : MonoBehaviour
	{
        public OxygenProperty oxygen;
		[Serializable]
        public class OxygenProperty
        {
            [SerializeField]
            protected float _supply;
            public float Supply
            {
                get => _supply;
                set
                {
                    _supply = value;

                    onSupplyChange.Invoke(Supply);
                }
            }

            public float maximumSupply;

            public float supplyRate => Supply / maximumSupply;

            public FloatUnityEvent onSupplyChange;

            SpaceShip ship;
            public void Init(SpaceShip reference)
            {
                ship = reference;
            }

            public void Process()
            {

            }
        }

        public PartSlot[] slots;
        public bool AllPartsAligned
        {
            get
            {
                for (int i = 0; i < slots.Length; i++)
                    if (slots[i].isAligned == false) return false;

                return true;
            }
        }

        private void Start()
        {
            oxygen.Init(this);

            foreach (var slot in slots)
            {
                slot.OnAlign.AddListener(OnPartAlign);
            }
        }

        private void Update()
        {
            oxygen.Process();
        }

        public UnityEvent OnAllPartsAligned;
        void OnPartAlign()
        {
            if(AllPartsAligned)
            {
                OnAllPartsAligned.Invoke();
            }
        }
    }
}