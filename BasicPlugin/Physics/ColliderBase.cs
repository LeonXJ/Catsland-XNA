using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Common;

namespace Catsland.Plugin.BasicPlugin {
    public class ColliderBase : CatComponent, Drawable{

#region Properties
        protected Body m_body;

        [SerialAttribute]
        protected readonly CatInteger m_bodyType;
        public BodyType BodyType {
            set {
                switch (value) {
                    case BodyType.Dynamic:
                        m_bodyType.SetValue(0);
                        break;
                    case BodyType.Kinematic:
                        m_bodyType.SetValue(1);
                        break;
                    case BodyType.Static:
                        m_bodyType.SetValue(2);
                        break;
                }
                if (m_body != null) {
                    m_body.BodyType = value;
                }
            }
            get {
                switch (m_bodyType.GetValue()) {
                    case 0:
                        return BodyType.Dynamic;
                    case 1:
                        return BodyType.Kinematic;
                    case 2:
                        return BodyType.Static;
                }
                return 0;
            }
        }
        protected Scene m_scene;

        [SerialAttribute]
        protected readonly CatFloat m_mass;
        public float Mass {
            set {
                m_mass.SetValue(value);
                if (m_body != null) {
                    m_body.Mass = value;
                }
            }
            get {
                return m_mass.GetValue();
            }
        }

        [SerialAttribute]
        protected readonly CatFloat m_friction;
        public float Friction {
            set{
                m_friction.SetValue(value);
                if(m_body != null){
                    m_body.Friction = value;
                }
            }
            get{
                return m_friction;
            }
        }

        [SerialAttribute]
        protected readonly CatFloat m_restitution;
        public float Restitution {
            set {
                m_restitution.SetValue(value);
                if (m_body != null) {
                    m_body.Restitution = value;
                }
            }
            get {
                return m_restitution;
            }
        }

        [SerialAttribute]
        protected readonly CatInteger m_collisionCategroy = new CatInteger((int)FixtureCollisionCategroy.Kind.SolidBlock);
        public FixtureCollisionCategroy.Kind CollisionCategroy {
            set {
                m_collisionCategroy.SetValue((int)value);
                UpdateCollisionCategroy();
            }
            get {
                return (FixtureCollisionCategroy.Kind)(m_collisionCategroy.GetValue());
            }
        }

        [SerialAttribute]
        protected readonly CatBool m_collide = new CatBool(true);
        public bool Collide {
            set {
                m_collide.SetValue(value);
                if (m_body != null) {
                    m_body.IsSensor = !value;
                }
            }
            get {
                return m_collide;
            }
        }

        // the vertex of collider debug box
        protected VertexPositionColor[] m_vertex;
        protected VertexBuffer m_vertexBuffer;

#endregion

        public ColliderBase()
            : base() {
           
            m_bodyType = new CatInteger(0);
            m_mass = new CatFloat(0.5f);
            m_friction = new CatFloat(0.2f);
            m_restitution = new CatFloat(0.0f);
        }

        public ColliderBase(GameObject _gameObject)
            : base(_gameObject){

            m_bodyType = new CatInteger(0);
            m_mass = new CatFloat(0.5f);
            m_friction = new CatFloat(0.2f);
            m_restitution = new CatFloat(0.0f);
        }

        public override void Initialize(Scene scene) {
            m_scene = scene;
            CreateAndConfigBody();
            UpdateDebugVertex();
        }

        public override void BindToScene(Scene scene) {
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                scene._debugDrawableList.AddItem(this);
            }
        }

        public override void UnbindFromScene(Scene _scene) {
            base.UnbindFromScene(_scene);
            DeleteBody();
        }

        protected void CreateAndConfigBody() {
            PhysicsSystem physicsSystem =
                m_scene.GetPhysicsSystem();
            if (physicsSystem != null) {
                 if (m_body != null) {
                     physicsSystem.GetWorld().RemoveBody(m_body);
                 }
                m_body = CreateBody(physicsSystem);
                m_body.BodyType = BodyType;
                m_body.Friction = m_friction;
                m_body.IsSensor = !m_collide;
                UpdateCollisionCategroy();
                MoveBodyToGameObject();
             }
        }

        protected void UpdateCollisionCategroy() {
            FixtureCollisionCategroy.SetCollsionCategroy(
                m_body, (FixtureCollisionCategroy.Kind)(m_collisionCategroy.GetValue()));
        }

        protected void DeleteBody() {
            PhysicsSystem physicsSystem =
               m_scene.GetPhysicsSystem();
            if (physicsSystem != null) {
                if (m_body != null) {
                    physicsSystem.GetWorld().RemoveBody(m_body);
                }
            }
        }

        virtual protected Body CreateBody(PhysicsSystem _physicsSystem){
            return BodyFactory.CreateBody(_physicsSystem.GetWorld());
        }

        virtual protected void UpdateDebugVertex() { }

        public override void EditorUpdate(int timeLastFrame) {
            base.EditorUpdate(timeLastFrame);

            if (m_body != null) {
                MoveBodyToGameObject();
            }
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);
            if (m_body != null) {
                if (m_body.BodyType == BodyType.Static) {
                    MoveBodyToGameObject();
                }
                else if (m_body.BodyType == BodyType.Dynamic) {
                    MoveGameObjectToBody();
                }
            }
        }

        private void MoveBodyToGameObject() {
            m_gameObject.ForceUpdateAbsTransformation();
           Vector3 absRotate = CatMath.MatrixToEulerAngleVector3(m_gameObject.AbsTransform);
           m_body.SetTransform(new Vector2(m_gameObject.AbsPosition.X,
                                                m_gameObject.AbsPosition.Y),
                                                               absRotate.Z);
        }

        // Warning: if this method is used, make sure the gameobject don't has parent
        private void MoveGameObjectToBody() {
            m_gameObject.Position = new Vector3(m_body.Position.X,
                                                        m_body.Position.Y,
                                                        m_gameObject.Position.Z);
            m_gameObject.RotationInDegree = new Vector3(m_gameObject.RotationInDegree.X,
                                                m_gameObject.RotationInDegree.Y,
                                                MathHelper.ToDegrees(m_body.Rotation));
        }

        virtual public void Draw(int timeLastFrame) {
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {

                BasicEffect effect = Mgr<DebugTools>.Singleton.DrawEffect;
                effect.View = Mgr<Camera>.Singleton.View;
                effect.Projection = Mgr<Camera>.Singleton.m_projection;
                effect.VertexColorEnabled = false;

                Transform transform;
                m_body.GetTransform(out transform);

                Matrix position = Matrix.CreateTranslation(new Vector3(transform.p.X,
                                                                       transform.p.Y,
                                                                       m_gameObject.AbsPosition.Z));
                Matrix rotation = Matrix.CreateFromYawPitchRoll(0.0f,
                                                                0.0f,
                                                                transform.q.GetAngle());
                effect.World = rotation * position;
                effect.DiffuseColor = new Vector3(0.0f, 0.5f, 0.5f);
                effect.Alpha = 1.0f;

                Mgr<GraphicsDevice>.Singleton.SetVertexBuffer(m_vertexBuffer);
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                    pass.Apply();
                    Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionColor>(
                        PrimitiveType.LineStrip,
                        m_vertex,
                        0,
                        m_vertex.Length - 1);
                }
            }
        }

        virtual public float GetDepth() {
            return m_gameObject.AbsPosition.Z;
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
