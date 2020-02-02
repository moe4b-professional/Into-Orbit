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
#pragma warning disable CS0108
	public class Player : MonoBehaviour
    {
        public Rigidbody rigidbody { get; protected set; }

        public PlayerIndex index;

        public LayerMask mask = Physics.DefaultRaycastLayers;

        public InputProperty input;
        [Serializable]
        public class InputProperty
        {
            public Vector2 Move { get; protected set; }
            public Vector2 Look { get; protected set; }

            public DualActionInput PrimaryAction { get; protected set; }
            public DualActionInput SecondaryAction { get; protected set; }

            [Serializable]
            public class DualActionInput
            {
                public ButtonInput Button { get; protected set; }

                public float Axis { get; protected set; }

                public void Process(float axis, bool button)
                {
                    Button.Process(axis > 0f || button);

                    this.Axis = Mathf.Clamp01(axis + (button ? 1f : 0f));
                }

                public DualActionInput()
                {
                    Button = new ButtonInput();
                }
            }

            Player player;
            public void Init(Player reference)
            {
                player = reference;

                player.OnProcess += Process;

                PrimaryAction = new DualActionInput();
                SecondaryAction = new DualActionInput();
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

                PrimaryAction.Process(state.Triggers.Right, state.Buttons.RightShoulder == ButtonState.Pressed);
                SecondaryAction.Process(state.Triggers.Left, state.Buttons.LeftShoulder == ButtonState.Pressed);
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
                var input = player.input.Look;
                var sensitivity = this.sensitivity;

                player.weapon.laser.enabled = input.magnitude > 0f;

                if (input.magnitude == 0f && player.weapon.clutch.Input.Axis == 0f)
                {
                    input = player.input.Move;
                    sensitivity /= 2f;
                }

                if(player.weapon.clutch.joint != null)
                {

                }
                else if (input.magnitude > 0f)
                {
                    angle = Utility.Vector2Angle(input);

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

        public WeaponProperty weapon;
        [Serializable]
        public class WeaponProperty
        {
            public Transform transform;

            public LaunchProperty launch;
            [Serializable]
            public class LaunchProperty
            {
                public float range;

                public float radius;

                public float force;

                public float oxygenConsumption;

                public InputProperty.DualActionInput Input => player.input.PrimaryAction;

                RaycastHit[] hits;

                Player player;
                public void Init(Player reference)
                {
                    player = reference;

                    player.OnProcess += Process;
                }

                public Transform transform => player.weapon.transform;

                private void Process()
                {
                    if(Input.Button.Press)
                    {
                        Action();
                    }
                }

                void Action()
                {
                    if(player.oxygen.value >= oxygenConsumption)
                    {
                        player.oxygen.value -= oxygenConsumption;

                        player.rigidbody.velocity = Vector3.zero;
                        Action(player.rigidbody, -player.transform.forward, player.transform.position);

                        hits = Physics.SphereCastAll(transform.position, radius, transform.forward, range, player.mask);

                        for (int i = 0; i < hits.Length; i++)
                        {
                            Action(hits);
                        }
                    }
                }

                void Action(IList<RaycastHit> list)
                {
                    var hash = new HashSet<Rigidbody>();

                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].rigidbody == null) continue;

                        if (list[i].rigidbody == player.rigidbody) continue;

                        if (hash.Contains(list[i].rigidbody)) continue;

                        hash.Add(list[i].rigidbody);

                        Action(list[i].rigidbody, player.transform.forward, list[i].point);
                    }
                }

                void Action(Rigidbody rigidbody, Vector3 direction, Vector3 point)
                {
                    rigidbody.velocity = Vector3.one;

                    rigidbody.AddForceAtPosition(direction * force, point, ForceMode.VelocityChange);
                }

                public void OnDrawGizmos(Player player)
                {
#if UNITY_EDITOR
                    Gizmos.matrix = player.weapon.transform.localToWorldMatrix;
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireCube(Vector3.forward * range / 2f, new Vector3(radius, radius, range));
#endif
                }
            }

            public ClutchProperty clutch;
            [Serializable]
            public class ClutchProperty
            {
                public InputProperty.DualActionInput Input => player.input.SecondaryAction;

                public float range;

                public float radius;

                public SpringJoint joint;

                public Vector3 anchor;

                public float spring = 10;
                public float damper = 0.2f;
                public float tolerance = 0.025f;

                public float massScale = 40;
                public float connectedMassScale = 40;

                public Rigidbody target;

                RaycastHit hit;

                Player player;
                public void Init(Player reference)
                {
                    player = reference;

                    player.OnProcess += Process;
                }

                public Transform transform => player.weapon.transform;

                private void Process()
                {
                    if(Input.Button.Press)
                    {
                        Detect();
                    }
                    
                    if(Input.Button.Up)
                    {
                        Detatch();
                    }
                }

                void Detect()
                {
                    if(Physics.SphereCast(transform.position, radius, transform.forward, out hit, range, player.mask))
                    {
                        Connect(hit.rigidbody, hit.point);
                    }
                    else
                    {
                        
                    }
                }

                void Connect(Rigidbody rigidbody, Vector3 point)
                {
                    if(rigidbody == null)
                    {
                        
                    }
                    else
                    {
                        joint = target.gameObject.AddComponent<SpringJoint>();

                        joint.anchor = anchor;
                        joint.connectedBody = rigidbody;
                        joint.anchor = Vector3.zero;
                        joint.autoConfigureConnectedAnchor = false;
                        joint.connectedAnchor = rigidbody.transform.InverseTransformPoint(point);

                        joint.spring = spring;
                        joint.damper = damper;

                        joint.tolerance = tolerance;

                        joint.massScale = massScale;
                        joint.connectedMassScale = connectedMassScale;

                        joint.enableCollision = true;
                    }
                }

                void Detatch()
                {
                    Destroy(joint);
                }
            }

            public LaserProperty laser;
            [Serializable]
            public class LaserProperty
            {
                public bool enabled
                {
                    get => renderer.enabled;
                    set => renderer.enabled = value;
                }

                public LineRenderer renderer;

                RaycastHit hit;

                Player player;
                public void Init(Player reference)
                {
                    player = reference;

                    player.OnProcess += Process;

                    renderer.useWorldSpace = true;
                }

                void Process()
                {
                    if(enabled)
                    {
                        renderer.SetPosition(0, renderer.transform.position);

                        if (Physics.Raycast(renderer.transform.position, renderer.transform.forward, out hit, Mathf.Infinity, player.mask))
                        {
                            renderer.SetPosition(1, hit.point);
                        }
                        else
                        {
                            renderer.SetPosition(1, renderer.transform.position + renderer.transform.forward * 100f);
                        }
                    }
                    else
                    {

                    }
                }
            }

            Player player;
            public void Init(Player reference)
            {
                player = reference;

                clutch.Init(reference);
                launch.Init(reference);
                laser.Init(reference);
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
            weapon.Init(this);
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

        public event Action DrawGizmosEvent;
        private void OnDrawGizmos()
        {
            DrawGizmosEvent?.Invoke();

            weapon.launch.OnDrawGizmos(this);
        }
    }
#pragma warning restore CS0108
}