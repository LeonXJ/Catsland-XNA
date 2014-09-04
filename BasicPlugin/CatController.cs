using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Catsland.Core;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Contacts;

namespace Catsland.Plugin.BasicPlugin {
    public class CatController : CatComponent, Drawable {

        #region Properties

        [SerialAttribute]
        private readonly CatVector2 m_outSize = new CatVector2(0.3f, 0.6f);
        public Vector2 OutSize {
            set {
                float width = MathHelper.Max(m_inSize.X, value.X);
                float height = MathHelper.Max(m_inSize.Y, value.Y);
                m_outSize.SetValue(new Vector2(width, height));
                UpdateAll();
            }
            get {
                return m_outSize.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatVector2 m_inSize = new CatVector2(0.2f, 0.5f);
        public Vector2 InSize {
            set {
                float width = MathHelper.Clamp(value.X, 0.0f, m_outSize.X);
                float height = MathHelper.Clamp(value.Y, 0.0f, m_outSize.Y);
                m_inSize.SetValue(new Vector2(width, height));
                Tag bodyTag = m_body.UserData as Tag;
                bodyTag.setHalfHeight(height / 2.0f);
                UpdateAll();
            }
            get {
                return m_inSize.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_mass = new CatFloat(1.0f);
        public float Mass {
            set {
                m_mass.SetValue(MathHelper.Max(0.0f, value));
                m_body.Mass = m_mass;
            }
            get {
                return m_mass.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_restitution = new CatFloat(0.0f);
        public float Restitution {
            set {
                m_restitution.SetValue(MathHelper.Clamp(value, 0.0f, 1.0f));
                m_body.Restitution = m_restitution;
            }
            get {
                return m_restitution.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_friction = new CatFloat(0.8f);
        public float Friction {
            set {
                m_friction.SetValue(MathHelper.Clamp(value, 0.0f, 1.0f));
                m_body.Friction = m_restitution;
            }
            get {
                return m_friction.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_damping = new CatFloat(0.0f);
        public float Damping {
            set {
                m_damping.SetValue(MathHelper.Max(value, 0.0f));
                m_body.LinearDamping = m_damping;
            }
            get {
                return m_damping.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_walkSpeed = new CatFloat(0.5f);
        public float WalkSpeed {
            set {
                m_walkSpeed.SetValue(MathHelper.Max(0.0f, value));
            }
            get {
                return m_walkSpeed.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_runAcc = new CatFloat(0.2f);
        public float RunAcc {
            set {
                m_runAcc.SetValue(MathHelper.Max(0.0f, value));
            }
            get {
                return m_runAcc.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_breakAcc = new CatFloat(0.3f);
        public float BreakAcc {
            set {
                m_breakAcc.SetValue(MathHelper.Max(0.0f, value));
            }
            get {
                return m_breakAcc.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_runSpeed = new CatFloat(0.9f);
        public float RunSpeed {
            set {
                m_runSpeed.SetValue(MathHelper.Max(0.0f, value));
            }
            get {
                return m_runSpeed.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_jumpForce = new CatFloat(20.0f);
        public float JumpForce {
            set {
                m_jumpForce.SetValue(MathHelper.Max(0.0f, value));
            }
            get {
                return m_jumpForce.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_horjumpBias = new CatFloat(0.1f);
        public float HorJumpBias {
            set {
                m_horjumpBias.SetValue(MathHelper.Clamp(value, 0.0f, 1.0f));
            }
            get {
                return m_horjumpBias.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_wallJumpForce = new CatFloat(10.0f);
        public float WallJumpForce {
            set {
                m_wallJumpForce.SetValue(MathHelper.Max(0.0f, value));
            }
            get {
                return m_wallJumpForce.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_airBiasOffset = new CatFloat(0.2f);
        public float AirBiasOffset {
            set {
                m_airBiasOffset.SetValue(MathHelper.Max(0.0f, value));
            }
            get {
                return m_airBiasOffset.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_jumpLiftForce = new CatFloat(3.0f);
        public float JumpLiftForce {
            set {
                m_jumpLiftForce.SetValue(MathHelper.Max(0.0f, value));
            }
            get {
                return m_jumpLiftForce.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_stealthSpeed = new CatFloat(0.9f);
        public float StealthSpeed {
            set {
                m_stealthSpeed.SetValue(MathHelper.Max(0.0f, value));
            }
            get {
                return m_stealthSpeed.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatInteger m_attackPrepareTimeMS = new CatInteger(200);
        public int AttackPrepareTimeMS {
            set {
                m_attackPrepareTimeMS.SetValue((int)MathHelper.Max(value, 0));
            }
            get {
                return m_attackPrepareTimeMS;
            }
        }

        [SerialAttribute]
        private readonly CatInteger m_attackPostTimeMS = new CatInteger(200);
        public int AttackPostTimeMS {
            set {
                m_attackPostTimeMS.SetValue((int)MathHelper.Max(value, 0.0f));
            }
            get {
                return m_attackPostTimeMS;
            }
        }

        [SerialAttribute]
        private readonly CatInteger m_attackCooldownTimeMS = new CatInteger(200);
        public int AttackCooldownTimeMS {
            set {
                m_attackCooldownTimeMS.SetValue((int)MathHelper.Max(value, 0.0f));
            }
            get {
                return m_attackCooldownTimeMS;
            }
        }

        [SerialAttribute]
        private string m_attackPrefabName = "";
        public string AttackPrefabName {
            set {
                m_attackPrefabName = value;
            }
            get {
                return m_attackPrefabName;
            }
        }

        internal int m_currentAttackCooldownMS = 0;

        public Body m_body;
        public Fixture m_taller;
        private SensorAttachment m_onGroundSensor;
        private SensorAttachment m_leftAttachableSensor;
        private SensorAttachment m_rightAttachableSensor;
        private StealthKillSensor m_stealthKillSensor;

        private VertexPositionColor[] m_vertex;
        private VertexBuffer m_vertexBuffer;
        private int m_tallerSensorTouched = 0;

        private ControllState m_currentState;
        public ControllState CurrentState {
            set {
                m_currentState = value;
            }
        }

        public enum XOrientation {
            Left,
            Right,
        }
        private XOrientation m_orientation = XOrientation.Right; // 1 for right, -1 for left
        public XOrientation Orientation {
            set {
                m_orientation = value;
            }
            get {
                return m_orientation;
            }
        }
        public void SetOrientationByFloat(float _value) {
            if (_value < 0.0f) {
                m_orientation = XOrientation.Left;
            }
            else {
                m_orientation = XOrientation.Right;
            }
        }

        // animation
        [SerialAttribute]
        public string m_aniStand = "stand";
        public string AniStand {
            set { m_aniStand = value; }
            get { return m_aniStand; }
        }

        [SerialAttribute]
        public string m_aniWalk = "walk";
        public string AniWalk {
            set { m_aniWalk = value; }
            get { return m_aniWalk; }
        }

        [SerialAttribute]
        public string m_aniRun = "run";
        public string AniRun {
            set { m_aniRun = value; }
            get { return m_aniRun; }
        }

        [SerialAttribute]
        public string m_aniJumpUp = "jump";
        public string AniJumpUp {
            set { m_aniJumpUp = value; }
            get { return m_aniJumpUp; }
        }

        [SerialAttribute]
        public string m_aniFall = "fall";
        public string AniFall {
            set { m_aniFall = value; }
            get { return m_aniFall; }
        }

        [SerialAttribute]
        public string m_aniAttach = "attach";
        public string AniAttach {
            set { m_aniAttach = value; }
            get { return m_aniAttach; }
        }

        public bool m_wantLeft = false;
        public bool m_wantRight = false;
        public bool m_wantUp = false;
        public bool m_wantDown = false;
        public bool m_wantRun = false;
        public bool m_wantJump = false;
        public bool m_wantLift = false;
        public bool m_wantKill = false;

        #endregion

        public CatController()
            : base() {
        }

        public CatController(GameObject _gameObject)
            : base(_gameObject) {
        }

        public override void UnbindFromScene(Scene _scene) {
            base.UnbindFromScene(_scene);
            PhysicsSystem physicsSystem = Mgr<Scene>.Singleton.GetPhysicsSystem();
            if (physicsSystem == null) {
                return;
            }
            if (m_body != null) {
                physicsSystem.GetWorld().RemoveBody(m_body);
                m_body = null;
            }
        }

        public override void Initialize(Scene _scene) {
            base.Initialize(_scene);
            UpdateAll();
            m_currentState = StateStandWalk.GetState();
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
            // create body
            m_body = BodyFactory.CreateRectangle(physicsSystem.GetWorld(),
                                                 m_inSize.X,
                                                 m_inSize.Y / 2.0f,
                                                 m_mass, new Tag(0, m_inSize.Y / 4.0f));

            m_body.BodyType = BodyType.Dynamic;
            m_body.SleepingAllowed = false;
            m_body.FixedRotation = true;
            m_body.Position = new Vector2(0.0f, 0.0f);
            m_body.UserData = new Tag(4, 0.0f, m_gameObject);
            m_body.CollisionGroup = -2;
            // create tail part
            m_taller = FixtureFactory.AttachRectangle(m_inSize.X - 0.1f, m_inSize.Y / 2.0f, 0.01f,
                new Vector2(0.0f, m_inSize.Y / 2.0f), m_body);
            m_taller.OnCollision = OnCollision;
            m_taller.OnSeparation = OnSeparation;
            m_taller.CollisionGroup = -2;
            MoveBodyToGameObject();
        }

        protected void UpdateAnimation(int _timeLastFrame) {

            if (m_orientation == XOrientation.Right) {
                m_gameObject.Rotation = new Vector3(0.0f, 0.0f, 0.0f);
            }
            else {
                m_gameObject.Rotation = new Vector3(0.0f, 180.0f, 0.0f);
            }

            m_currentState.UpdateAnimation(this,
                m_gameObject.GetComponent(typeof(Animator).ToString()) as Animator,
                _timeLastFrame);
        }

        protected void UpdateCooldown(int _timeInMS) {
            if (m_currentAttackCooldownMS > 0) {
                m_currentAttackCooldownMS -= _timeInMS;
            }
        }

        internal void TryAttack() {
            if (m_currentAttackCooldownMS <= 0) {
                m_currentState = StateAttack.GetState();
            }
        }

        protected void UpdateSensors() {
            // onGroundSensor
            if (m_onGroundSensor == null) {
                m_onGroundSensor = new SensorAttachment(m_body);
                m_onGroundSensor.BindToScene(Mgr<Scene>.Singleton);
            }
            float sensorHalfSize = 0.03f;
            float sensorGap = 0.01f;
            m_onGroundSensor.Size = new Vector2(m_inSize.X - 0.1f, sensorHalfSize);
            m_onGroundSensor.Offset = new Vector2(0.0f,
                -m_inSize.Y / 4.0f - sensorGap - sensorHalfSize);
            // leftAttachableSensor
            if (m_leftAttachableSensor == null) {
                m_leftAttachableSensor = new SensorAttachment(m_body);
                m_leftAttachableSensor.BindToScene(Mgr<Scene>.Singleton);
                m_leftAttachableSensor.AcceptTag = Tag.AttachPoint;
            }
            m_leftAttachableSensor.Size = new Vector2(sensorHalfSize, sensorHalfSize);
            m_leftAttachableSensor.Offset = new Vector2(-m_inSize.X / 2.0f - sensorGap - sensorHalfSize,
                m_inSize.Y * 0.75f - sensorHalfSize);
            // rightAttachableSensor
            if (m_rightAttachableSensor == null) {
                m_rightAttachableSensor = new SensorAttachment(m_body);
                m_rightAttachableSensor.BindToScene(Mgr<Scene>.Singleton);
                m_rightAttachableSensor.AcceptTag = Tag.AttachPoint;
            }
            m_rightAttachableSensor.Size = new Vector2(sensorHalfSize, sensorHalfSize);
            m_rightAttachableSensor.Offset = new Vector2(m_inSize.X / 2.0f + sensorGap + sensorHalfSize,
                m_inSize.Y * 0.75f - sensorHalfSize);
            // stealth kill sensor
            if (m_stealthKillSensor == null) {
                m_stealthKillSensor = new StealthKillSensor(m_gameObject, m_body);
                m_stealthKillSensor.BindToScene(Mgr<Scene>.Singleton);
            }
            m_stealthKillSensor.Radius = 2.0f;
            m_stealthKillSensor.Offset = Vector2.Zero;
        }

        public override void EditorUpdate(int timeLastFrame) {
            base.EditorUpdate(timeLastFrame);
            MoveBodyToGameObject();
        }

        public override void BindToScene(Scene scene) {
            base.BindToScene(scene);
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                if (scene != null) {
                    scene._debugDrawableList.AddItem(this);
                }
            }
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);
            MoveGameObjectToBody();
            if (m_currentState == StateStealth.GetState()) {
                m_stealthKillSensor.Update(timeLastFrame);
            }
            UpdateCooldown(timeLastFrame);
            /*UpdateSensors();*/
            DoControll(timeLastFrame);
            UpdateAnimation(timeLastFrame);
            ResetAllWant();
        }

        protected void DoControll(int _timeLastFrame) {
            m_currentState.Do(this, _timeLastFrame);
        }

        public void MoveGameObjectToBody() {
            // Warning: the gameObject should not have parent
            m_gameObject.Position = new Vector3(m_body.Position.X,
                                                m_body.Position.Y + m_inSize.Y / 4.0f + (m_outSize.Y - m_inSize.Y) / 2.0f,
                                                m_gameObject.Position.Z);
        }

        private void MoveBodyToGameObject() {
            m_gameObject.ForceUpdateAbsTransformation();
            m_body.SetTransform(new Vector2(m_gameObject.AbsPosition.X,
                                            m_gameObject.AbsPosition.Y - (m_outSize.Y - m_inSize.Y) / 2.0f - m_inSize.Y / 4.0f),
                                            0.0f);
        }

        private void ResetAllWant() {
            m_wantLeft = false;
            m_wantRight = false;
            m_wantUp = false;
            m_wantDown = false;
            m_wantJump = false;
            m_wantRun = false;
            m_wantLift = false;
            m_wantKill = false;
        }

        public void UpdateDebugVertex() {
            if (m_vertex == null) {
                m_vertex = new VertexPositionColor[10];
                for (int i = 0; i < 10; ++i) {
                    m_vertex[i] = new VertexPositionColor();
                }
            }
            // set position
            UpdateRectangleVertex(m_vertex, 0, m_outSize.X, m_outSize.Y);
            UpdateRectangleVertex(m_vertex, 5, m_inSize.X, m_inSize.Y);
            // set buffer
            if (m_vertexBuffer == null) {
                m_vertexBuffer = new VertexBuffer(Mgr<GraphicsDevice>.Singleton,
                    typeof(VertexPositionColor),
                    10, BufferUsage.None);

            }
            m_vertexBuffer.SetData<VertexPositionColor>(m_vertex);
        }

        protected void UpdateRectangleVertex(VertexPositionColor[] _vertex,
                                             int _offset,
                                             float _width, float _height) {
            float halfWidth = _width / 2.0f;
            float halfHeight = _height / 2.0f;
            _vertex[_offset].Position = new Vector3(-halfWidth, halfHeight, 1);
            _vertex[_offset + 1].Position = new Vector3(halfWidth, halfHeight, 1);
            _vertex[_offset + 2].Position = new Vector3(halfWidth, -halfHeight, 1);
            _vertex[_offset + 3].Position = new Vector3(-halfWidth, -halfHeight, 1);
            _vertex[_offset + 4].Position = new Vector3(-halfWidth, halfHeight, 1);
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
                                                                   position.Y + m_inSize.Y / 4.0f + (m_outSize.Y - m_inSize.Y) / 2.0f,
                                                                   0));
                    Matrix matRotaion = Matrix.CreateRotationZ(transform.q.GetAngle());
                    effect.World = matPosition * matRotaion;
                    pass.Apply();
                    Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionColor>(
                        PrimitiveType.LineStrip,
                        m_vertex,
                        0,
                        4);

                    effect.World = Matrix.CreateTranslation(new Vector3(position.X,
                                                                        position.Y + m_inSize.Y / 4.0f,
                                                                        0));
                    pass.Apply();
                    Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionColor>(
                        PrimitiveType.LineStrip,
                        m_vertex,
                        5,
                        4);
                }
            }
        }

        private bool IsTallBlocker(Fixture _fixture) {
            if (_fixture.Body.UserData != null) {
                if (Tag.Platform != _fixture.Body.UserData as Tag &&
                    Tag.Role != _fixture.Body.UserData as Tag) {
                    return true;
                }
            }
            return false;
        }

        protected bool OnCollision(Fixture _fixtureA, Fixture _fixtureB, Contact _contact) {
            if (_fixtureA.Body != m_body && IsTallBlocker(_fixtureA)) {
                m_tallerSensorTouched += 1;
            }
            else if (_fixtureB.Body != m_body && IsTallBlocker(_fixtureB)) {
                m_tallerSensorTouched += 1;
            }
            return true;
        }

        protected void OnSeparation(Fixture _fixtureA, Fixture _fixtureB) {
            if (_fixtureA.Body != m_body && IsTallBlocker(_fixtureA)) {
                m_tallerSensorTouched -= 1;
            }
            else if (_fixtureB.Body != m_body && IsTallBlocker(_fixtureB)) {
                m_tallerSensorTouched -= 1;
            }
        }

        public bool IsOnGround() {
            return m_onGroundSensor.IsTriggered;
        }

        public bool IsRightAttachable() {
            return IsAttachable(m_rightAttachableSensor);
        }

        public bool IsLeftAttachable() {
            return IsAttachable(m_leftAttachableSensor);
        }

        private bool IsAttachable(SensorAttachment _sensor) {
            return _sensor.IsTriggered;
        }

        public bool CanPerformStealthKill() {
            if (m_stealthKillSensor == null) {
                return false;
            }
            return (m_stealthKillSensor.ToBeKilled != null);
        }

        public GameObject GetToBeKilled() {
            if (m_stealthKillSensor != null) {
                return m_stealthKillSensor.ToBeKilled;
            }
            return null;
        }

        public float GetDepth() {
            return 0;
        }

        public int CompareTo(object obj) {
            return 1;
        }

        public void DoJump(bool _isStealth = false) {
            if (!IsOnGround()) {
                return;
            }
            if (m_wantDown) {
                if (m_onGroundSensor.LastContactFixture != null
                    && m_onGroundSensor.LastContactFixture.Body != null) {
                    Tag bodyTag = m_onGroundSensor.LastContactFixture.Body.UserData as Tag;
                    if (bodyTag != null && Tag.Platform == bodyTag) {
                        StateFall.GetState().IgnoreBody = m_onGroundSensor.LastContactFixture.Body;
                        m_body.IgnoreCollisionWith(m_onGroundSensor.LastContactFixture.Body);
                        CurrentState = StateFall.GetState();
                    }
                }
            }
            else {
                if (!_isStealth || TryExitStealth()) {
                    float hor_force = 0.0f;
                    if (m_wantLeft) {
                        hor_force -= HorJumpBias;
                    }
                    if (m_wantRight) {
                        hor_force += HorJumpBias;
                    }
                    Vector2 direction = new Vector2(hor_force, 1.0f);
                    m_body.ApplyForce(direction * m_jumpForce);
                    CurrentState = StateJumpUp.GetState();
                }
            }
        }

        public void EnterStealth() {
            m_taller.IsSensor = true;
            CurrentState = StateStealth.GetState();
        }

        public bool TryExitStealth() {
            if (m_tallerSensorTouched > 0) {
                return false;
            }
            else {
                m_taller.IsSensor = false;
                CurrentState = StateStandWalk.GetState();
                return true;
            }
        }
    }

    public interface ControllState {
        void Do(CatController _controller, int _delta);
        void UpdateAnimation(CatController _controller, Animator _animator, int _delta);
    }

    class StateStandWalk : ControllState {
        static private StateStandWalk state;

        static public StateStandWalk GetState() {
            if (StateStandWalk.state == null) {
                StateStandWalk.state = new StateStandWalk();
            }
            return StateStandWalk.state;
        }

        public void Do(CatController _controller, int _delta) {
            if (_controller.IsOnGround()) {
                // jump
                if (_controller.m_wantJump) {
                    _controller.DoJump();
                }
                else if (_controller.m_wantKill) {
                    _controller.TryAttack();
                }
                // left / right
                else if (_controller.m_wantDown) {
                    _controller.EnterStealth();
                }
                else {
                    float hor_vel = 0.0f;
                    if (_controller.m_wantLeft) {
                        hor_vel -= 1.0f;
                    }
                    if (_controller.m_wantRight) {
                        hor_vel += 1.0f;
                    }
                    if (hor_vel * hor_vel > 0.0f) {
                        _controller.SetOrientationByFloat(hor_vel);
                    }
                    float x_vel = _controller.m_body.LinearVelocity.X;
                    if (_controller.m_wantRun ||
                        x_vel * x_vel > _controller.WalkSpeed * _controller.WalkSpeed) {
                        _controller.CurrentState = StateRun.GetState();
                    }
                    else {
                        hor_vel *= _controller.WalkSpeed;
                        float impluse = (hor_vel - _controller.m_body.LinearVelocity.X) *
                                _controller.m_body.Mass;
                        _controller.m_body.ApplyLinearImpulse(new Vector2(impluse, 0.0f));
                    }
                }
            }
            else {
                _controller.CurrentState = StateFall.GetState();
            }
        }

        public void UpdateAnimation(CatController _controller, Animator _animator, int _timeLastFrame) {

            float hor_vel = _controller.m_body.LinearVelocity.X;
            if (hor_vel * hor_vel > 0.05f) {
                _animator.CheckPlayAnimation(_controller.m_aniWalk);
            }
            else {
                _animator.CheckPlayAnimation(_controller.m_aniStand);
            }
        }
    }

    class StateRun : ControllState {
        static private StateRun state;

        static public StateRun GetState() {
            if (StateRun.state == null) {
                StateRun.state = new StateRun();
            }
            return StateRun.state;
        }

        public void Do(CatController _controller, int _delta) {
            if (_controller.IsOnGround()) {
                // jump
                if (_controller.m_wantJump) {
                    _controller.DoJump();
                }
                else if (_controller.m_wantKill) {
                    _controller.TryAttack();
                }
                else {
                    // want walk and same direction and slow enough,
                    // if not slow enough, just do nothing and let it slow down
                    float hor_v = _controller.m_body.LinearVelocity.X;
                    float hor_desired_vel = 0.0f;
                    if (_controller.m_wantLeft) {
                        hor_desired_vel -= 1.0f;
                    }
                    if (_controller.m_wantRight) {
                        hor_desired_vel += 1.0f;
                    }
                    if (hor_desired_vel * hor_desired_vel > 0.0f) {
                        _controller.SetOrientationByFloat(hor_desired_vel);
                    }
                    if (hor_v * hor_desired_vel > 0.0 || hor_v * hor_v < 0.01f) {
                        // same direction
                        // want run, acc or keep speed
                        if (_controller.m_wantRun && !_controller.m_wantDown) {
                            hor_desired_vel *=
                                MathHelper.Min(Math.Abs(hor_v) + _controller.RunAcc, _controller.RunSpeed);

                        }
                        // slowing down
                        else {
                            if (hor_v * hor_v < _controller.WalkSpeed * _controller.WalkSpeed) {
                                _controller.CurrentState = StateStandWalk.GetState();
                            }
                            hor_desired_vel *= MathHelper.Max(Math.Abs(hor_v) - _controller.RunAcc, _controller.WalkSpeed);
                        }
                        float impluse = (hor_desired_vel - hor_v) * _controller.m_body.Mass;
                        _controller.m_body.ApplyLinearImpulse(new Vector2(impluse, 0.0f));

                    }
                    // different direction or want stop
                    else {
                        _controller.CurrentState = StateBreak.GetState();
                    }
                }
            }
            else {
                _controller.CurrentState = StateFall.GetState();
            }
        }

        public void UpdateAnimation(CatController _controller, Animator _animator, int _timeLastFrame) {

            _animator.CheckPlayAnimation(_controller.m_aniRun);
        }

    }

    class StateBreak : ControllState {
        static private StateBreak state;

        static public StateBreak GetState() {
            if (StateBreak.state == null) {
                StateBreak.state = new StateBreak();
            }
            return StateBreak.state;
        }

        public void Do(CatController _controller, int _delta) {
            if (_controller.IsOnGround()) {
                float hor_v = _controller.m_body.LinearVelocity.X;
                float impluse = MathHelper.Max(Math.Abs(hor_v) - _controller.BreakAcc, 0.0f) * hor_v
                    * _controller.m_body.Mass;

                if (hor_v * hor_v < 0.01f) {
                    _controller.CurrentState = StateStandWalk.GetState();
                }
            }
            else {
                _controller.CurrentState = StateFall.GetState();
            }
        }

        public void UpdateAnimation(CatController _controller, Animator _animator, int _timeLastFrame) {

            _animator.CheckPlayAnimation(_controller.m_aniStand);
            // TODO: should be breaking animation
        }
    }

    class StateJumpUp : ControllState {
        static private StateJumpUp state;

        static public StateJumpUp GetState() {
            if (StateJumpUp.state == null) {
                StateJumpUp.state = new StateJumpUp();
            }
            return StateJumpUp.state;
        }

        public void Do(CatController _controller, int _delta) {
            if (_controller.m_body.LinearVelocity.Y <= 0.0f) {
                _controller.CurrentState = StateFall.GetState();
            }
            if (_controller.IsRightAttachable() && _controller.m_wantRight) {
                _controller.CurrentState = StateAttach.GetState();
                _controller.Orientation = CatController.XOrientation.Right;
            }
            else if (_controller.IsLeftAttachable() && _controller.m_wantLeft) {
                _controller.CurrentState = StateAttach.GetState();
                _controller.Orientation = CatController.XOrientation.Left;
            }
            float ver_force = 0.0f;
            if (_controller.m_wantLift) {
                ver_force = _controller.JumpLiftForce;
                _controller.m_body.ApplyForce(new Vector2(0.0f, ver_force * _delta / 1000.0f));
            }
            float hor_force = 0.0f;
            if (_controller.m_wantLeft) {
                hor_force -= 1.0f;
            }
            if (_controller.m_wantRight) {
                hor_force += 1.0f;
            }
            if (hor_force * hor_force > 0.1f) {
                _controller.m_body.Position += new Vector2(hor_force * _controller.AirBiasOffset, 0.0f)
                    * _delta / 1000.0f;
                _controller.SetOrientationByFloat(hor_force);
            }
        }

        public void UpdateAnimation(CatController _controller, Animator _animator, int _timeLastFrame) {

            _animator.CheckPlayAnimation(_controller.m_aniJumpUp);
        }
    }

    class StateFall : ControllState {
        private Body m_ignoreBody;
        public Body IgnoreBody {
            set {
                m_ignoreBody = value;
            }
        }

        static public StateFall GetState() {
            return new StateFall();
        }

        private void RestoreIgnoreBody(Body _body) {
            if (m_ignoreBody != null) {
                _body.RestoreCollisionWith(m_ignoreBody);
            }
            m_ignoreBody = null;
        }

        public void Do(CatController _controller, int _delta) {
            // attach
            if (_controller.IsRightAttachable() && _controller.m_wantRight) {
                RestoreIgnoreBody(_controller.m_body);
                _controller.CurrentState = StateAttach.GetState();
                _controller.Orientation = CatController.XOrientation.Right;
            }
            else if (_controller.IsLeftAttachable() && _controller.m_wantLeft) {
                RestoreIgnoreBody(_controller.m_body);
                _controller.CurrentState = StateAttach.GetState();
                _controller.Orientation = CatController.XOrientation.Left;
            }
            // touch ground
            if (_controller.IsOnGround()) {
                RestoreIgnoreBody(_controller.m_body);
                _controller.CurrentState = StateStandWalk.GetState();
            }
            // adjust
            float hor_force = 0.0f;
            if (_controller.m_wantLeft) {
                hor_force -= 1.0f;
            }
            if (_controller.m_wantRight) {
                hor_force += 1.0f;
            }
            if (hor_force * hor_force > 0.1f) {
                _controller.m_body.Position += new Vector2(hor_force * _controller.AirBiasOffset, 0.0f)
                    * _delta / 1000.0f;
                _controller.SetOrientationByFloat(hor_force);
            }
        }

        public void UpdateAnimation(CatController _controller, Animator _animator, int _timeLastFrame) {
            _animator.CheckPlayAnimation(_controller.m_aniFall);
        }
    }

    class StateAttach : ControllState {
        static private StateAttach state;

        static public StateAttach GetState() {
            if (StateAttach.state == null) {
                StateAttach.state = new StateAttach();
            }
            return StateAttach.state;
        }

        public void Do(CatController _controller, int _delta) {

            _controller.m_body.ApplyLinearImpulse(
                -_controller.m_body.LinearVelocity * _controller.m_body.Mass);

            //_controller.m_body.LinearVelocity = Vector2.Zero;

            _controller.m_body.IgnoreGravity = true;
            float hor_force = 0.0f;
            if (_controller.m_wantLeft) {
                hor_force += -1.0f;
            }
            if (_controller.m_wantRight) {
                hor_force += 1.0f;
            }
            float ver_force = 1.2f;
            if (_controller.m_wantDown) {
                ver_force = 0.0f;
            }
            else if (!_controller.m_wantUp) {
                ver_force = 1.0f;
            }
            // want up 
            else {
                ver_force = 1.2f;
            }

            if (_controller.m_wantJump) {
                _controller.m_body.ApplyForce(new Vector2(hor_force, ver_force) * _controller.WallJumpForce);
                _controller.CurrentState = StateJumpUp.GetState();
                _controller.m_body.IgnoreGravity = false;
            }
        }

        public void UpdateAnimation(CatController _controller, Animator _animator, int _timeLastFrame) {

            _animator.CheckPlayAnimation(_controller.m_aniAttach);
        }
    }

    class StateStealth : ControllState {
        static private StateStealth state;

        static public StateStealth GetState() {
            if (StateStealth.state == null) {
                StateStealth.state = new StateStealth();
            }
            return StateStealth.state;
        }

        public void Do(CatController _controller, int _delta) {
            if (_controller.IsOnGround() || _controller.TryExitStealth() == false) {
                if (!_controller.m_wantDown) {
                    _controller.TryExitStealth();
                }
                if (_controller.m_wantJump) {
                    _controller.DoJump(true);
                    // return?
                }
                // try
                if (_controller.m_wantKill && _controller.CanPerformStealthKill()) {
                    _controller.CurrentState = StateStealthKill.GetState();
                }
                // end try
                float hor_force = 0.0f;
                if (_controller.m_wantLeft) {
                    hor_force -= 1.0f;
                }
                if (_controller.m_wantRight) {
                    hor_force += 1.0f;
                }
                if (hor_force * hor_force > 0.0f) {
                    _controller.SetOrientationByFloat(hor_force);
                }
                hor_force *= _controller.StealthSpeed;
                float impluse = (hor_force - _controller.m_body.LinearVelocity.X) *
                    _controller.m_body.Mass;
                _controller.m_body.ApplyLinearImpulse(new Vector2(impluse, 0.0f));
            }
            else {
                _controller.TryExitStealth();
            }
        }

        public void UpdateAnimation(CatController _controller, Animator _animator, int _timeLastFrame) {

            _animator.CheckPlayAnimation(_controller.m_aniStand);
            // TODO: should be stealth
        }
    }

    class StateStealthKill : ControllState {
        static private int moveInMS = 400;

        private int m_phase = 0; // 0 - to init, 1 - to move
        private int m_moveProgressInMS = 0;
        private Vector2 m_initPosition;

        private GameObject m_toBeKilled = null;

        static public StateStealthKill GetState() {
            StateStealthKill state = new StateStealthKill();
            state.m_phase = 0;
            return state;
        }

        public void Do(CatController _controller, int _delta) {
            if (m_phase == 0) { // to init
                m_toBeKilled = _controller.GetToBeKilled();
                if (m_toBeKilled == null) {
                    _controller.CurrentState = StateStealth.GetState();
                }
                // send commend to m_toBeKilled
                m_moveProgressInMS = 0;
                m_initPosition = _controller.m_body.Position;
                m_phase = 1;
            }
            else if (m_phase == 1) {    // to move
                m_moveProgressInMS += _delta;
                if(m_moveProgressInMS > moveInMS){
                    m_phase = 2;
                    m_moveProgressInMS = moveInMS;
                }
                float progress = (float)m_moveProgressInMS / moveInMS;
                Vector2 targetPosition = new Vector2(m_toBeKilled.AbsPosition.X,
                                                    m_toBeKilled.AbsPosition.Y);
                Vector2 setPosition = m_initPosition + (targetPosition - m_initPosition) * progress;
                _controller.m_body.SetTransform(setPosition, _controller.m_body.Rotation);
            }
            else if (m_phase == 2) {
                m_phase = 0;
                CanBeStealthKilled canBeStealthKilled = m_toBeKilled.GetComponent(typeof(CanBeStealthKilled))
                    as CanBeStealthKilled;
                if (canBeStealthKilled != null) {
                    canBeStealthKilled.Killed();
                }
                m_toBeKilled = null;
                System.Console.Out.WriteLine("Performing Stealth Killed");
                _controller.CurrentState = StateStealth.GetState();
            }
        }

        public void UpdateAnimation(CatController _controller, Animator _animator, int _delta) {
            /*throw new NotImplementedException();*/
        }
    }

    public class StateStealthKillToDeath : ControllState {
        static private StateStealthKillToDeath state;
        static public StateStealthKillToDeath GetState() {
            if (StateStealthKillToDeath.state == null) {
                StateStealthKillToDeath.state = new StateStealthKillToDeath();
            }
            return StateStealthKillToDeath.state;
        }

        public void Do(CatController _controller, int _delta) {
            _controller.GameObject.Scene._gameObjectList.RemoveGameObject(_controller.GameObject.GUID);
        }

        public void UpdateAnimation(CatController _controller, Animator _animator, int _delta) {

        }
    }

    class StateAttack : ControllState {

        private int m_phase = 0; // 0 - init, 1 - prepare, 2 - post
        private int m_currentMS = 0;

        static public StateAttack GetState() {
            return new StateAttack();
        }

        public void Do(CatController _controller, int _delta) {
            m_currentMS += _delta;
            if (_controller.m_currentAttackCooldownMS <= 0) {
                if (m_phase == 0) { // init
                    _controller.m_currentAttackCooldownMS = _controller.AttackCooldownTimeMS;
                    m_phase = 1;
                }
                if (m_phase == 1) { // prepare
                    if (m_currentMS > _controller.AttackPrepareTimeMS) {
                        m_currentMS -= _controller.AttackPrepareTimeMS;
                        m_phase = 2;
                        Attack(_controller);
                    }
                }
                if (m_phase == 2) { // post
                    if (m_currentMS > _controller.AttackPrepareTimeMS) {
                        _controller.CurrentState = StateStandWalk.GetState();
                    }
                }
            }
        }

        private void Attack(CatController _controller) {
            GameObject attackObject = null;
            if (_controller.AttackPrefabName != "") {
                string prefabName = _controller.AttackPrefabName;
                if (Mgr<CatProject>.Singleton != null
                    && Mgr<CatProject>.Singleton.prefabList != null) {
                    PrefabList prefabs = Mgr<CatProject>.Singleton.prefabList;
                    if (prefabs.ContainKey(prefabName)) {
                        Serialable.BeginSupportingDelayBinding();
                        attackObject = prefabs.GetItem(prefabName).DoClone() as GameObject;
                        Serialable.EndSupportingDelayBinding();
                    }
                }
            }
            if (attackObject != null) {
                Vector3 absPosition = _controller.GameObject.AbsPosition + new Vector3(0.0f, 0.0f, 0.01f);
                attackObject.Position = absPosition;
                OneTimeHurt oneTimeHurt = attackObject.GetComponent(typeof(OneTimeHurt)) as OneTimeHurt;
                if (oneTimeHurt != null) {
                    oneTimeHurt.BelongGUID = _controller.GameObject.GUID;
                }
                _controller.GameObject.Scene._gameObjectList.AddGameObject(attackObject);
            }
        }

        public void UpdateAnimation(CatController _controller, Animator _animator, int _delta) {
            /*throw new NotImplementedException();*/
        }
    }
}
