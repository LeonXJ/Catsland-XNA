using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using Catsland.Core;
using Catsland.Editor;
using System.ComponentModel;
using CatsEditor;
using System.Diagnostics;

namespace Catsland.Plugin.BasicPlugin {
    public class ParticleEmitter : CatComponent, Drawable, IParticleUpdater {

        #region  Properties

        // emitter
        [SerialAttribute]
        private readonly CatVector3 m_emitterSize = new CatVector3(0.1f, 0.1f, 0.1f);
        public Vector3 EmitterSize {
            set {
                m_emitterSize.SetValue(value);
            }
            get {
                return m_emitterSize;
            }
        }

        [SerialAttribute]
        private readonly CatVector3 m_emitterOffset = new CatVector3();
        public Vector3 EmitterOffset {
            set {
                m_emitterOffset.SetValue(value);
            }
            get {
                return m_emitterOffset;
            }
        }

        [SerialAttribute]
        private readonly CatBool m_isEmitting = new CatBool(true);
        public bool IsEmitting {
            set {
                m_isEmitting.SetValue(value);
            }
            get {
                return m_isEmitting;
            }
        }

        // generate particle
        [SerialAttribute]
        private readonly CatInteger m_generateRatePerSecond = new CatInteger(10);
        public int GenerateRatePerSecond {
            set {
                m_generateRatePerSecond.SetValue((int)MathHelper.Max(0, value));
            }
            get {
                return m_generateRatePerSecond;
            }
        }
        public CatInteger GenerateRatePerSecondRef {
            get {
                return m_generateRatePerSecond;
            }
        }

        [SerialAttribute]
        private readonly CatInteger m_maxParticleNumber = new CatInteger(20);
        public int MaxParticleNumber {
            set {
                m_maxParticleNumber.SetValue((int)MathHelper.Max(10, value));
                UpdateMaxParticleNumber();
            }
            get {
                return m_maxParticleNumber;
            }
        }

        // speed
        [SerialAttribute]
        private readonly CatVector3 m_emitDirection = new CatVector3(0, 1, 0);
        public Vector3 EmitDirection {
            set {
                Vector3 direction = value;
                direction.Normalize();
                m_emitDirection.SetValue(direction);
            }
            get {
                return m_emitDirection;
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_emitBiasDegree = new CatFloat();
        public float EmitBiasDegree {
            set {
                float degree = MathHelper.Max(0, value);
                m_emitBiasDegree.SetValue(degree);
            }
            get {
                return m_emitBiasDegree;
            }
        }
        private float m_emitBiasRadian { 
            get { 
                return MathHelper.ToRadians(EmitBiasDegree); 
            } 
        }


        [SerialAttribute]
        private readonly CatFloat m_emitSpeed = new CatFloat(0.5f);
        public float EmitSpeed {
            set {
                m_emitSpeed.SetValue(MathHelper.Max(0, value));
            }
            get {
                return m_emitSpeed;
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_emitSpeedBias = new CatFloat(0);
        public float EmitSpeedBias {
            set {
                m_emitSpeedBias.SetValue(MathHelper.Max(0, value));
            }
            get {
                return m_emitSpeedBias;
            }
        }

        [SerialAttribute]
        private readonly CatVector3 m_particleAcceleration = new CatVector3();
        public Vector3 ParticleAcceleration {
            set {
                m_particleAcceleration.SetValue(value);
            }
            get {
                return m_particleAcceleration;
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_angularVelocityOnZ = new CatFloat();
        public float AngularVelocityOnZ {
            set {
                m_angularVelocityOnZ.SetValue(value);
            }
            get {
                return m_angularVelocityOnZ;
            }
        }

        // particle
        [SerialAttribute]
        private readonly CatVector2 m_particleSize = new CatVector2(0.1f, 0.1f);
        public Vector2 ParticleSize {
            set {
                m_particleSize.X = MathHelper.Max(0, value.X);
                m_particleSize.Y = MathHelper.Max(0, value.Y);
            }
            get {
                return m_particleSize;
            }
        }

        [SerialAttribute]
        private readonly CatVector2 m_particleSizeBias = new CatVector2();
        public Vector2 ParticleSizeBias {
            set {
                m_particleSizeBias.SetValue(value);
            }
            get {
                return m_particleSizeBias;
            }
        }

        [SerialAttribute]
        private readonly CatInteger m_particleLifetimeInMS = new CatInteger(1000);
        public int ParticleLifetimeInMS {
            set {
                m_particleLifetimeInMS.SetValue((int)MathHelper.Max(value, 0));
            }
            get {
                return m_particleLifetimeInMS;
            }
        }

        [SerialAttribute]
        private string m_particleUpdateComponentName = typeof(ParticleEmitter).ToString();
        public string ParticleUpdaterComponentName {
            set {
                m_particleUpdateComponentName = value;
            }
            get {
                return m_particleUpdateComponentName;
            }
        }

        // inner
        private Particle[] m_particles;
        private Stack<int> m_freeParticleIndexes;

        private int m_accumulateMillionSecond;
        private Random m_random = new Random();

        public VertexBuffer m_vertexBuffer;
        public VertexPositionTexture[] m_vertex;

        #endregion

        public ParticleEmitter()
            : base() {
        }

        public ParticleEmitter(GameObject gameObject)
            : base(gameObject) {
        }

        public override void BindToScene(Scene scene) {
            base.BindToScene(scene);
            scene._renderList.AddItem(this);

        }

        // all the parameters should be set before init
        public override void Initialize(Scene scene) {
            base.Initialize(scene);

            UpdateMaxParticleNumber();
            GenerateVertex();
            m_accumulateMillionSecond = 0;
        }

        protected void ReleaseParticle() {
            if (m_particles != null) {
                foreach (Particle particle in m_particles) {
                    particle.Unbind();
                }
                m_particles = null;
            }
            if (m_freeParticleIndexes != null) {
                m_freeParticleIndexes.Clear();
                m_freeParticleIndexes = null;
            }
        }

        protected void UpdateMaxParticleNumber() {
            // release resource
            ReleaseParticle();
            // allocate resource
            m_particles = new Particle[m_maxParticleNumber];
            m_freeParticleIndexes = new Stack<int>();
            for (int i = 0; i < m_maxParticleNumber; ++i) {
                m_particles[i] = new Particle(this, i);
                m_particles[i].m_lifeInMS = -1;
                m_particles[i].BindToScene(Mgr<Scene>.Singleton);
                m_freeParticleIndexes.Push(i);
            }
        }

        protected void GenerateVertex() {
            m_vertex = new VertexPositionTexture[4];
            m_vertex[0] = new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0),
                                                    new Vector2(1, 0));
            m_vertex[1] = new VertexPositionTexture(new Vector3(0.5f,-0.5f, 0),
                                                    new Vector2(1, 1));
            m_vertex[2] = new VertexPositionTexture(new Vector3(-0.5f, 0.5f, 0),
                                                    new Vector2(0, 0));
            m_vertex[3] = new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0),
                                                    new Vector2(0, 1));
            m_vertexBuffer = new VertexBuffer(Mgr<GraphicsDevice>.Singleton,
                                  typeof(VertexPositionTexture), 4,
                                  BufferUsage.None);
            m_vertexBuffer.SetData<VertexPositionTexture>(m_vertex);
        }

        public override void Update(int timeLastFrame) {
            // generate particle
            if (m_isEmitting) {
                m_accumulateMillionSecond += timeLastFrame;
                // timeLastFrame past, so generateNum particles will be generated
                int generateNum = (int)(m_accumulateMillionSecond * m_generateRatePerSecond / 1000.0);
                m_accumulateMillionSecond -= (int)(generateNum * 1000 / (float)m_generateRatePerSecond);
                OneShot(generateNum);
            }

            // update living particle
            CatComponent particleUpdateBase = m_gameObject.GetComponent(m_particleUpdateComponentName);
            if (particleUpdateBase != null && particleUpdateBase is IParticleUpdater) {
                IParticleUpdater particleUpdate = (IParticleUpdater)particleUpdateBase;
                foreach (Particle particle in m_particles) {
                    if (particle.m_lifeInMS > 0) {
                        particleUpdate.ParticleUpdate(timeLastFrame, particle, this);
                    }
                }
            }
        }

        public void Draw(int timeLastFrame) {

        }

        public void OneShot(int number) {
            //m_isEmitting = false;
            GenerateParticles(number, m_gameObject.AbsPosition + m_emitterOffset);
        }

        public void OneShot(int number, Vector3 _position) {
            GenerateParticles(number, _position);
        }

        // generate a random position in _basePoint +- m_emitterSize
        private Vector3 RandomPosition(Vector3 _basePoint) {
            return _basePoint
                    + (new Vector3((float)m_random.NextDouble() - 0.5f, 
                        (float)m_random.NextDouble() - 0.5f, 
                        (float)m_random.NextDouble() - 0.5f) * m_emitterSize);
        }


        private Vector3 RotateVector(Vector3 _toBeRotated, Vector3 _axis, float _radian) {
            float halfCos = (float)Math.Cos(_radian);
            float halfSin = (float)Math.Sin(_radian);
            Quaternion axisQ = new Quaternion(_axis * halfSin, halfCos);
            Quaternion vectorQ = new Quaternion(_toBeRotated, 0);
            Quaternion result = Quaternion.Multiply(axisQ, vectorQ);
            return new Vector3(result.X, result.Y, result.Z);
        }

        private Vector3 RandomLinearVelocity() {
           
            Vector3 notParalleAxis = Vector3.UnitX;
            if (Math.Abs(Vector3.Dot(m_emitDirection, notParalleAxis)) > 0.99f) { // parallel
                notParalleAxis = Vector3.UnitY;
            }
            Vector3 padAxis = Vector3.Cross(m_emitDirection, notParalleAxis);
            // random angle in padAxis surface
            float padAngle = (float)(m_random.NextDouble() * Math.PI * 2);
            Vector3 newPadAix = RotateVector(padAxis, m_emitDirection, padAngle);
            // rotate axis
            Vector3 rotateAix = Vector3.Cross(m_emitDirection, newPadAix);
            float parAngle = (float)(m_random.NextDouble() * m_emitBiasRadian);
            Vector3 finalDirection = RotateVector(m_emitDirection, rotateAix, parAngle);

            finalDirection.Normalize();
            float speed = m_emitSpeed + (float)m_random.NextDouble() * m_emitSpeedBias;
            return finalDirection * speed;
        }

        private Vector2 RandomSize() {
            return m_particleSize + 
                m_particleSizeBias.GetValue() * (float)m_random.NextDouble();
        }

        protected Particle FetchFreeParticle() {
            if (m_freeParticleIndexes == null ||
                m_freeParticleIndexes.Count == 0) {
                return null;
            }
            else {
                int freeParticleIndex = m_freeParticleIndexes.Pop();
                return m_particles[freeParticleIndex];
            }
        }

        void GenerateParticles(int _number, Vector3 _position) {
            int i;
            for (i = 0; i < _number; ++i) {
                Particle particle = FetchFreeParticle();
                if (particle == null) {
                    break;
                }
                // random position
                particle.position = RandomPosition(_position);     
                particle.velocity = RandomLinearVelocity();
                // TODO: make it real
                particle.m_rotationZ = 0.0f;
                particle.m_angularVelocity = 0.0f;
                particle.m_size = RandomSize();
                particle.m_lifeInMS = m_particleLifetimeInMS;
            }
        }

        public override void UnbindFromScene(Scene _scene) {
            ReleaseParticle();
            base.UnbindFromScene(_scene);
        }

        public float GetDepth() {
            return m_gameObject.Position.Z + m_emitterOffset.Z;
        }

        public int CompareTo(object obj) {
            float otherDepth = ((Drawable)obj).GetDepth();
            float thisDepth = GetDepth();
            if (otherDepth > thisDepth) {
                return 1;
            }
            else if (otherDepth < thisDepth) {
                return -1;
            }
            return 0;
        }

        public override void EditorUpdate(int timeLastFrame) {
            base.EditorUpdate(timeLastFrame);
            Update(timeLastFrame);
        }

        public void KillParticle(Particle particle) {
            particle.m_lifeInMS = -1;
            m_freeParticleIndexes.Push(particle.m_index);
        }

        public void ParticleUpdate(int timeLastFrame, Particle particle, ParticleEmitter emitter) {
            particle.m_lifeInMS -= timeLastFrame;
            if (particle.m_lifeInMS <= 0) {
                particle.m_lifeInMS = 0;
                emitter.KillParticle(particle);
            }
            else {
                particle.velocity += m_particleAcceleration;
                particle.position += particle.velocity * timeLastFrame / 1000.0f;
                //particle.m_rotationZ += m_angularVelocityOnZ;
            }
        }

        // tool
        public static ParticleEmitter GetParticleEmitterOfGameObjectInCurrentScene(string _name) {
            GameObject gameObject =
                Mgr<Scene>.Singleton._gameObjectList.GetOneGameObjectByName(_name);
            if (gameObject != null) {
                return gameObject.GetComponent(typeof(ParticleEmitter))
                    as ParticleEmitter;
            }
            return null;
        }
    }

    public class Particle : Drawable {
        public Particle(ParticleEmitter _particleEmitter, int _index) {
            m_emitter = _particleEmitter;
            m_index = _index;
        }
        public int m_index;
        public ParticleEmitter m_emitter;
        public Vector3 velocity;
        public Vector3 position;
        public float m_rotationZ;
        public float m_angularVelocity;
        public Vector2 m_size;
        public int m_lifeInMS;
        private Scene m_scene;

        public void BindToScene(Scene _scene) {
            m_scene = _scene;
            _scene._renderList.AddItem(this);
        }

        public void Unbind() {
            m_scene._renderList.RemoveItem(this);
        }

        public void Draw(int timeLastFrame) {

            if (m_lifeInMS <= 0) {
                return;
            }

            // now just take it as basic effect
            Mgr<GraphicsDevice>.Singleton.SetVertexBuffer(m_emitter.m_vertexBuffer);
            ModelComponent modelComponent = (ModelComponent)m_emitter.m_gameObject.
                GetComponent(typeof(ModelComponent));
            Effect effect = null;
            Matrix matPosition = Matrix.CreateTranslation(position);
            Matrix matRotation = Matrix.CreateRotationZ(m_rotationZ);
            Matrix matScale = Matrix.CreateScale(m_size.X, m_size.Y, 1.0f);
            Matrix transform = Matrix.Multiply(Matrix.Multiply(matScale, matRotation), matPosition);
            if (modelComponent != null && modelComponent.Model != null) {
                CatMaterial material = modelComponent.GetCatModelInstance().GetMaterial();
                material.SetParameter("Alpha", new CatFloat((float)m_lifeInMS / m_emitter.ParticleLifetimeInMS));
                material.SetParameter("World", new CatMatrix(transform));
                material.SetParameter("View", new CatMatrix(Mgr<Camera>.Singleton.View));
                material.SetParameter("Projection", new CatMatrix(Mgr<Camera>.Singleton.m_projection));
                effect = material.ApplyMaterial();
            }
            else {
                effect = Mgr<DebugTools>.Singleton.DrawEffect;
                ((BasicEffect)effect).Alpha = (float)m_lifeInMS / m_emitter.ParticleLifetimeInMS;
                ((BasicEffect)effect).DiffuseColor = new Vector3(1.0f, 0.0f, 1.0f);
                ((BasicEffect)effect).View = Mgr<Camera>.Singleton.View;
                ((BasicEffect)effect).Projection = Mgr<Camera>.Singleton.m_projection;
                ((BasicEffect)effect).VertexColorEnabled = false;
                ((BasicEffect)effect).World = transform;
            }
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionTexture>(
                    PrimitiveType.TriangleStrip, m_emitter.m_vertex, 0, 2);
            }
            
        }

        public static string GetMenuNames() {
            return "Particle|Particle Emitter";
        }

        public float GetDepth() {
            return position.Z;
        }

        public int CompareTo(object obj) {
            float otherDepth = ((Drawable)obj).GetDepth();
            float thisDepth = GetDepth();
            if (otherDepth > thisDepth) {
                return 1;
            }
            else if (otherDepth < thisDepth) {
                return -1;
            }
            return 0;
        }

        
    }
}
