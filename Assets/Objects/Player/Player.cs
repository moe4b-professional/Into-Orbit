﻿using System;
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
#pragma warning disable CS0108
	public class Player : MonoBehaviour
    {
        public Rigidbody rigidbody { get; protected set; }

        public PlayerIndex index;

        public InputProperty input;
        [Serializable]
        public class InputProperty
        {
            public Vector2 Move { get; protected set; }
            public Vector2 Look { get; protected set; }

            public bool PrimaryAction { get; protected set; }
            public bool SecondaryAction { get; protected set; }

            Player player;
            public void Init(Player reference)
            {
                player = reference;

                player.OnProcess += Process;
            }

            void Process()
            {
                ProcessGamePad();
            }

            void ProcessGamePad()
            {
                var state = GamePad.GetState(player.index);

                Move = ThumbSticksToVector(state.ThumbSticks.Left);
                Look = ThumbSticksToVector(state.ThumbSticks.Right);

                PrimaryAction = state.Triggers.Right > 0f || state.Buttons.RightShoulder == ButtonState.Pressed;
                SecondaryAction = state.Triggers.Left > 0f || state.Buttons.LeftShoulder == ButtonState.Pressed;
            }

            public static Vector2 ThumbSticksToVector(GamePadThumbSticks.StickValue value)
            {
                return new Vector2(value.X, value.Y);
            }
        }

        public MovementProperty movement;
        [Serializable]
        public class MovementProperty
        {
            public float force;

            public Vector3 input;
            public Vector3 vector;

            public Vector3 right => Vector3.right;
            public Vector3 forward => Vector3.forward;

            public float oxygenConsumption;

            Player player;
            public void Init(Player reference)
            {
                player = reference;

                player.OnProcess += Process;
            }

            void Process()
            {
                input = forward * player.input.Move.y + right * player.input.Move.x;
                input = Vector3.ClampMagnitude(input, 1f);

                vector = input * force;

                if (player.oxygen.value <= 0f) vector = Vector3.zero;

                player.rigidbody.AddForce(vector, ForceMode.Acceleration);

                if(vector.magnitude > 0f)
                {
                    player.oxygen.value =
                        Mathf.MoveTowards(player.oxygen.value, 0f, oxygenConsumption * input.magnitude * Time.deltaTime);
                }
            }
        }

        public LookProperty look;
        [Serializable]
        public class LookProperty
        {
            public float angle;

            public float sensitivity;

            Player player;
            public void Init(Player reference)
            {
                player = reference;

                player.OnProcess += Process;
            }

            void Process()
            {
                if(player.input.Look.magnitude > 0f)
                {
                    angle = Utility.Vector2Angle(player.input.Look);

                    var angles = player.transform.localEulerAngles;

                    angles.y = Mathf.MoveTowardsAngle(angles.y, angle, sensitivity * Time.deltaTime);

                    player.transform.localEulerAngles = angles;
                }
            }
        }

        public OxygenProperty oxygen;
        [Serializable]
        public class OxygenProperty
        {
            [SerializeField]
            protected float _value;
            public float value
            {
                get => _value;
                set
                {
                    _value = value;

                    onValueChange.Invoke(value);
                }
            }

            public float maxValue;

            public float rate => value / maxValue;

            public FloatUnityEvent onValueChange;

            public RefillProperty refil;
            [Serializable]
            public class RefillProperty
            {
                public SpaceShipOxygenRefillZone zone;

                Player player;
                public void Init(Player reference)
                {
                    player = reference;

                    player.OnProcess += Process;

                    player.TriggerStayEnter += TriggerEnter;

                    player.TriggerStayExit += TriggerExit;
                }

                private void Process()
                {
                    if(zone != null)
                    {
                        if(zone.ship.oxygen.Supply > 0f && player.oxygen.rate < 1f)
                        {
                            var value = zone.speed * Time.deltaTime;

                            if (value > zone.ship.oxygen.Supply)
                                value = zone.ship.oxygen.Supply;

                            if (player.oxygen.value + value > player.oxygen.maxValue)
                                value = player.oxygen.maxValue - player.oxygen.value;

                            zone.ship.oxygen.Supply -= value;
                            player.oxygen.value += value;
                        }
                    }
                }

                public void TriggerEnter(Collider collider)
                {
                    var component = collider.GetComponent<SpaceShipOxygenRefillZone>();

                    if (component != null)
                    {
                        zone = component;
                    }
                }

                private void TriggerExit(Collider collider)
                {
                    var component = collider.GetComponent<SpaceShipOxygenRefillZone>();

                    if(component != null)
                    {
                        zone = null;
                    }
                }
            }

            Player player;
            public void Init(Player reference)
            {
                player = reference;

                refil.Init(reference);

                player.OnProcess += Process;
            }

            void Process()
            {
                
            }
        }

        public ThrustersProperty thrusters;
        [Serializable]
        public class ThrustersProperty
        {
            public Element[] elements;
            [Serializable]
            public class Element
            {
                public Thruster component;

                public Vector3 direction;
            }

            Player player;
            public void Init(Player reference)
            {
                player = reference;

                player.OnProcess += Process;
            }

            void Process()
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    var localInput = player.transform.InverseTransformDirection(player.movement.input);

                    var dot = Vector3.Dot(localInput, -elements[i].direction);

                    var rate = Mathf.InverseLerp(0, 1, dot);

                    if (player.oxygen.value <= 0f) rate = 0f;

                    elements[i].component.rate = rate;
                }
            }
        }

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();

            input.Init(this);
            movement.Init(this);
            look.Init(this);
            oxygen.Init(this);
            thrusters.Init(this);
        }

        public event Action OnProcess;
        private void Update()
        {
            OnProcess?.Invoke();
        }

        public event Action<Collider> TriggerStayEnter;
        private void OnTriggerEnter(Collider other)
        {
            TriggerStayEnter?.Invoke(other);
        }

        public event Action<Collider> TriggerStayExit;
        private void OnTriggerExit(Collider other)
        {
            TriggerStayExit?.Invoke(other);
        }
    }
#pragma warning restore CS0108
}