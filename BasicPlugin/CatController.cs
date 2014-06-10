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
                m_body.Mass = value;
            }
            get{
                return m_mass.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_walkAcc = new CatFloat(1.0f);
        public float WalkAcc {
            set {
                m_walkAcc.SetValue(MathHelper.Max(0.0f, value));
            }
            get {
                return m_walkAcc.GetValue();
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
        private readonly CatFloat m_runAcc = new CatFloat(2.0f);
        public float RunAcc {
            set {
                m_runAcc.SetValue(MathHelper.Max(0.0f, value));
            }
            get {
                return m_runAcc.GetValue();
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
        private readonly CatFloat m_jumpForce = new CatFloat(2.0f);
        public float JumpForce {
            set {
                m_jumpForce.SetValue(MathHelper.Max(0.0f, value));
            }
            get {
                return m_jumpForce.GetValue();
            }
        }

        public Body m_body;
        private SensorAttachment m_onGroundSensor;
        private SensorAttachment m_leftAttachableSensor;
        private SensorAttachment m_rightAttachableSensor;

        private VertexPositionColor[] m_vertex;
        private VertexBuffer m_vertexBuffer;

        private ControllState m_currentState;
        public ControllState CurrentState {
            set {
                m_currentState = value;
            }
        }

        public bool m_wantLeft = false;
        public bool m_wantRight = false;
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
                                                 m_inSize.Y,
                                                 m_mass, "player");
            m_body.BodyType = BodyType.Dynamic;
            m_body.SleepingAllowed = false;
            m_body.FixedRotation = true;
            m_body.Position = new Vector2(0.0f, 0.0f);
        
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
                - m_inSize.Y / 2.0f - sensorGap - sensorHalfSize);
            // leftAttachableSensor
            if(m_leftAttachableSensor == null){
                m_leftAttachableSensor = new SensorAttachment(m_body);
                m_leftAttachableSensor.BindToScene(Mgr<Scene>.Singleton);
            }
            m_leftAttachableSensor.Size = new Vector2(sensorHalfSize, sensorHalfSize);
            m_leftAttachableSensor.Offset = new Vector2(-m_inSize.X /2.0f - sensorGap - sensorHalfSize,
                m_inSize.Y / 2.0f - sensorHalfSize);
            // rightAttachableSensor
            if(m_rightAttachableSensor == null){
                m_rightAttachableSensor = new SensorAttachment(m_body);
                m_rightAttachableSensor.BindToScene(Mgr<Scene>.Singleton);
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
                                                m_body.Position.Y + (m_outSize.Y - m_inSize.Y) / 2.0f,
                                                m_gameObject.Position.Z);
        }

        private void MoveBodyToGameObject(){
            m_gameObject.ForceUpdateAbsTransformation();
            m_body.SetTransform(new Vector2(m_gameObject.AbsPosition.X, 
                                            m_gameObject.AbsPosition.Y - (m_outSize.Y - m_inSize.Y) / 2.0f),
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
                                                                   position.Y + (m_outSize.Y - m_inSize.Y) / 2.0f,
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
                                                                        position.Y,
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

        public bool IsOnGround() {
            return m_onGroundSensor.IsTriggered;
        }

        public float GetDepth(){
            return 0;
        }

        public int CompareTo(object obj){
            return 1;
        }

        public void DoJump() {
            m_body.ApplyForce(new Vector2(0.0f, m_jumpForce));
            CurrentState = StateJumpUp.GetState();
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
                else{
                    float hor_force = 0.0f;
                    if (_controller.m_wantLeft) {
                        hor_force -= 1.0f;
                    }
                    if (_controller.m_wantRight) {
                        hor_force += 1.0f;
                    }

                    if (_controller.m_wantRun) {
                        hor_force *= _controller.RunAcc;
                        _controller.m_body.ApplyForce(new Vector2(hor_force, 0.0f));
                        if (_controller.m_body.LinearVelocity.X * _controller.m_body.LinearVelocity.X
                            > _controller.WalkSpeed * _controller.WalkSpeed) {
                            _controller.CurrentState = StateRun.GetState();
                        }
                    }
                    else {
                        hor_force *= _controller.WalkAcc;
                        // if different direction or same but not full speed
                        if (_controller.m_body.LinearVelocity.X * hor_force < 0 ||
                            _controller.m_body.LinearVelocity.X * _controller.m_body.LinearVelocity.X
                            < _controller.WalkSpeed * _controller.WalkSpeed) {
                            _controller.m_body.ApplyForce(new Vector2(hor_force, 0.0f));
                        }
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
                    float hor_force = 0.0f;
                    if(_controller.m_wantLeft){
                        hor_force -= 1.0f;
                    }
                    if(_controller.m_wantRight){
                        hor_force += 1.0f;
                    }
                    if (hor_v * hor_force > 0.0){
                        // same direction
                        // want run
                        if (_controller.m_wantRun) {
                            if (hor_v * hor_v < _controller.RunSpeed * _controller.RunSpeed) {
                                _controller.m_body.ApplyForce(new Vector2(hor_force * _controller.RunAcc, 0.0f));
                            }
                        }
                        // want walk
                        if (hor_v * hor_v < _controller.WalkSpeed * _controller.WalkSpeed) {
                            _controller.CurrentState = StateStandWalk.GetState();
                        }
                        // else do nothing and let it slow down
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
        }
    }

    class StateFall : ControllState {
        static private StateFall state;

        static public StateFall GetState() {
            if (StateFall.state == null) {
                StateFall.state = new StateFall();
            }
            return StateFall.state;
        }

        public void Do(CatController _controller) {
            if (_controller.IsOnGround()) {
                _controller.CurrentState = StateStandWalk.GetState();
            }
        }
    }
}
