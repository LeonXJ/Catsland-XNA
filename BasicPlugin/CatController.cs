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
        public float Mass{
            set{
                m_mass.SetValue(MathHelper.Max(0.0f, value));
                m_body.Mass = m_mass;
            }
            get{
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
        private readonly CatFloat m_airBiasForce = new CatFloat(0.2f);
        public float AirBiasForce {
            set {
                m_airBiasForce.SetValue(MathHelper.Max(0.0f, value));
            }
            get {
                return m_airBiasForce.GetValue();
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

        public Body m_body;
        public Fixture m_taller;
        private SensorAttachment m_onGroundSensor;
        private SensorAttachment m_jumpableSensor;
        private SensorAttachment m_leftAttachableSensor;
        private SensorAttachment m_rightAttachableSensor;

        private VertexPositionColor[] m_vertex;
        private VertexBuffer m_vertexBuffer;
        private int m_tallerSensorTouched = 0;

        private ControllState m_currentState;
        public ControllState CurrentState {
            set {
                m_currentState = value;
            }
        }

        public bool m_wantLeft = false;
        public bool m_wantRight = false;
        public bool m_wantUp = false;
        public bool m_wantDown = false;
        public bool m_wantRun = false;
        public bool m_wantJump = false;

#endregion

        public CatController()
            : base() {
        }

        public CatController(GameObject _gameObject)
            : base(_gameObject) {

        }

        public override void Destroy() {
            base.Destroy();
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
                                                 m_inSize.Y/2.0f,
                                                 m_mass, new Tag(0, m_inSize.Y/4.0f));
            
            m_body.BodyType = BodyType.Dynamic;
            m_body.SleepingAllowed = false;
            m_body.FixedRotation = true;
            m_body.Position = new Vector2(0.0f, 0.0f);
            // create tail part
            m_taller = FixtureFactory.AttachRectangle(m_inSize.X, m_inSize.Y / 2.0f, 0.01f,
                new Vector2(0.0f, m_inSize.Y / 2.0f), m_body);
            m_taller.OnCollision = OnCollision;
            m_taller.OnSeparation = OnSeparation;
            MoveBodyToGameObject();
        }

        protected void UpdateAnimation(){
            // TODO
        }



        protected void UpdateSensors(){
            // onGroundSensor
            if(m_onGroundSensor == null){
                m_onGroundSensor = new SensorAttachment(m_body);
                m_onGroundSensor.BindToScene(Mgr<Scene>.Singleton);
            }
            float sensorHalfSize = 0.03f;
            float sensorGap = 0.01f;
            m_onGroundSensor.Size = new Vector2(m_inSize.X, sensorHalfSize);
            m_onGroundSensor.Offset = new Vector2(0.0f, 
                - m_inSize.Y / 4.0f - sensorGap - sensorHalfSize);
            // jumpable sensor
            if (m_jumpableSensor == null) {
                m_jumpableSensor = new SensorAttachment(m_body);
                m_jumpableSensor.BindToScene(Mgr<Scene>.Singleton);
            }
            m_jumpableSensor.Size = new Vector2(m_inSize.X / 2.0f, sensorHalfSize);
            m_jumpableSensor.Offset = new Vector2(0.0f, 
                - m_inSize.Y / 4.0f - sensorGap - sensorHalfSize);
            // leftAttachableSensor
            if(m_leftAttachableSensor == null){
                m_leftAttachableSensor = new SensorAttachment(m_body);
                m_leftAttachableSensor.BindToScene(Mgr<Scene>.Singleton);
                m_leftAttachableSensor.AcceptTag = Tag.AttachPoint;
            }
            m_leftAttachableSensor.Size = new Vector2(sensorHalfSize, sensorHalfSize);
            m_leftAttachableSensor.Offset = new Vector2(-m_inSize.X /2.0f - sensorGap - sensorHalfSize,
                m_inSize.Y / 2.0f - sensorHalfSize);
            // rightAttachableSensor
            if(m_rightAttachableSensor == null){
                m_rightAttachableSensor = new SensorAttachment(m_body);
                m_rightAttachableSensor.BindToScene(Mgr<Scene>.Singleton);
                m_rightAttachableSensor.AcceptTag = Tag.AttachPoint;
            }
            m_rightAttachableSensor.Size = new Vector2(sensorHalfSize, sensorHalfSize);
            m_rightAttachableSensor.Offset = new Vector2(m_inSize.X /2.0f + sensorGap + sensorHalfSize,
                m_inSize.Y / 2.0f - sensorHalfSize);
        }

        public override void EditorUpdate(int timeLastFrame){
 	        base.EditorUpdate(timeLastFrame);
            MoveBodyToGameObject();
        }

        public override void BindToScene(Scene scene)
        {
 	        base.BindToScene(scene);
            if(Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor){
                if(scene != null){
                    scene._debugDrawableList.AddItem(this);
                }
            }
        }

        public override void Update(int timeLastFrame)
        {
 	        base.Update(timeLastFrame);
            MoveGameObjectToBody();
            /*UpdateSensors();*/
            DoControll(timeLastFrame);
            UpdateAnimation();
        }

        protected void DoControll(int _timeLastFrame){
            m_currentState.Do(this);
        }

        public void MoveGameObjectToBody(){
            // Warning: the gameObject should not have parent
            m_gameObject.Position = new Vector3(m_body.Position.X,
                                                m_body.Position.Y + m_inSize.Y / 4.0f + (m_outSize.Y - m_inSize.Y) / 2.0f,
                                                m_gameObject.Position.Z);
        }

        private void MoveBodyToGameObject(){
            m_gameObject.ForceUpdateAbsTransformation();
            m_body.SetTransform(new Vector2(m_gameObject.AbsPosition.X, 
                                            m_gameObject.AbsPosition.Y - (m_outSize.Y - m_inSize.Y) / 2.0f - m_inSize.Y/4.0f),
                                            0.0f);
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

        protected void UpdateRectangleVertex(VertexPositionColor []_vertex, 
                                             int _offset,
                                             float _width, float _height) {
            float halfWidth = _width / 2.0f;
            float halfHeight = _height / 2.0f;
            _vertex[_offset].Position = new Vector3(-halfWidth, halfHeight, 1);
            _vertex[_offset+1].Position = new Vector3(halfWidth, halfHeight, 1);
            _vertex[_offset+2].Position = new Vector3(halfWidth, -halfHeight, 1);
            _vertex[_offset+3].Position = new Vector3(-halfWidth, -halfHeight, 1);
            _vertex[_offset+4].Position = new Vector3(-halfWidth, halfHeight, 1);
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
                                                                   position.Y + m_inSize.Y /4.0f + (m_outSize.Y - m_inSize.Y) / 2.0f,
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
                                                                        position.Y + m_inSize.Y/4.0f,
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

        private bool IsTallBlocker(Fixture _fixture){
            if(_fixture.Body.UserData != null){
                if(Tag.Platform != _fixture.Body.UserData as Tag){
                    return true;
                }
            }
            return false;
        }

        protected bool OnCollision(Fixture _fixtureA, Fixture _fixtureB, Contact _contact) {
            if(IsTallBlocker(_fixtureA)){
                m_tallerSensorTouched += 1;
            }
            else if(IsTallBlocker(_fixtureB)){
                m_tallerSensorTouched += 1;
            }
            return true;
        }

        protected void OnSeparation(Fixture _fixtureA, Fixture _fixtureB) {
            if (IsTallBlocker(_fixtureA)) {
                m_tallerSensorTouched -= 1;
            }
            else if (IsTallBlocker(_fixtureB)) {
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

        public float GetDepth(){
            return 0;
        }

        public int CompareTo(object obj){
            return 1;
        }

        public void DoJump(bool _isStealth = false) {
            if (!m_jumpableSensor.IsTriggered) {
                return;
            }
            if (m_wantDown) {
                if (m_jumpableSensor.LastContactFixture != null
                    && m_jumpableSensor.LastContactFixture.Body != null) {
                        Tag bodyTag = m_jumpableSensor.LastContactFixture.Body.UserData as Tag;
                    if (bodyTag != null && Tag.Platform == bodyTag) {
                        StateFall.GetState().IgnoreBody = m_jumpableSensor.LastContactFixture.Body;
                        m_body.IgnoreCollisionWith(m_jumpableSensor.LastContactFixture.Body);
                        CurrentState = StateFall.GetState();
                    }
                }
            }
            else {
                if (!_isStealth || TryExitStealth()) {
                    m_body.ApplyForce(new Vector2(0.0f, m_jumpForce));
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
        void Do(CatController _controller);
    }

    class StateStandWalk : ControllState {
        static private StateStandWalk state;

        static public StateStandWalk GetState() {
            if (StateStandWalk.state == null) {
                StateStandWalk.state = new StateStandWalk();
            }
            return StateStandWalk.state;
        }

        public void Do(CatController _controller) {
            if (_controller.IsOnGround()) {
                // jump
                if (_controller.m_wantJump) {
                    _controller.DoJump();
                }
                // left / right
                else if (_controller.m_wantDown) {
                    _controller.EnterStealth();
                }
                else{
                    float hor_vel = 0.0f;
                    if (_controller.m_wantLeft) {
                        hor_vel -= 1.0f;
                    }
                    if (_controller.m_wantRight) {
                        hor_vel += 1.0f;
                    }

                    if (_controller.m_wantRun) {
                        _controller.CurrentState = StateRun.GetState();


// 
//                         hor_vel *= _controller.RunAcc;
//                         _controller.m_body.ApplyForce(new Vector2(hor_vel, 0.0f));
//                         if (_controller.m_body.LinearVelocity.X * _controller.m_body.LinearVelocity.X
//                             > _controller.WalkSpeed * _controller.WalkSpeed) {
//                            
/*                        }*/
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
    }

    class StateRun : ControllState {
        static private StateRun state;

        static public StateRun GetState() {
            if (StateRun.state == null) {
                StateRun.state = new StateRun();
            }
            return StateRun.state;
        }

        public void Do(CatController _controller) {
            if (_controller.IsOnGround()) {
                // jump
                if (_controller.m_wantJump) {
                    _controller.DoJump();
                }
                else {
                    // want walk and same direction and slow enough,
                    // if not slow enough, just do nothing and let it slow down
                    float hor_v = _controller.m_body.LinearVelocity.X;
                    float hor_desired_vel = 0.0f;
                    if(_controller.m_wantLeft){
                        hor_desired_vel -= 1.0f;
                    }
                    if(_controller.m_wantRight){
                        hor_desired_vel += 1.0f;
                    }
                    if (hor_v * hor_desired_vel > 0.0 || hor_v * hor_v < 0.01f){
                        // same direction
                        // want run, acc or keep speed
                        if (_controller.m_wantRun && !_controller.m_wantDown) {
                            hor_desired_vel *= 
                                MathHelper.Min(Math.Abs(hor_v) + _controller.RunAcc, _controller.RunSpeed);
                            
                        }
                        // slowing down
                        else{
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
    }

    class StateBreak : ControllState {
        static private StateBreak state;

        static public StateBreak GetState() {
            if (StateBreak.state == null) {
                StateBreak.state = new StateBreak();
            }
            return StateBreak.state;
        }

        public void Do(CatController _controller) {
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
    }

    class StateJumpUp : ControllState {
        static private StateJumpUp state;

        static public StateJumpUp GetState() {
            if (StateJumpUp.state == null) {
                StateJumpUp.state = new StateJumpUp();
            }
            return StateJumpUp.state;
        }

        public void Do(CatController _controller) {
            if (_controller.m_body.LinearVelocity.Y <= 0.0f) {
                _controller.CurrentState = StateFall.GetState();
            }
            if (_controller.IsRightAttachable() && _controller.m_wantRight) {
                _controller.CurrentState = StateAttach.GetState();
            }
            else if (_controller.IsLeftAttachable() && _controller.m_wantLeft) {
                _controller.CurrentState = StateAttach.GetState();
            }
            float ver_force = 0.0f;
            if (_controller.m_wantJump) {
                ver_force = _controller.JumpLiftForce;
            }
            float hor_force = 0.0f;
            if (_controller.m_wantLeft) {
                hor_force -= 1.0f;
            }
            if (_controller.m_wantRight) {
                hor_force += 1.0f;
            }
            float hor_vel = _controller.m_body.LinearVelocity.X;
            if (hor_vel * hor_force > 0.0f && hor_vel * hor_vel > _controller.RunSpeed * _controller.RunSpeed) {
                hor_force = 0.0f;
            }
            if (hor_force * hor_force > 0.1f || ver_force > 0.1f) {
                _controller.m_body.ApplyForce(new Vector2(hor_force * _controller.AirBiasForce, ver_force));
            }
        }
    }

    class StateFall : ControllState {
        static private StateFall state;
        private Body m_ignoreBody;
        public Body IgnoreBody {
            set {
                m_ignoreBody = value;
            }
        }

        static public StateFall GetState() {
            if (StateFall.state == null) {
                StateFall.state = new StateFall();
            }
            return StateFall.state;
        }

        private void RestoreIgnoreBody(Body _body) {
            if (m_ignoreBody != null) {
                _body.RestoreCollisionWith(m_ignoreBody);
            }
            m_ignoreBody = null;
        }

        public void Do(CatController _controller) {
            if (_controller.IsOnGround()) {
                RestoreIgnoreBody(_controller.m_body);
                _controller.CurrentState = StateStandWalk.GetState();
            }
            if (_controller.IsRightAttachable() && _controller.m_wantRight) {
                RestoreIgnoreBody(_controller.m_body);
                _controller.CurrentState = StateAttach.GetState();
            }
            else if (_controller.IsLeftAttachable() && _controller.m_wantLeft) {
                RestoreIgnoreBody(_controller.m_body);
                _controller.CurrentState = StateAttach.GetState();
            }
            float hor_force = 0.0f;
            if (_controller.m_wantLeft) {
                hor_force -= 1.0f;
            }
            if (_controller.m_wantRight) {
                hor_force += 1.0f;
            }
            if (hor_force * hor_force > 0.1f) {
                _controller.m_body.ApplyForce(new Vector2(hor_force * _controller.AirBiasForce, 0.0f));
            }
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

        public void Do(CatController _controller) {
            _controller.m_body.LinearVelocity = Vector2.Zero;
            _controller.m_body.IgnoreGravity = true;
            float hor_force = 0.0f;
            if (_controller.m_wantLeft) {
                hor_force += -1.0f;
            }
            if (_controller.m_wantRight) {
                hor_force += 1.0f;
            }
            float ver_force = 1.0f;
            if (_controller.m_wantDown && !_controller.m_wantUp) {
                ver_force = 0.0f;
            }
            if (_controller.m_wantJump) {
                _controller.m_body.ApplyForce(new Vector2(hor_force, ver_force) * _controller.WallJumpForce);
                _controller.CurrentState = StateJumpUp.GetState();
                _controller.m_body.IgnoreGravity = false;
            }
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

        public void Do(CatController _controller) {
            if (_controller.IsOnGround()) {
                if (!_controller.m_wantDown) {
                    _controller.TryExitStealth();
                }
                if (_controller.m_wantJump) {
                    _controller.DoJump(true);
                }
                    float hor_force = 0.0f;
                    if (_controller.m_wantLeft) {
                        hor_force -= 1.0f;
                    }
                    if (_controller.m_wantRight) {
                        hor_force += 1.0f;
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
    }
}
