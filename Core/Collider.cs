using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml;
using System.ComponentModel;

/**
 * @file Collider CatsComponent
 * 
 * Collider is a inherent CatsComponent.
 * it detect the collision between boundingBoxes.
 * it can also be used as trigger
 * 
 * @author LeonXie
 * */

namespace Catsland.Core {
    /**
     * @brief Collider does collision detection
     * */
    public class Collider : CatComponent, Drawable {
        // the root boundingBox of the boundingBox tree
        public CatsBoundingBox m_rootBounding;
        // the vertexBuffer to render the debug box
        VertexBuffer m_vertexBuffer;
        public VertexPositionColor[] m_vertex;
        int[] m_vertexNum;
        Vector2[] m_ZBound;

        // TODO: add triager selector
        // if it is a trigger, the TriggerInvoker will be
        // invoked when collided
        public string m_triagerInvoker;
        public string TriggerInvoker {
            get { return m_triagerInvoker; }
            set { m_triagerInvoker = value; }
        }
        // the set to hold the colliders which collided with this one
        // used to support complex trigger events: enter, stay, exit
        HashSet<Collider> m_invokers;
        // if true, enter, stay, exit events will be invoked, or only enter.
        private bool m_advanceTrigger = true; 
        public bool AdvantageTrigger {
            get { return m_advanceTrigger; }
            set { m_advanceTrigger = value; }
        }
        // collider just do collide test, while trigger will invoke invoker
        public enum ColliderType {
            Collider,
            NegitiveTrigger,
            PositiveTrigger
        };
        private ColliderType m_colliderType = ColliderType.Collider;
        public ColliderType ColliderTypeAttribute {
            get { return m_colliderType; }
            set { m_colliderType = value; }
        }


        /**
         * @brief quickly set the root boundingBox
         * */
        public Vector2 XBound {
            get { return m_rootBounding.m_XBound; }
            set {
                m_rootBounding.m_XBound = value;
                m_rootBounding.UpdateBoundFromRectangle();
                UpdateDebugBound();
            }
        }
        /**
         * @brief quickly set the root boundingBox
         * */
        public Vector2 YBound {
            get { return m_rootBounding.m_YBound; }
            set {
                m_rootBounding.m_YBound = value;
                m_rootBounding.UpdateBoundFromRectangle();
                UpdateDebugBound();
            }
        }
        /**
         * @brief quickly set the root boundingBox
         * */
        public Vector2 ZBound {
            get { return m_rootBounding.m_heightRange; }
            set {
                m_rootBounding.m_heightRange = value;
                m_rootBounding.UpdateBoundFromRectangle();
                UpdateDebugBound();
            }
        }

        public Collider(GameObject gameObject)
            : base(gameObject) {
        }

//         public override CatComponent CloneComponent(GameObject gameObject) {
//             Collider collider = new Collider(gameObject);
//             // clone bounding box?
// 
//             // TODO: correct?
//             collider.m_rootBounding = m_rootBounding;
//             if (m_triagerInvoker != null) {
//                 collider.m_triagerInvoker = m_triagerInvoker;
//             }
//             collider.m_advanceTrigger = m_advanceTrigger;
//             collider.m_colliderType = m_colliderType;
//             
//             return collider;
//         }

        public override void BindToScene(Scene scene) {
            base.BindToScene(scene);

            //scene._renderList.AddItem(this);
            scene._colliderList.AddItem(this);
            
            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                scene._debugDrawableList.AddItem(this);
            }

        }

        // before Initialize, m_rootBounding should be set.
        public override void Initialize(Scene scene) {
            base.Initialize(scene);

            if (m_rootBounding == null) {
                m_rootBounding = new CatsBoundingBox();
                m_rootBounding.UpdateBoundFromRectangle();
            }

            UpdateDebugBound();

            if (Mgr<GameEngine>.Singleton._gameEngineMode == GameEngine.GameEngineMode.MapEditor) {
                m_vertexBuffer = new VertexBuffer(Mgr<GraphicsDevice>.Singleton,
                    typeof(VertexPositionColor), m_vertex.Length, BufferUsage.None);
                m_vertexBuffer.SetData<VertexPositionColor>(m_vertex);
            }
            m_invokers = new HashSet<Collider>();
        }


        /**
         * @brief build the debug box according to boudingBox tree
         *
         * perform breath first search in boundingBox tree, build debug boxes
         * to m_vertex, m_vertex_num and m_ZBound
         * */
        public void UpdateDebugBound() {
            List<VertexPositionColor> vertex = new List<VertexPositionColor>();
            List<int> vertex_num = new List<int>();
            List<Vector2> ZBound = new List<Vector2>();
            // wfs
            Queue<CatsBoundingBox> searchQueue = new Queue<CatsBoundingBox>();
            searchQueue.Enqueue(m_rootBounding);
            while (searchQueue.Count > 0) {
                CatsBoundingBox curBoundingBox = searchQueue.Dequeue();
                // add base vertex
                foreach (Vector2 vector in curBoundingBox.m_convex) {
                    vertex.Add(new VertexPositionColor(new Vector3(vector.X,
                        vector.Y * Mgr<Scene>.Singleton._yCos, 0.0f),
                        Color.Red));
                }
                vertex.Add(new VertexPositionColor(new Vector3(curBoundingBox.m_convex[0].X,
                    curBoundingBox.m_convex[0].Y * Mgr<Scene>.Singleton._yCos, 0.0f),
                        Color.Red));
                vertex_num.Add(curBoundingBox.m_convex.Length + 1);
                // add low vertex
                ZBound.Add(new Vector2(curBoundingBox.m_heightRange.X, curBoundingBox.m_heightRange.Y));

                // add sub boundingbox
                if (curBoundingBox.m_subBounding != null) {
                    foreach (CatsBoundingBox boundingBox in curBoundingBox.m_subBounding) {
                        searchQueue.Enqueue(boundingBox);
                    }
                }
            }
            // generate vertex
            m_vertex = vertex.ToArray();
            m_vertexNum = vertex_num.ToArray();
            m_ZBound = ZBound.ToArray();
        }

        public override void Update(int timeLastFrame) {
            base.Update(timeLastFrame);
            // just do collide detection for trigger
            // collider will test collision in need
            if (m_colliderType == ColliderType.PositiveTrigger) {
                // get invoker component
                CatComponent invokerComponent = null;
                if (m_triagerInvoker != null) {
                    invokerComponent = m_gameObject.GetComponent(m_triagerInvoker);
                }
                if (invokerComponent == null) {
                    return;
                }
                // test for collide
                Collider[] colliders = Mgr<Scene>.Singleton._colliderList.JudgeCollides(m_rootBounding,
                    m_gameObject.AbsPositionOld, m_gameObject.AbsHeight, this);
                if (colliders != null) {
                    // for each collider, test and invoke corresponding event
                    HashSet<Collider> cur_collider = new HashSet<Collider>();
                    foreach (Collider collider in colliders) {
                        if (m_advanceTrigger == true) {
                            // for advanceTrigger, distinguish different events
                            if (m_invokers.Contains(collider)) {
                                // if it has collide in last frame, invoke the stay event
                                invokerComponent.InTrigger(this, collider);
                            }
                            else {
                                // if not appear in last frame, invoke enter event
                                invokerComponent.EnterTrigger(this, collider);
                                m_invokers.Add(collider);
                            }
                            cur_collider.Add(collider);
                        }
                        else {
                            // for simple trigger, just invoke inTrigger
                            invokerComponent.InTrigger(this, collider);
                        }
                    }
                    if (m_advanceTrigger == true) {
                        // test exit event
                        foreach (Collider collider in m_invokers) {
                            if (!cur_collider.Contains(collider)) {
                                // if in last frame but not in current frame, invoke exit event
                                invokerComponent.ExitTrigger(this, collider);
                            }
                        }
                        m_invokers = cur_collider;
                    }
                }
            }
        }

        /**
         * @brief test whether this collider collide with the given one
         * 
         * @param boundingBox the boundingBox to test
         * @param XYOffset the offset on XY of boundingBox
         * @param ZOffset the offset on Z of boundingBox
         * 
         * @result collide?
         * */
        public bool JudgeCollide(CatsBoundingBox boundingBox, Vector2 XYOffset, float ZOffest = 0.0f) {
            // TODO: Judge collide with bounding box
            return m_rootBounding.IsCollide(boundingBox,
                XYOffset, ZOffest,
                m_gameObject.AbsPositionOld,
                m_gameObject.AbsHeight);
        }


        /**
         * @brief draw debug frame
         * */
        void Drawable.Draw(int timeLastFrame) {
            // draw XYPlant boundingBox
            BasicEffect effect = Mgr<DebugTools>.Singleton.DrawEffect;
            effect.View = Mgr<Camera>.Singleton.View;
            effect.Projection = Mgr<Camera>.Singleton.m_projection;
            effect.VertexColorEnabled = false;


            foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                int baseIndex = 0;
                int i;

                Mgr<GraphicsDevice>.Singleton.SetVertexBuffer(m_vertexBuffer);

                for (i = 0; i < m_vertexNum.Length; ++i) {
                    int num = m_vertexNum[i];
                    // draw base
                    effect.World = Matrix.CreateTranslation(
                        new Vector3(m_gameObject.AbsPositionOld.X,
                        m_gameObject.AbsPositionOld.Y * Mgr<Scene>.Singleton._yCos,
                        0.0f));
                    effect.DiffuseColor = new Vector3(0.3f, 0.0f, 0.0f);
                    pass.Apply();
                    Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.LineStrip, m_vertex, baseIndex, num - 1);

                    // draw low bound
                    effect.World = Matrix.CreateTranslation(
                        new Vector3(m_gameObject.AbsPositionOld.X,
                        m_gameObject.AbsPositionOld.Y * Mgr<Scene>.Singleton._yCos
                            + (m_ZBound[i].X + m_gameObject.AbsHeight) * Mgr<Scene>.Singleton._ySin,
                        0.0f));
                    effect.DiffuseColor = new Vector3(0.3f, 0.6f, 0.3f);
                    pass.Apply();
                    Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.LineStrip, m_vertex, baseIndex, num - 1);

                    // draw up bound
                    effect.World = Matrix.CreateTranslation(
                        new Vector3(m_gameObject.AbsPositionOld.X,
                        m_gameObject.AbsPositionOld.Y * Mgr<Scene>.Singleton._yCos
                            + (m_ZBound[i].Y + m_gameObject.AbsHeight ) * Mgr<Scene>.Singleton._ySin,
                        0.0f));
                    effect.DiffuseColor = new Vector3(0.5f, 0.9f, 0.5f);
                    pass.Apply();
                    Mgr<GraphicsDevice>.Singleton.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.LineStrip, m_vertex, baseIndex, num - 1);

                    baseIndex += num;
                }
            }
        }

        void InvodeTriager(Collider invoker) {
        }

        public HitInfoPack VerticalDownRayHit(Vector2 _rayXY, float _rayHeight) {
            if (m_rootBounding != null) {
                Vector3 hitPoint = Vector3.Zero;
                if (m_rootBounding.VerticalDownRayHit(
                    _rayXY, _rayHeight, m_gameObject.AbsPositionOld, m_gameObject.AbsHeight, ref hitPoint)) {
                    return new HitInfoPack(hitPoint, m_gameObject);
                }
            }
            return HitInfoPack.NoHit;
        }

        /**
         * @brief save the collider to XML node
         * 
         * @param node
         * @param doc
         * 
         * @result
         * */
        public override bool SaveToNode(XmlNode node, XmlDocument doc) {
            XmlElement collider = doc.CreateElement("Collider");
            node.AppendChild(collider);

            collider.SetAttribute("isTrigger", "" + (m_colliderType == ColliderType.Collider ? false : true));
            // save trigger invoker
            collider.SetAttribute("invoker", "" + m_triagerInvoker);

            m_rootBounding.SaveToNode(collider, doc);

            return true;
        }


        /**
         * @brief configure the collider from XML node
         * 
         * @param node
         * @param scene
         * @param gameObject
         * 
         * @result
         * */
        public override void ConfigureFromNode(XmlElement node, Scene scene, GameObject gameObject) {
            base.ConfigureFromNode(node, scene, gameObject);

            bool isTrigger = bool.Parse(node.GetAttribute("isTrigger"));
            m_colliderType = isTrigger ? ColliderType.PositiveTrigger : ColliderType.Collider;

            // load invoker
            m_triagerInvoker = node.GetAttribute("invoker");
            XmlNode boundingBox = node.SelectSingleNode("BoundingBox");
            m_rootBounding = CatsBoundingBox.LoadFromNode(boundingBox, scene, gameObject);
        }

        public override void Destroy() {
            Mgr<Scene>.Singleton._colliderList.RemoveItem(this);
#if DEBUG
            Mgr<Scene>.Singleton._debugDrawableList.RemoveItem(this);
#endif
            base.Destroy();
        }

        public float GetDepth() {
            return m_gameObject.PositionOld.Y;
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

    public class HitInfoPack {
        public bool IsHit;
        public Vector3 HitPoint;
        public GameObject HitGameObject;
        public static HitInfoPack NoHit = new HitInfoPack();

        public HitInfoPack(Vector3 _hitPoint, GameObject _hitGameObject) {
            IsHit = true;
            HitPoint = _hitPoint;
            HitGameObject = _hitGameObject;
        }

        public HitInfoPack() {
            IsHit = false;
        }
    }
}
