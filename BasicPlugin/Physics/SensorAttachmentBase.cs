﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catsland.Core;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Common;

namespace Catsland.Plugin.BasicPlugin {
    public abstract class SensorAttachmentBase {

#region Properties

        [SerialAttribute]
        protected readonly CatVector2 m_offset = new CatVector2();
        public Vector2 Offset {
            set {
                m_offset.SetValue(value);
                UpdateSensor();
                UpdateDebugShape();
            }
            get {
                return m_offset.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatBool m_enable = new CatBool(true);
        public bool Enable {
            set {
                m_enable.SetValue(value);
            }
            get {
                return m_enable;
            }
        }

        public enum SensorCollisionCategroy{
            EnvironmentSensor = (int)(FixtureCollisionCategroy.Kind.EnvironmentSensor),
            RoleSensor = (int)(FixtureCollisionCategroy.Kind.RoleSensor),
            AttachPointSensor = (int)(FixtureCollisionCategroy.Kind.AttachPointSensor),
        }

        [SerialAttribute]
        protected readonly CatInteger m_collisionCategroy = 
            new CatInteger((int)FixtureCollisionCategroy.Kind.EnvironmentSensor);
        public SensorCollisionCategroy CollisionCategroy {
            set {
                m_collisionCategroy.SetValue((int)value);
                UpdateSensor();
            }
            get {
                return (SensorCollisionCategroy)(m_collisionCategroy.GetValue());
            }
        }

        protected Body m_body;
        protected Fixture m_fixture;
        protected DebugShape m_debugShape;
        protected GameObject m_gameObject;

#endregion

        public SensorAttachmentBase(Body _body, GameObject _gameObject) {
            m_body = _body;
            m_gameObject = _gameObject;
        }

        public void BindToScene(Scene _scene) {
            UpdateSensor();
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                UpdateDebugShape();
                m_debugShape.BindToScene(_scene);
            }
        }

        public void RemoveFromScene(Scene _scene) {
            if (m_debugShape != null 
                && Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                m_debugShape.Destroy(_scene);
            }
        }

        protected void UpdateSensor() {
            if (m_fixture != null) {
                PhysicsSystem physicsSystem = m_gameObject.Scene.GetPhysicsSystem();
                m_body.DestroyFixture(m_fixture);
            }
            m_fixture = CreateSensor();
            m_fixture.IsSensor = true;
            FixtureCollisionCategroy.SetCollsionCategroy(m_fixture, (FixtureCollisionCategroy.Kind)(m_collisionCategroy.GetValue()));
            m_fixture.OnCollision += OnCollision;
            m_fixture.OnSeparation += OnSeparation;
        }

        protected void UpdateDebugShape() {
            if (m_debugShape == null) {
                m_debugShape = new DebugShape();
            }
            m_debugShape.DiffuseColor = Color.Green;
            UpdateDebugShapeVertex();
        }

        protected abstract void UpdateDebugShapeVertex();

        protected abstract Fixture CreateSensor();

        private bool OnCollision(Fixture _fixtureA, Fixture _fixtureB, Contact _contact) {
            return Collision(_fixtureA, _fixtureB, _contact);
        }

        protected abstract bool Collision(Fixture _fixtureA, Fixture _fixtureB, Contact _contact);

        private void OnSeparation(Fixture _fixtureA, Fixture _fixtureB) {
            Separation(_fixtureA, _fixtureB);
        }

        protected abstract void Separation(Fixture _fixtureA, Fixture _fixtureB);

        public void UpdateDebugShapePosition(int _timeLastFrame) {
            if (m_debugShape != null) {
                Transform transform;
                m_body.GetTransform(out transform);
                Vector2 position = transform.p + m_offset;
                m_debugShape.Position = new Vector3(position.X, position.Y, 0.0f);
            }
        }
    }
}
