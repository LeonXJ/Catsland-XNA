using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework.Input;
using Catsland.Plugin.BasicPlugin;

namespace HorseRiding {
    public class HorseController : CatComponent, Drawable{

#region Properties

        [SerialAttribute]
        private readonly CatVector2 m_size = new CatVector2(0.6f, 0.4f);
        public Vector2 Size {
            set {
                float height = MathHelper.Max(value.Y, 0.0f);
                float width = MathHelper.Max(value.X, value.Y);
                m_size.SetValue(new Vector2(width, height));
                UpdateAll();
            }
            get {
                return m_size.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_mass = new CatFloat(1.0f);
        public float Mass {
            set {
                m_mass.SetValue(MathHelper.Max(value, 0.0f));
                m_body.Mass = value;
            }
            get {
                return m_mass;
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_lift = new CatFloat(0.2f);
        public float Lift {
            set {
                m_lift.SetValue(MathHelper.Clamp(value, 0.0f, m_size.Y));
                UpdateAll();
            }
            get {
                return m_lift;
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_motorSpeed = new CatFloat(2.0f);
        public float MotorSpeed {
            set {
                m_motorSpeed.SetValue(value);
                m_axis.MotorSpeed = value;
            }
            get {
                return m_motorSpeed;
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_jumpForce = new CatFloat(4.0f);
        public float JumpForce {
            set {
                m_jumpForce.SetValue(MathHelper.Max(value, 0.0f));
            }
            get {
                return m_jumpForce;
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_jumpRadian = new CatFloat(MathHelper.Pi / 2.0f);
        public float JumpDegree {
            set {
                m_jumpRadian.
                    SetValue(MathHelper.ToRadians(MathHelper.Clamp(value, 0.0f, 90.0f)));
            }
            get {
                return MathHelper.ToDegrees(m_jumpRadian);
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_jumpCooldown = new CatFloat(1.0f);
        public float JumpCoolDownS {
            set {
                m_jumpCooldown.SetValue(MathHelper.Max(value, 0.0f));
            }
            get {
                return m_jumpCooldown;
            }
        }

        [SerialAttribute]
        private string m_runningAnimationName = "";
        public string RunningAnimationName {
            set {
                m_runningAnimationName = value;
            }
            get {
                return m_runningAnimationName;
            }
        }

        [SerialAttribute]
        private string m_standingAnimationName = "";
        public string StandingAnimationName {
            set {
                m_standingAnimationName = value;
            }
            get {
                return m_standingAnimationName;
            }
        }

        [SerialAttribute]
        private string m_jumpingUpAnimationName = "";
        public string JumpingUpAnimationName {
            set {
                m_jumpingUpAnimationName = value;
            }
            get {
                return m_jumpingUpAnimationName;
            }
        }

        [SerialAttribute]
        private string m_jumpingDownAnimationName = "";
        public string JumpingDownAnimationName {
            set {
                m_jumpingDownAnimationName = value;
            }
            get {
                return m_jumpingDownAnimationName;
            }
        }
        
        
        private Body m_body;
        private Body m_wheel;
        private RevoluteJoint m_axis;
        private float m_jumpCooldownCounter = 0.0f;

        private VertexPositionColor []m_vertex;
        private VertexBuffer m_vertexBuffer;

        private SensorAttachment m_onGroundSensor;

#endregion

        public HorseController()
            : base() {
        }

        public HorseController(GameObject _gameObject)
            : base(_gameObject) {
        }

        public override void Initialize(Scene scene) {
            base.Initialize(scene);
            UpdateAll();
        }

        protected void UpdateAll() {
            ConfigPhysicsBody();
            UpdateSensors();
            UpdateDebugVertex();
        }

        protected void ConfigPhysicsBody() {
            PhysicsSystem physicsSystem = Mgr<Scene>.Singleton.GetPhysicsSystem();
            if (physicsSystem == null) {
                return;
            }
            if (m_body != null) {
                physicsSystem.GetWorld().RemoveBody(m_body);
                m_body = null;
            }
            if (m_wheel != null) {
                physicsSystem.GetWorld().RemoveBody(m_wheel);
                m_wheel = null;
            }
            // create body
            m_body = BodyFactory.CreateRectangle(physicsSystem.GetWorld(),
                                                 m_size.X,
                                                 m_size.Y - m_lift,
                                                 m_mass, "player");
            m_body.BodyType = BodyType.Dynamic;
            m_body.SleepingAllowed = false;
            m_body.FixedRotation = true;
            m_body.Position = new Vector2(0.0f, m_lift + (m_size.Y - m_lift) / 2.0f);

            m_wheel = BodyFactory.CreateCircle(physicsSystem.GetWorld(),
                            m_size.Y / 2.0f, m_mass, "player");
            m_wheel.BodyType = BodyType.Dynamic;
            m_wheel.SleepingAllowed = false;
            m_wheel.Friction = 1.0f;
            m_wheel.Position = new Vector2(0.0f, m_size.Y / 2.0f);
            m_wheel.Friction = 1.0f;

            JointFactory.CreateAngleJoint(physicsSystem.GetWorld(),
                                   m_wheel, m_body);
            m_axis = JointFactory.CreateRevoluteJoint(physicsSystem.GetWorld(),
                        m_wheel, m_body, m_wheel.WorldCenter);
            m_axis.CollideConnected = false;
            m_axis.MotorEnabled = true;
            m_axis.MaxMotorTorque = 100.0f;
            m_axis.MotorSpeed = m_motorSpeed;

            MoveBodyToGameObject();
        }

        protected void UpdateAnimation() {
            Animator animator = 
                m_gameObject.GetComponent(typeof(Animator).ToString()) as Animator;
            if (animator == null) {
                return;
            }
            if (m_onGroundSensor.IsTriggered) {
                // on ground
                if (m_body.LinearVelocity.X * m_body.LinearVelocity.X > 0.00005f) {
                    animator.CheckPlayAnimation(RunningAnimationName);
                }
                else {
                    animator.CheckPlayAnimation(StandingAnimationName);
                }
                
            }
            else {
                // in the air
                if (m_body.LinearVelocity.Y > 0) {
                    animator.CheckPlayAnimation(JumpingUpAnimationName);
                }
                else {
                    // going down
                    animator.CheckPlayAnimation(JumpingDownAnimationName);
                }
            }
        }

        protected void UpdateSensors() {
            if (m_onGroundSensor == null) {
                m_onGroundSensor = new SensorAttachment(m_wheel);
                m_onGroundSensor.BindToScene(Mgr<Scene>.Singleton);
            }
            float sensorHalfSize = 0.03f;
            float gap = 0.01f;
            m_onGroundSensor.Size = new Vector2(sensorHalfSize, sensorHalfSize);
            m_onGroundSensor.Offset = new Vector2(0.0f,
                            - m_size.Y /2.0f - gap - sensorHalfSize);
        }

        public override void EditorUpdate(int timeLastFrame) {
            base.EditorUpdate(timeLastFrame);

            MoveBodyToGameObject();
            /*UpdateSensors();*/
        }

        public override void BindToScene(Scene scene) {
            base.BindToScene(scene);

            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                // add to debug drawable
                if (scene != null) {
                    scene._debugDrawableList.AddItem(this);
                }
            }
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);

            MoveGameObjectToBody();
            /*UpdateSensors();*/
            DoControll(timeLastFrame);
            UpdateAnimation();
        }

        protected void DoControll(int _timeLastFrame) {

            // TODO: make this as input
            KeyboardState keyboardState = Keyboard.GetState();
            if (m_onGroundSensor.IsTriggered) {
                // On ground
                if (keyboardState.IsKeyDown(Keys.Space)) {
                    if (m_jumpCooldownCounter < 0.001f) {
                        DoJump();
                        m_jumpCooldownCounter = m_jumpCooldown;
                    } 
                }
            }
            if (m_jumpCooldownCounter > 0.0f) {
                m_jumpCooldownCounter -= _timeLastFrame / 1000.0f;
            }
        }

        private void DoJump() {
            m_body.ApplyForce(new Vector2((float)Math.Cos(m_jumpRadian), 
                                          (float)Math.Sin(m_jumpRadian))
                                         * m_jumpForce);
        }

        public void MoveGameObjectToBody() {
            // Warning: the gameObject should not have parent
            m_gameObject.Position = new Vector3(m_wheel.Position.X,
                                                m_wheel.Position.Y - m_size.Y /2.0f,
                                                m_gameObject.Position.Z);
//             m_gameObject.Rotation = new Vector3(m_gameObject.Rotation.X,
//                                                  m_gameObject.Rotation.Y,
//                                                 MathHelper.ToDegrees(m_body.Rotation));
        }

        private void MoveBodyToGameObject() {
            m_gameObject.ForceUpdateAbsTransformation();
            float bodyY = (m_size.Y - m_lift) / 2.0f + m_lift;
            m_body.SetTransform(new Vector2(m_gameObject.AbsPosition.X,
                                          m_gameObject.AbsPosition.Y + bodyY),
                                          m_gameObject.AbsRotation.Z);
            m_wheel.SetTransform(new Vector2(m_gameObject.AbsPosition.X,
                                           m_gameObject.AbsPosition.Y + m_size.Y / 2.0f),
                                           0.0f);
        }

        public void UpdateDebugVertex() {
            if (m_vertex == null) {
                // 5 for rectangle, 17 for circle
                m_vertex = new VertexPositionColor[5 + 17];
                for (int i = 0; i < 5 + 17; ++i) {
                    m_vertex[i] = new VertexPositionColor();
                }
            }
            // set position
            UpdateRectangleVertex(m_vertex);
            UpdateCircleVertex(m_vertex, 5);
            // set buffer
            if (m_vertexBuffer == null) {
                m_vertexBuffer = new VertexBuffer(Mgr<GraphicsDevice>.Singleton,
                                        typeof(VertexPositionColor),
                                        5 + 17,
                                        BufferUsage.None);
            }
            m_vertexBuffer.SetData<VertexPositionColor>(m_vertex);

        }

        protected void UpdateRectangleVertex(VertexPositionColor []_vertex) {
            float halfHeight = (m_size.Y - m_lift) / 2.0f;
            _vertex[0].Position = new Vector3(-m_size.X / 2.0f, halfHeight, 1);
            _vertex[1].Position = new Vector3(m_size.X / 2.0f, halfHeight, 1);
            _vertex[2].Position = new Vector3(m_size.X / 2.0f, -halfHeight, 1);
            _vertex[3].Position = new Vector3(-m_size.X / 2.0f, -halfHeight, 1);
            _vertex[4].Position = new Vector3(-m_size.X / 2.0f, halfHeight, 1);
        }

        protected void UpdateCircleVertex(VertexPositionColor[] _vertex, int _offset = 5) {
            float degree = (float)(Math.PI * 2.0f / 16.0f);
            float radius = m_size.Y / 2.0f;
            for (int i = 0; i < 16; ++i) {

                _vertex[_offset + i].Position = new Vector3(
                                                radius * (float)Math.Cos(degree * i),
                                                radius * (float)Math.Sin(degree * i),
                                                1);
            }
            _vertex[_offset + 16].Position = new Vector3(radius, 0, 1);
        }

        public void Draw(int timeLastFrame) {
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                UpdateDebugVertex();

                BasicEffect effect = Mgr<DebugTools>.Singleton.DrawEffect;
                effect.View = Mgr<Camera>.Singleton.View;
                effect.Projection = Mgr<Camera>.Singleton.m_projection;
                effect.VertexColorEnabled = false;
                effect.DiffuseColor = new Vector3(0.0f, 1.0f, 0.0f);
                effect.Alpha = 1.0f;

                Mgr<GraphicsDevice>.Singleton.SetVertexBuffer(m_vertexBuffer);
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                    Transform transform;
                    m_body.GetTransform(out transform);
                    Vector2 position = transform.p;

                    Matrix matPosition = Matrix.CreateTranslation(new Vector3(
                                                                   position.X,
                                                                   position.Y,
                                                                   0));
                    Matrix matRotaion = Matrix.CreateRotationZ(transform.q.GetAngle());
                    effect.World = matPosition * matRotaion;
                    pass.Apply();
                    Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionColor>(
                        PrimitiveType.LineStrip,
                        m_vertex,
                        0,
                        4);
                    position = m_wheel.WorldCenter;
                    effect.World = Matrix.CreateTranslation(new Vector3(position.X,
                                                                        position.Y,
                                                                        0));
                    pass.Apply();
                    Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionColor>(
                        PrimitiveType.LineStrip,
                        m_vertex,
                        5,
                        16);
                }
            }
        }

        public float GetDepth() {
            return 0;
        }

        public int CompareTo(object obj) {
            return 1;
        }

    }
}
