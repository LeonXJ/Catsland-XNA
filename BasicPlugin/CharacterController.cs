using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using Catsland.Core;
using System.ComponentModel;
using CatsEditor;
using Catsland.Editor;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics.Joints;

namespace Catsland.Plugin.BasicPlugin {
    public class CharacterController : CatComponent {
        /**
         * @brief Status describes if the cat is in stealth or normal
         * */
        public enum CatStatus {
            Normal,
            Stealth
        }
        /**
         * @brief Motion describes the motion the cat is performing
         * */
        public enum CatMotion {
            Moving,
            Defence,
            Attacking,
            Faint,
            Die
        }
        private CatStatus catStatus;
        private CatMotion catMotion;

        // cool-downs and intervals. the action will be avaliable when interval == 0
        #region
        private int m_talkInterval = 0;
        [CategoryAttribute("Talk"), DefaultValue(1000)]
        public int m_talkCooldown { get; set; }
        private int m_jumpInterval = 0;
        [CategoryAttribute("Movement"), DefaultValue(200)]
        public int m_jumpCooldown { get; set; }
        private int m_attackInterval = 0;
        private int m_attackCoolDown;
        [CategoryAttribute("Attack"), DefaultValue(500)]
        public int AttackCooldown {
            get { return m_attackCoolDown; }
            set { m_attackCoolDown = value; }
        }
        [CategoryAttribute("Attack"), DefaultValue(200)]
        public int attackTime { get; set; }

        private float m_stealthSpeed = 0.1f;
        [CategoryAttribute("Movement")]
        public float StealthMovingSpeed {
            get { return m_stealthSpeed; }
            set { m_stealthSpeed = value; }
        }

        private float m_walkingSpeed = 0.4f;
        [CategoryAttribute("Movement")]
        public float WalkingSpeed {
            get { return m_walkingSpeed; }
            set { m_walkingSpeed = value; }
        }

        private float m_runningSpeed = 1.0f;
        [CategoryAttribute("Movement")]
        public float RunningSpeed {
            get { return m_runningSpeed; }
            set { m_runningSpeed = value; }
        }

        private float m_damping;
        [CategoryAttribute("Movement")]
        public float Damping {
            get { return m_damping; }
            set { m_damping = value; }
        }

        private float m_gravity;
        [CategoryAttribute("Movement")]
        public float Gravity {
            get { return m_gravity; }
            set { m_gravity = value; }
        }
        #endregion

        // animations and generated objects
        #region
        private string meleePrefabName;
        [EditorAttribute(typeof(PropertyGridPrefabSelector),
         typeof(System.Drawing.Design.UITypeEditor))]
        public string MeleePrefabName {
            get { return meleePrefabName; }
            set { meleePrefabName = value; }
        }
        private string heavyMeleePrefabName;
        [EditorAttribute(typeof(PropertyGridPrefabSelector),
         typeof(System.Drawing.Design.UITypeEditor))]
        public string HeavyMeleePrefabName {
            get { return heavyMeleePrefabName; }
            set { heavyMeleePrefabName = value; }
        }
        private string blockPrefabName;
        [EditorAttribute(typeof(PropertyGridPrefabSelector),
         typeof(System.Drawing.Design.UITypeEditor))]
        public string BlockPrefabName {
            get { return blockPrefabName; }
            set { blockPrefabName = value; }
        }
        [EditorAttribute(typeof(PropertyGridAnimationSelector),
         typeof(System.Drawing.Design.UITypeEditor)), CategoryAttribute("Animation")]
        public string StandAnimationClipName { get; set; }
        [EditorAttribute(typeof(PropertyGridAnimationSelector),
         typeof(System.Drawing.Design.UITypeEditor)), CategoryAttribute("Animation")]
        public string StealthAnimationClipName { get; set; }
        [EditorAttribute(typeof(PropertyGridAnimationSelector),
         typeof(System.Drawing.Design.UITypeEditor)), CategoryAttribute("Animation")]
        public string WalkAnimationClipName { get; set; }
        [EditorAttribute(typeof(PropertyGridAnimationSelector),
         typeof(System.Drawing.Design.UITypeEditor)), CategoryAttribute("Animation")]
        public string RunningAnimationClipName { get; set; }
        [EditorAttribute(typeof(PropertyGridAnimationSelector),
         typeof(System.Drawing.Design.UITypeEditor)), CategoryAttribute("Animation")]
        public string JumpUpAnimationClipName { get; set; }
        [EditorAttribute(typeof(PropertyGridAnimationSelector),
         typeof(System.Drawing.Design.UITypeEditor)), CategoryAttribute("Animation")]
        public string FallDownAnimationClipName { get; set; }
        [EditorAttribute(typeof(PropertyGridAnimationSelector),
        typeof(System.Drawing.Design.UITypeEditor)), CategoryAttribute("Animation")]
        public string DefenceAnimationClipName { get; set; }
        [EditorAttribute(typeof(PropertyGridAnimationSelector),
         typeof(System.Drawing.Design.UITypeEditor)), CategoryAttribute("Animation")]
        public string MeleeAnimationClipName { get; set; }
        [EditorAttribute(typeof(PropertyGridAnimationSelector),
         typeof(System.Drawing.Design.UITypeEditor)), CategoryAttribute("Animation")]
        public string HurtAnimationClipName { get; set; }
        [EditorAttribute(typeof(PropertyGridAnimationSelector),
         typeof(System.Drawing.Design.UITypeEditor)), CategoryAttribute("Animation")]
        public string DieAnimationClipName { get; set; }
        #endregion

        // run-time variables
        #region
        // the time of performing a motion, will be set when starting the motion
        private int motionTicker = 0;
        // moving attribute
        public bool m_isRunning;
        private bool controlEnable = true;
        // wants
        public bool m_wantLeft;
        public bool m_wantRight;
        public bool m_wantUp;
        public bool m_wantDown;
        public bool m_wantJump;
        public bool m_wantRun;
        public bool m_wantAttack;
        public bool m_wantTalk;
        public bool m_wantDefence;
        // speed
        public Vector2 m_XYspeed;
        public float m_Zspeed;
        public bool m_isOnGround;
        public float jumpOffSpeedMag = 0.0f;
        #endregion

        Scene m_scene;
        Body m_body;
        Body m_wheel;
        Fixture m_fixture;
        RevoluteJoint m_axis;
        MotorJoint m_motor;

        int m_hit = 0;

        public CharacterController(GameObject gameObject)
            : base(gameObject) {
        }

        public CharacterController()
            : base() {

        }

        private void CreateAndConfigBody() {
            PhysicsSystem physicsSystem = m_scene.GetPhysicsSystem();
            if(physicsSystem != null){
                if (m_body != null) {
                    physicsSystem.GetWorld().RemoveBody(m_body);
                    m_body = null;
                }
                if (m_wheel != null) {
                    physicsSystem.GetWorld().RemoveBody(m_wheel);
                    m_wheel = null;
                }

                m_body = BodyFactory.CreateRectangle(physicsSystem.GetWorld(),
                                0.1f, 0.1f, 1.0f);
                m_body.BodyType = BodyType.Dynamic;
                m_body.Position = new Vector2(0.0f, 0.0f);
                m_body.SleepingAllowed = false;
                m_body.FixedRotation = true;

                m_wheel = BodyFactory.CreateCircle(physicsSystem.GetWorld(),
                                0.08f, 1.0f);
                m_wheel.Position = new Vector2(0.0f, 0.0f);
                m_wheel.BodyType = BodyType.Dynamic;
                m_wheel.SleepingAllowed = false;
                
                m_wheel.Friction = 1.0f;

                m_axis = JointFactory.CreateRevoluteJoint(physicsSystem.GetWorld(),
                            m_wheel, m_body, m_wheel.WorldCenter);
//                     JointFactory.CreateRevoluteJoint(physicsSystem.GetWorld(),
//                             m_wheel, m_body, new Vector2(0.0f, -0.05f));

                m_axis.CollideConnected = false;
                m_axis.MotorEnabled = true;
               /* m_axis.Frequency = 4.0f;*/
                m_axis.MaxMotorTorque = 40.0f;
                m_axis.MotorSpeed = .0f;
                
                //m_axis.DampingRatio = 0.01f;
                

                //m_motor = JointFactory.CreateMotorJoint(physicsSystem.GetWorld(),
                //            m_wheel, m_body);
                //m_motor.CollideConnected = false;
                

//                 m_body = BodyFactory.CreateCircle(physicsSystem.GetWorld(),
//                                                   0.1f,
//                                                   1.0f);
//                 m_body.BodyType = BodyType.Dynamic;
//                 m_body.Position = new Vector2(0.0f, -0.05f);
//                 m_body.SleepingAllowed = false;
// 
//                 Body body = BodyFactory.CreateRectangle(physicsSystem.GetWorld(),
//                                                   0.2f,
//                                                   0.1f,
//                                                   0.1f);
//                 body.BodyType = BodyType.Dynamic;
//                 body.SleepingAllowed = false;
//                 //JointFactory.CreateFixedMouseJoint(physicsSystem.GetWorld(), body, Vector2.Zero);
//                 body.FixedRotation = true;
//                 m_body.Friction = 0.8f;
//                 m_axis = JointFactory.CreateRevoluteJoint(physicsSystem.GetWorld(),
//                                                  m_body, body, Vector2.Zero);
//                 //JointFactory.CreateAngleJoint()
//                 m_axis.CollideConnected = false;
//                 m_axis.MotorEnabled = true;
//                 m_axis.MotorSpeed = 0;
                
                //m_axis.MaxMotorTorque = 10000;
//                 m_body = BodyFactory.CreateCapsule(physicsSystem.GetWorld(),
//                                                    0.5f,
//                                                    0.1f,
//                                                    1.0f);

//                 m_body = BodyFactory.CreateBody(physicsSystem.GetWorld());
//                 CircleShape shape = new CircleShape(0.1f, 0.1f);
//                 m_body.BodyType = BodyType.Dynamic;
//                 m_body.SleepingAllowed = false;
//                 m_fixture = m_body.CreateFixture(shape);
//                 m_body.Mass = 1.0f;
                
//                 m_body.BodyType = BodyType.Dynamic;
//                 m_fixture.IsSensor = true;

//                 physicsSystem.GetWorld().ContactManager.BeginContact += BeginContact;
//                 physicsSystem.GetWorld().ContactManager.EndContact += EndContact;
                m_body.OnCollision += OnCollison;
                m_body.OnSeparation += OnSeparater;
                

//                 m_body = BodyFactory.CreateRectangle(physicsSystem.GetWorld(),
//                                                      0.1f,
//                                                      0.1f,
//                                                      0.1f);
//                 physicsSystem.GetWorld().ContactManager.BeginContact += 
//                 m_body.BodyType = BodyType.Static;
                //m_body.FixedRotation = true;
                
                // TODO: config body
            }
        }
        

        public override void Initialize(Scene scene) {
            base.Initialize(scene);

            m_scene = scene;
            m_wantLeft = false;
            m_wantRight = false;
            m_wantUp = false;
            m_wantDown = false;
            m_wantJump = false;
            m_wantRun = false;

            m_isRunning = false;
            m_isOnGround = true;
            m_XYspeed = new Vector2(0.0f, 0.0f);

            catStatus = CatStatus.Normal;
            catMotion = CatMotion.Moving;

            m_wantTalk = false;

            CreateAndConfigBody();
        }


        public override void Update(int timeLastFrame) {
            if (Enable == false) {
                return;
            }
            base.Update(timeLastFrame);
            float timeLastFrameSecond = timeLastFrame / 1000.0f;
#region 

            // save old status
//             bool old_isOnGround = m_isOnGround;
//             // prepare component references
//             Animator animator = (Animator)m_gameObject.GetComponent(typeof(Animator).Name);
//             CatsCollider collider = (CatsCollider)m_gameObject.GetComponent(typeof(CatsCollider).Name);
//             // set control
//             Vector2 desiredSpeedXY = Vector2.Zero;
//             if (controlEnable) {
//                 // get moving direction
//                 #region
//                 Vector2 wantXYDirection = new Vector2(0.0f, 0.0f);
//                 if (m_wantLeft) {
//                     wantXYDirection += new Vector2(-1.0f, 0.0f);
//                 }
//                 if (m_wantRight) {
//                     wantXYDirection += new Vector2(1.0f, 0.0f);
//                 }
//                 if (m_wantUp) {
//                     wantXYDirection += new Vector2(0.0f, 1.0f);
//                 }
//                 if (m_wantDown) {
//                     wantXYDirection += new Vector2(0.0f, -1.0f);
//                 }
//                 if (wantXYDirection.LengthSquared() > 0.0f) {
//                     wantXYDirection.Normalize();
//                 }
//                 #endregion
//                 // perform different actions according to air/ground and status
//                 if (m_isOnGround) {
//                     // horizontal velocity
//                     #region
//                     if (catMotion == CatMotion.Moving) {
//                         m_isRunning = m_wantRun;    // want run?
//                         float velocity = 0.0f;
//                         if (m_isRunning) {
//                             velocity = m_runningSpeed;
//                             // if run, exit stealth mode
//                             if (catStatus == CatStatus.Stealth) {
//                                 InvertCatStatus();
//                             }
//                         }
//                         else {
//                             if (catStatus == CatStatus.Normal) {
//                                 velocity = m_walkingSpeed;
//                             }
//                             else {
//                                 velocity = m_stealthSpeed;
//                             }
//                         }
//                         desiredSpeedXY = wantXYDirection * velocity;
//                     }
// 
//                     #endregion
//                     // jumping
//                     #region
//                     if (m_jumpInterval > 0) {
//                         m_jumpInterval -= timeLastFrame;
//                     }
//                     if (catMotion == CatMotion.Moving && m_wantJump && m_jumpInterval <= 0) {
//                         m_Zspeed += 3.0f;
//                         jumpOffSpeedMag = m_XYspeed.Length();
//                         m_jumpInterval = m_jumpCooldown;
//                     }
//                     #endregion
//                     // other effect: attack and talk
//                     #region
//                     if (m_talkInterval > 0) {
//                         m_talkInterval -= timeLastFrame;
//                     }
//                     if (m_attackInterval > 0) {
//                         m_attackInterval -= timeLastFrame;
//                     }
// 
//                     // defence has higher priority
//                     if (catMotion == CatMotion.Moving && m_wantDefence) {
//                         catMotion = CatMotion.Defence;
//                     }
//                     else if (catMotion == CatMotion.Moving && m_wantAttack && m_attackInterval <= 0) {
//                         m_attackInterval = m_attackCoolDown;
// 
//                         catMotion = CatMotion.Attacking;
//                         motionTicker = attackTime;
// 
//                         // give different attack according to the speed
//                         if (m_XYspeed.LengthSquared() > m_walkingSpeed * m_walkingSpeed) {
//                             // heavy melee
//                             GameObject prefab = null;
//                             if (heavyMeleePrefabName != null) {
//                                 prefab = Mgr<CatProject>.Singleton.prefabList.GetItem(heavyMeleePrefabName);
//                             }
//                             if (prefab != null) {
//                                 GameObject attackObject = prefab.CloneGameObject();
//                                 attackObject.PositionOld = m_gameObject.PositionOld +
//                                     (animator.Mirror ? 1.0f : -1.0f) * new Vector2(0.2f, 0.0f);
//                                 attackObject.HeightOld = m_gameObject.HeightOld + 0.2f;
//                                 // set information
//                                 CatsCollider attackCollider = (CatsCollider)attackObject.GetComponent(typeof(CatsCollider).Name);
//                                 if (attackCollider != null &&
//                                     attackCollider.TriggerInvoker != null) {
//                                     CatComponent attackInvoker = attackCollider.m_gameObject.GetComponent(attackCollider.TriggerInvoker);
//                                     if (attackInvoker is AttackInvoke) {
//                                         ((AttackInvoke)attackInvoker).SetOwner(m_gameObject);
//                                     }
//                                 }
//                                 QuadRender quadRender = (QuadRender)attackObject.GetComponent(typeof(QuadRender).Name);
//                                 if (quadRender != null) {
//                                     quadRender.XMirror = animator.Mirror;
//                                 }
//                                 // add to gameobject list
//                                 Mgr<Scene>.Singleton._gameObjectList.AddItem(attackObject.GUID, attackObject);
//                             }
//                         }
//                         else {
//                             // light melee
//                             GameObject prefab = null;
//                             if (meleePrefabName != null) {
//                                 prefab = Mgr<CatProject>.Singleton.prefabList.GetItem(meleePrefabName);
//                             }
//                             if (prefab != null) {
//                                 GameObject attackObject = prefab.CloneGameObject();
//                                 attackObject.PositionOld = m_gameObject.PositionOld +
//                                     (animator.Mirror ? 1.0f : -1.0f) * new Vector2(0.2f, 0.0f);
//                                 attackObject.HeightOld = m_gameObject.HeightOld + 0.2f;
//                                 // set information
//                                 CatsCollider attackCollider = (CatsCollider)attackObject.GetComponent(typeof(CatsCollider).Name);
//                                 if (attackCollider != null &&
//                                     attackCollider.TriggerInvoker != null) {
//                                     CatComponent attackInvoker = attackCollider.m_gameObject.GetComponent(attackCollider.TriggerInvoker);
//                                     if (attackInvoker is AttackInvoke) {
//                                         ((AttackInvoke)attackInvoker).SetOwner(m_gameObject);
//                                     }
//                                 }
//                                 QuadRender quadRender = (QuadRender)attackObject.GetComponent(typeof(QuadRender).Name);
//                                 if (quadRender != null) {
//                                     quadRender.XMirror = animator.Mirror;
//                                 }
//                                 // add to gameobject list
//                                 Mgr<Scene>.Singleton._gameObjectList.AddItem(attackObject.GUID, attackObject);
//                             }
// 
//                         }
//                     }
//                     /*
//                     if (catMotion == CatMotion.Moving && m_wantTalk && m_talkInterval <= 0) {
//                         m_talkInterval = m_talkCooldown;
//                         GameObject talkMessage = new GameObject();
//                         talkMessage.Position = m_gameObject.Position +
//                             (animator.Mirror ? 1.0f : -1.0f) * new Vector2(0.2f, 0.0f);
// 
//                         Collider detector = new Collider(talkMessage);
//                         detector.AdvantageTrigger = false;
//                         detector.ColliderTypeAttribute = Collider.ColliderType.PositiveTrigger;
//                         CatsBoundingBox boundingBox = new CatsBoundingBox();
//                         boundingBox.m_XBound = new Vector2(-0.1f, 0.1f);
//                         boundingBox.m_YBound = new Vector2(-0.1f, 0.1f);
//                         boundingBox.m_heightRange = new Vector2(0.0f, 0.2f);
//                         boundingBox.UpdateBoundFromRectangle();
//                         detector.m_rootBounding = boundingBox;
//                         DialogTrigger dialogTrigger = new DialogTrigger();
//                         dialogTrigger.SetOwner(m_gameObject);
//                         detector.m_triagerInvoker = dialogTrigger;
// 
//                         talkMessage.AddComponent(typeof(Collider).Name, detector);
//                         //talkMessage.Initialize(Mgr<Scene>.Singleton);
//                         Mgr<Scene>.Singleton._gameObjectList.AddItem(talkMessage._guid, talkMessage);
// 
//                         SelfDestroyer selfDestroyer = new SelfDestroyer(talkMessage);
//                         selfDestroyer.m_time = 500;
//                         talkMessage.AddComponent(typeof(SelfDestroyer).Name, selfDestroyer);
//                         //selfDestroyer.Initialize(Mgr<Scene>.Singleton);
//                     }
//                     */
//                     #endregion
// 
//                 }
//                 else {  // in the air
//                     if (catStatus == CatStatus.Normal) {
//                         desiredSpeedXY = wantXYDirection * m_walkingSpeed;
//                     }
//                     else if (catStatus == CatStatus.Stealth) {
//                         desiredSpeedXY = wantXYDirection * m_stealthSpeed;
//                     }
// 
//                 }
// 
//             }
//             // non-control motion
//             // apply gravity
//             m_Zspeed += m_gravity * timeLastFrameSecond;
//             // calculate destination
//             if (m_isOnGround) {
//                 m_XYspeed = (m_damping) * m_XYspeed + desiredSpeedXY * (1.0f - m_damping);
//             }
//             // keep original speed if in the air but apply the adjustment to displacement
//             Vector2 destination = m_gameObject.PositionOld + m_XYspeed * timeLastFrameSecond;
//             if (!m_isOnGround) {
//                 destination += desiredSpeedXY * timeLastFrameSecond;
//             }
//             float destination_height = m_gameObject.HeightOld + m_Zspeed * timeLastFrameSecond;
//             // check whether destination available
//             #region
//             if (collider != null) {
//                 // 1) first try to move to destination horizontally
//                 Collider result = Mgr<Scene>.Singleton._colliderList.JudgeCollide(collider.m_collider.m_rootBounding,
//                     destination, m_gameObject.HeightOld, collider.m_collider);
//                 // if collide with something, do not move to destination, but will check whether it will fall
//                 if (result != null) {
//                     destination = m_gameObject.PositionOld;
//                 }
//                 // 2) check whether it will fall
//                 result = Mgr<Scene>.Singleton._colliderList.JudgeCollide(collider.m_collider.m_rootBounding,
//                     destination, destination_height, collider.m_collider);
//                 // solid ground or solid on top, do not move vertically
//                 if (result != null) {
//                     destination_height = m_gameObject.HeightOld;
//                     // check whether it collide on top or on bottom
//                     if (m_Zspeed > 0.0f) {
//                         // top collide
//                         m_isOnGround = false;
//                     }
//                     else {
//                         // bottom collide
//                         m_isOnGround = true;
//                     }
//                     m_Zspeed = 0.0f;
//                 }
//                 // not thing on vertical destination, move to it
//                 else {
//                     m_isOnGround = false;
//                 }
//             }
//             // scene boundary check
//             if (destination_height < 0.0f) {
//                 destination_height = 0.0f;
//                 m_isOnGround = true;
//                 m_Zspeed = 0.0f;
//             }
//             Vector3 destination_pos3 = Mgr<Scene>.Singleton.GetInBoundPosition(destination, destination_height);
//             #endregion
//             // apply position
//             m_gameObject.PositionOld = new Vector2(destination_pos3.X, destination_pos3.Y);
//             m_gameObject.HeightOld = destination_pos3.Z;
//             // other status update, ticking
//             if (catMotion == CatMotion.Attacking) {
//                 if (motionTicker > 0) {
//                     motionTicker -= timeLastFrame;
//                 }
//                 if (motionTicker <= 0) {
//                     catMotion = CatMotion.Moving;
//                 }
//             }
//             else if (catMotion == CatMotion.Faint) {
//                 if (motionTicker > 0) {
//                     motionTicker -= timeLastFrame;
//                 }
//                 if (motionTicker <= 0) {
//                     catMotion = CatMotion.Moving;
//                 }
//             }
//             else if (catMotion == CatMotion.Defence) {
//                 if (!m_wantDefence) {
//                     catMotion = CatMotion.Moving;
//                 }
//             }
// 
//             // apply animation
//             if (animator != null) {
//                 if (catMotion == CatMotion.Moving || catMotion == CatMotion.Attacking) {
//                     // left or right
//                     if (m_XYspeed.X < -0.01f) {
//                         animator.Mirror = false;
//                     }
//                     if (m_XYspeed.X > 0.01f) {
//                         animator.Mirror = true;
//                     }
//                 }
//                 if (catMotion == CatMotion.Moving) {
//                     if (m_isOnGround) {
//                         float walkRunThreshold = (m_walkingSpeed + m_runningSpeed) * 0.5f;
//                         float standWalkThreshold = m_walkingSpeed * 0.5f;
//                         float stealthWalkThreshold = m_stealthSpeed * 0.5f;
//                         if (m_XYspeed.LengthSquared() > walkRunThreshold * walkRunThreshold) {
//                             animator.fadeToAnimation(RunningAnimationClipName);
//                         }
//                         else if (catStatus == CatStatus.Normal
//                             && m_XYspeed.LengthSquared() > standWalkThreshold * standWalkThreshold) {
//                             animator.fadeToAnimation(WalkAnimationClipName);
//                         }
//                         else if (catStatus == CatStatus.Stealth
//                             && m_XYspeed.LengthSquared() > stealthWalkThreshold * stealthWalkThreshold) {
//                             // stealth mode
//                             animator.fadeToAnimation(StealthAnimationClipName);
//                         }
//                         else {
//                             animator.fadeToAnimation(StandAnimationClipName);
//                         }
//                     }
//                     else {
//                         // animation clip
//                         if (m_Zspeed > 0.05f) {
//                             animator.CheckPlayAnimation(JumpUpAnimationClipName);
//                         }
//                         else if (m_Zspeed < -0.5f) {
//                             animator.CheckPlayAnimation(FallDownAnimationClipName);
//                         }
//                     }
//                 }
//                 else if (catMotion == CatMotion.Defence) {
//                     animator.CheckPlayAnimation(DefenceAnimationClipName);
//                 }
//                 else if (catMotion == CatMotion.Attacking) {
//                     animator.CheckPlayAnimation(MeleeAnimationClipName);
//                 }
//                 else if (catMotion == CatMotion.Faint) {
//                     animator.CheckPlayAnimation(HurtAnimationClipName);
//                 }
//                 else if (catMotion == CatMotion.Die) {
//                     animator.CheckPlayAnimation(DieAnimationClipName);
//                 }
            //             }
#endregion

            // under feet
//             if(m_hit == 0)
//                 m_body.Position -= new Vector2(0.0f, 0.01f);
           // if (m_hit > 0){
                if (m_wantJump) {
                    m_body.ApplyForce(new Vector2(0.0f, 2f));
                }
                if (m_wantLeft) {
                    //m_body.Position -= new Vector2(0.01f, 0.0f);
                    //m_axis.MotorImpulse = -40f;
                    m_axis.MotorSpeed = -40f;
                    //m_body.ApplyForce(new Vector2(-0.05f, 0.0f));
                }
                else if (m_wantRight) {
                    //m_body.Position += new Vector2(0.01f, 0.0f);
                    m_axis.MotorSpeed = 40f;
                    //m_body.ApplyForce(new Vector2(0.05f, 0.0f));
                }
                else {
                    m_axis.MotorSpeed *= 0.6f;
                }
            //}

            
            MoveGameObjectToBody();
//            m_scene.GetPhysicsSystem().GetWorld()
            

        }

        private bool OnCollison(Fixture a, Fixture b, Contact c) {
            m_hit++;
            
            return true;
        }

        private void OnSeparater(Fixture a, Fixture b) {
            m_hit--;
        }

        protected  bool BeginContact(Contact contact) {
            if (contact.FixtureA == m_fixture || contact.FixtureB == m_fixture ) {
                m_hit ++;
            }
            return true;
        }

        protected  void EndContact(Contact contact) {
            if (contact.FixtureA == m_fixture || contact.FixtureB == m_fixture) {
                m_hit --;
            }
        }

        private void MoveBodyToGameObject() {
            m_body.SetTransform(new Vector2(m_gameObject.Position.X,
                                                m_gameObject.Position.Y),
                                        MathHelper.ToRadians(m_gameObject.Rotation.Z));
        }

        private void MoveGameObjectToBody() {
            m_gameObject.Position = new Vector3(m_body.Position.X,
                                                        m_body.Position.Y,
                                                        m_gameObject.Position.Z);
            m_gameObject.Rotation = new Vector3(m_gameObject.Rotation.X,
                                                m_gameObject.Rotation.Y,
                                                MathHelper.ToDegrees(m_body.Rotation));
        }

        public override bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement characterController = doc.CreateElement("CharacterController");
            node.AppendChild(characterController);
            // cool-downs and intervals
            #region 
            characterController.SetAttribute("talkCooldown", "" + m_talkCooldown);
            characterController.SetAttribute("jumpCooldown", "" + m_jumpCooldown);
            characterController.SetAttribute("attackCooldown", "" + m_attackCoolDown);
            characterController.SetAttribute("attackTime", "" + attackTime);
            #endregion
            // movements
            #region 
            characterController.SetAttribute("stealthSpeed", "" + m_stealthSpeed);
            characterController.SetAttribute("walkingSpeed", "" + m_walkingSpeed);
            characterController.SetAttribute("runningSpeed", "" + m_runningSpeed);
            characterController.SetAttribute("damping", "" + m_damping);
            characterController.SetAttribute("gravity", "" + m_gravity);
            #endregion
            // animations
            #region
            characterController.SetAttribute("meleePrefabName", meleePrefabName);
            characterController.SetAttribute("heavyMeleePrefabName", heavyMeleePrefabName);
            characterController.SetAttribute("blockPrefabName", BlockPrefabName);

            characterController.SetAttribute("standAnimationClipName", StandAnimationClipName);
            characterController.SetAttribute("stealthAnimationClipName", StealthAnimationClipName);
            characterController.SetAttribute("walkAnimationClipName", WalkAnimationClipName);
            characterController.SetAttribute("runningAnimationClipName", RunningAnimationClipName);
            characterController.SetAttribute("jumpUpAnimationClipName", JumpUpAnimationClipName);
            characterController.SetAttribute("fallDownAnimationClipName", FallDownAnimationClipName);
            characterController.SetAttribute("defenceAnimationClipName", DefenceAnimationClipName);
            characterController.SetAttribute("meleeAnimationClipName", MeleeAnimationClipName);
            characterController.SetAttribute("hurtAnimationClipName", HurtAnimationClipName);
            characterController.SetAttribute("dieAnimationClipName", DieAnimationClipName);
            #endregion     
            return true;
        }

        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject) {
            // cool-downs and intervals
            #region
            m_talkCooldown = int.Parse(node.GetAttribute("talkCooldown"));
            m_jumpCooldown = int.Parse(node.GetAttribute("jumpCooldown"));
            m_attackCoolDown = int.Parse(node.GetAttribute("attackCooldown"));
            attackTime = int.Parse(node.GetAttribute("attackTime"));
            #endregion
            // movements
            #region
            m_stealthSpeed = float.Parse(node.GetAttribute("stealthSpeed"));
            m_walkingSpeed = float.Parse(node.GetAttribute("walkingSpeed"));
            m_runningSpeed = float.Parse(node.GetAttribute("runningSpeed"));
            m_damping = float.Parse(node.GetAttribute("damping"));
            m_gravity = float.Parse(node.GetAttribute("gravity"));
            #endregion
            // animations
            #region
            meleePrefabName = node.GetAttribute("meleePrefabName");
            heavyMeleePrefabName = node.GetAttribute("heavyMeleePrefabName");
            BlockPrefabName = node.GetAttribute("blockPrefabName");

            StandAnimationClipName = node.GetAttribute("standAnimationClipName");
            StealthAnimationClipName = node.GetAttribute("stealthAnimationClipName");
            WalkAnimationClipName = node.GetAttribute("walkAnimationClipName");
            RunningAnimationClipName = node.GetAttribute("runningAnimationClipName");
            JumpUpAnimationClipName = node.GetAttribute("jumpUpAnimationClipName");
            FallDownAnimationClipName = node.GetAttribute("fallDownAnimationClipName");
            DefenceAnimationClipName = node.GetAttribute("defenceAnimationClipName");
            MeleeAnimationClipName = node.GetAttribute("meleeAnimationClipName");
            HurtAnimationClipName = node.GetAttribute("hurtAnimationClipName");
            DieAnimationClipName = node.GetAttribute("dieAnimationClipName");
            #endregion     
            return;
        }

        public override CatComponent CloneComponent(GameObject gameObject) {
            CharacterController characterController = new CharacterController(gameObject);
            
            #region
            characterController.m_talkCooldown = m_talkCooldown;
            characterController.m_jumpCooldown = m_jumpCooldown;
            characterController.m_attackCoolDown = m_attackCoolDown;
            characterController.attackTime = attackTime;
            #endregion
            // movements
            #region
            characterController.m_stealthSpeed = m_stealthSpeed;
            characterController.m_walkingSpeed = m_walkingSpeed;
            characterController.m_runningSpeed = m_runningSpeed;
            characterController.m_damping = m_damping;
            characterController.m_gravity = m_gravity;
            #endregion
            // animations
            #region
            characterController.meleePrefabName = meleePrefabName;
            characterController.heavyMeleePrefabName = heavyMeleePrefabName;
            characterController.BlockPrefabName = BlockPrefabName;

            characterController.StandAnimationClipName = StandAnimationClipName;
            characterController.StealthAnimationClipName = StealthAnimationClipName;
            characterController.WalkAnimationClipName = WalkAnimationClipName;
            characterController.RunningAnimationClipName = RunningAnimationClipName;
            characterController.JumpUpAnimationClipName = JumpUpAnimationClipName;
            characterController.FallDownAnimationClipName = FallDownAnimationClipName;
            characterController.DefenceAnimationClipName = DefenceAnimationClipName;
            characterController.MeleeAnimationClipName = MeleeAnimationClipName;
            characterController.HurtAnimationClipName = HurtAnimationClipName;
            characterController.DieAnimationClipName = DieAnimationClipName;
            #endregion     
            return characterController;
        }

        public void InvertCatStatus() {
            if (catStatus == CatStatus.Normal) {
                catStatus = CatStatus.Stealth;
            }
            else {
                catStatus = CatStatus.Normal;
            }
        }

        public void GetHurt(int faintTime, GameObject attackObject) {
            catMotion = CatMotion.Faint;
            motionTicker = faintTime;
            // get blocked direction
            Vector2 delta = m_gameObject.AbsPositionOld - attackObject.AbsPositionOld;
            if (delta.LengthSquared() > 0.000001f) {
                delta.Normalize();
                m_XYspeed = delta * 3;
            }
        }

        public void Block(GameObject attackObject) {
            // get blocked direction
            Vector2 delta = m_gameObject.AbsPositionOld - attackObject.AbsPositionOld;
            if (delta.LengthSquared() > 0.000001f) {
                delta.Normalize();
                m_XYspeed = delta * 1.5f;
            }
            // generate block object
            GameObject prefab = null;
            Animator animator = (Animator)m_gameObject.GetComponent(typeof(Animator).Name);
            if (BlockPrefabName != null) {
                prefab = Mgr<CatProject>.Singleton.prefabList.GetItem(BlockPrefabName);
            }
            if (animator != null && prefab != null) {
                GameObject blockObject = prefab.DoClone() as GameObject;
                blockObject.PositionOld = attackObject.PositionOld;
                blockObject.HeightOld = attackObject.HeightOld;
                QuadRender quadRender = (QuadRender)blockObject.GetComponent(typeof(QuadRender).Name);
                if (quadRender != null) {
                    quadRender.XMirror = animator.Mirror;
                }
                Mgr<Scene>.Singleton._gameObjectList.AddGameObject(blockObject);
            }
        }

        public void Die() {
            catMotion = CatMotion.Die;
        }

        public override CatModelInstance GetModel() {
            ModelComponent modelComponent = (ModelComponent)m_gameObject.GetComponent(typeof(ModelComponent).Name);
            if (modelComponent != null) {
                return modelComponent.GetCatModelInstance();
            }
            return null;
        }

        public bool IsDefending() {
            return catMotion == CatMotion.Defence;
        }

    }
}
