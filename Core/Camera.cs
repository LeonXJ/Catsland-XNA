using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

/**
 * @file the camera
 *
 * @author LeonXie
 */

namespace Catsland.Core {
    public class Camera : Serialable {

        // view
        private GameObject m_targetObject = null;
        public GameObject TargetObject {
            set {
                m_targetObject = value;
                UpdateView();
            }
            get {
                return m_targetObject;
            }
        }

        [SerialAttribute]
        private readonly CatVector3 m_targetPosition = new CatVector3(Vector3.Zero);
        public Vector3 TargetPosition {
            set {
                m_targetObject = null;
                m_targetPosition.SetValue(value);
                UpdateView();
            }
            get {
                return m_targetPosition.GetValue();
            }
        }
        public CatVector3 TargetPositionRef {
            get {
                return m_targetPosition;
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_cameraDistance = new CatFloat(2.0f);
        public float CameraDistance {
            set {
                m_cameraDistance.SetValue(value);
                UpdateView();
            }
            get {
                return m_cameraDistance.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatVector3 m_cameraPRY = new CatVector3(0.0f, 0.0f, 0.0f);  // pitch row yaw
        public Vector3 PRY {
            set {
                m_cameraPRY.SetValue(value);
                UpdateView();
            }
            get {
                return m_cameraPRY.GetValue();
            }
        }

        // projection
        [SerialAttribute]
        private readonly CatVector2 m_viewSize = new CatVector2(2.0f, 1.0f);
        public Vector2 ViewSize {
            set {
                m_viewSize.SetValue(value);
                UpdateProjection();
            }
            get {
                return m_viewSize.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatFloat m_fieldOfView = new CatFloat(90.0f);
        public float FOV {
            set {
                m_fieldOfView.SetValue(value);
                UpdateProjection();
            }
            get {
                return m_fieldOfView.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatVector2 m_clipDistance = new CatVector2(0.01f, 1000.0f);
        public Vector2 ClipDistance {
            set {
                m_clipDistance.SetValue(value);
                UpdateProjection();
            }
            get {
                return m_clipDistance.GetValue();
            }
        }

        [SerialAttribute]
        private readonly CatColor m_backgroundColor = new CatColor(Color.LightBlue.ToVector4());
        public Color BackgroundColor {
            set {
                m_backgroundColor.SetValue(value);
            }
            get {
                return m_backgroundColor;
            }
        }


        private bool m_forceUpdateMatrixByCamera = false;
        public bool ForceUpdateMatrixByCameraUpdate {
            set {
                m_forceUpdateMatrixByCamera = value;
            }
            get {
                return m_forceUpdateMatrixByCamera;
            }
        }

        // the following matrices are set by functions, readonly for outside
        private Matrix m_view;
        public Matrix View {
            get {
                return m_view;
            }
        }
        public Matrix m_projection;
        public Matrix Projection {
            get {
                return m_projection;
            }
        }

        public enum ProjectionMode{
            Orthographic = 0,
            Perspective,
        }
        
        private ProjectionMode m_projectionMode = ProjectionMode.Orthographic;
        public ProjectionMode CameraProjectionMode {
            set {
                m_projectionMode = value;
                UpdateProjection();
            }
            get {
                return m_projectionMode;
            }
        }

        private Vector3 m_oldVelocity;
        private float m_velocitySmooth = 0.2f;
        private bool m_isPreviousPositionValid = false;
        private Vector3 m_previousPosition;
        private Vector3 m_velocity;
        public Vector3 Velocity {
            get{
                return m_velocity;
            }
        }

        // TODO: Wait for update
        // begin
        public float maxWidth = 3.0f;
        public float MaxCameraWidth {
            get { return maxWidth; }
            set { maxWidth = value; }
        }
        public float minWidth = 2.0f;
        public float MinCameraWidth {
            get { return minWidth; }
            set { minWidth = value; }
        }

        public int targetResolutionWidth = 800;
        public int TargetMaxResolutionWidth {
            get { return targetResolutionWidth; }
            set { targetResolutionWidth = value; }
        }
        // end: Wait for update

        // the position of the camera, set by updateView
        // don't modify the value manually
        private readonly CatVector3 m_cameraPosition = new CatVector3(Vector3.Zero);
        public Vector3 CameraPosition {
            get {
                return m_cameraPosition.GetValue();
            }
        }

        public float m_editorCameraMovingSpeed = 0.01f;
        public float EditorCameraMovingSpeed {
            set {
                m_editorCameraMovingSpeed = value;
            }
            get {
                return m_editorCameraMovingSpeed;
            }
        }

        // relative move 
        public void Translate(Vector3 _offset) {
            TargetPosition = TargetPosition + _offset;
        }

        public void Rotate(Vector3 _offset) {
            PRY = PRY + _offset;
        }

        public void ZoomViewSize(Vector2 _offset) {
            ViewSize = ViewSize * _offset;
        }

        public void ZoomFieldOfView(float _offset) {
            FOV = FOV + _offset;
        }

        public void ZoomCameraDistance(float _offset) {
            CameraDistance = CameraDistance + _offset;
        }

        public void SetProjectionMode(ProjectionMode _projectionMode) {
            m_projectionMode = _projectionMode;
            UpdateProjection();
        }

        public Vector3 Forward {
            get {
                return Vector3.Normalize(TargetPosition - CameraPosition);
            }
        }

        public Vector3 Up {
            get {
                return Vector3.UnitY;
            }
        }

        public void SetViewSizeByReservingWidth(int _width, int _height) {
            ViewSize = new Vector2(ViewSize.X, _height * ViewSize.X / _width);
        }

         

        /**
         * @brief Update m_view matrix
         * */
        public void UpdateView() {
            if (m_targetObject != null) {
                m_targetPosition.SetValue(m_targetObject.AbsPosition);
            }
            m_cameraPosition.SetValue(m_targetPosition + m_cameraDistance 
                * new Vector3((float)(Math.Cos(m_cameraPRY.X) * Math.Sin(m_cameraPRY.Z)),
                                                            (float)Math.Sin(m_cameraPRY.X),
                                                            (float)(Math.Cos(m_cameraPRY.X) * Math.Cos(m_cameraPRY.Z))));
            m_view = Matrix.CreateLookAt(CameraPosition, m_targetPosition, Vector3.UnitY);
        }

        /**
         * @brief Update m_projection matrix
         * */
        public void UpdateProjection() {
            if (m_projectionMode == ProjectionMode.Orthographic) {
                m_projection = Matrix.CreateOrthographic(m_viewSize.X, m_viewSize.Y, m_clipDistance.X, m_clipDistance.Y);
            }
            else if (m_projectionMode == ProjectionMode.Perspective) {
                m_projection = Matrix.CreatePerspective(m_viewSize.X, m_viewSize.Y, m_clipDistance.X, m_clipDistance.Y/10);
                m_projection = Matrix.CreatePerspectiveFieldOfView((float)(m_fieldOfView * Math.PI / 180.0f), m_viewSize.X / m_viewSize.Y, m_clipDistance.X, m_clipDistance.Y);
            }
        }

        /**
         * @brief automatically pose the camera, if target exists
         *
         * this function will be invoked by engine automatically
         */
        public void Update(int timeLastFrame) {
            if (m_targetObject != null) {
                UpdateView();
            }
            // velocity
            if (!m_isPreviousPositionValid) {       
                m_isPreviousPositionValid = true;
            }
            else {
                Vector3 deltaPosition = m_cameraPosition - m_previousPosition;
                m_velocity = m_oldVelocity * m_velocitySmooth +
                    (1.0f - m_velocitySmooth) * deltaPosition * 1000.0f / timeLastFrame;
                m_oldVelocity = m_velocity;
            }
            m_previousPosition = m_cameraPosition;
            //EditorUpdate();
            if (m_forceUpdateMatrixByCamera) {
                UpdateProjection();
                UpdateView();
            }
        }

        public void EditorUpdate() {
            KeyboardState keyboardState = Keyboard.GetState();
            Vector3 offsetDirection = Vector3.Zero;

            if (Mgr<GameEngine>.Singleton.IsGetFocus() == false) {
                return;
            }

            if (keyboardState.IsKeyDown(Keys.W)) {
                offsetDirection = Vector3.UnitY;
                Translate(offsetDirection * EditorCameraMovingSpeed);
                //MoveBiasIn2D(Vector2.UnitY * m_editorCameraMoveSpeed);
            }
            if (keyboardState.IsKeyDown(Keys.S)) {
                offsetDirection = -Vector3.UnitY;
                Translate(offsetDirection * EditorCameraMovingSpeed);
                //MoveBiasIn2D(Vector2.UnitY * -m_editorCameraMoveSpeed);
            }
            if (keyboardState.IsKeyDown(Keys.A)) {
                offsetDirection = -Vector3.UnitX;
                Translate(offsetDirection * EditorCameraMovingSpeed);
                //MoveBiasIn2D(Vector2.UnitX * -m_editorCameraMoveSpeed);
            }
            if (keyboardState.IsKeyDown(Keys.D)) {
                offsetDirection = Vector3.UnitX;
                Translate(offsetDirection * EditorCameraMovingSpeed);
                //MoveBiasIn2D(Vector2.UnitX * m_editorCameraMoveSpeed);
            }
            if (keyboardState.IsKeyDown(Keys.Q)) {
                ZoomViewSize(new Vector2(0.99f, 0.99f));
                //ScaleWidthBias(-0.01f);
            }
            if (keyboardState.IsKeyDown(Keys.E)) {
                ZoomViewSize(new Vector2(1.01f, 1.01f));
                //ScaleWidthBias(0.01f);
            }
//             if (keyboardState.IsKeyDown(Keys.Z)) {
//                 Rotate(new Vector3(0.0f, 0.0f, -0.1f));
//             }
//             if (keyboardState.IsKeyDown(Keys.X)) {
//                 Rotate(new Vector3(0.0f, 0.0f, 0.1f));
//             }
//             if (keyboardState.IsKeyDown(Keys.R)) {
//                 Rotate(new Vector3(-0.1f, 0.0f, 0.0f));
//             }
//             if (keyboardState.IsKeyDown(Keys.F)) {
//                 Rotate(new Vector3(0.1f, 0.0f, 0.0f));
//             }
//             // projection mode
//             if (keyboardState.IsKeyDown(Keys.P)) {
//                 SetProjectionMode(ProjectionMode.Perspective);
//             }
//             if (keyboardState.IsKeyDown(Keys.O)) {
//                 SetProjectionMode(ProjectionMode.Orthographic);
//             }
//             // camera distance
//             if (keyboardState.IsKeyDown(Keys.I)) {
//                 ZoomCameraDistance(-0.01f);
//             }
//             if (keyboardState.IsKeyDown(Keys.K)) {
//                 ZoomCameraDistance(0.01f);
//             }
//             // field of view
//             if (keyboardState.IsKeyDown(Keys.J)) {
//                 ZoomFieldOfView(-0.1f);
//             }
//             if (keyboardState.IsKeyDown(Keys.L)) {
//                 ZoomFieldOfView(0.1f);
//             }
        }

        public void Reset() {
            // view
            TargetPosition = Vector3.Zero;
            PRY = Vector3.Zero;
            CameraDistance = 2.0f;
            // projection
            CameraProjectionMode = ProjectionMode.Orthographic;
            ViewSize = new Vector2(2.0f, 1.0f);
            FOV = 45.0f;
            ClipDistance = new Vector2(0.01f, 1000.0f);
            // update
            UpdateView();
            UpdateProjection();
        }

        /**
         * @brief translate point in camera x,y \in [-1,1] to world pos
         * 
         * @param _cameraX position in camera
         * @param _cameraY position in camera
         * @param _y position in world
         * @param _result (out) result if intersects
         * 
         * @result intersect?
         * */
        public bool CameraToWorldXZ(float _cameraX, float _cameraY, float _y, out Vector3 _result) {
            Vector3 posNear = CameraToWorld(new Vector3(_cameraX, _cameraY, 0.01f));
            Vector3 posFar = CameraToWorld(new Vector3(_cameraX, _cameraY, 1.0f));
            Vector3 dir = posFar - posNear;
            // no intersect point
            if( dir.Y < float.MinValue && dir.Y > -float.MinValue){
                _result = Vector3.Zero;
                return false;
            }
            // has intersect point
            float t = (_y - posNear.Y) / dir.Y;
            _result = posNear + dir * t;
            return true;
        }

        /**
         * @brief translate point in camera x,y \in [-1,1] to world pos
         * 
         * @param _cameraX position in camera
         * @param _cameraY position in camera
         * @param _z position in world
         * @param _result (out) result if intersects
         * 
         * @result intersect?
         * */
        public bool CameraToWorldXY(float _cameraX, float _cameraY, float _z, out Vector3 _result) {
            Vector3 posNear = CameraToWorld(new Vector3(_cameraX, _cameraY, 0.01f));
            Vector3 posFar = CameraToWorld(new Vector3(_cameraX, _cameraY, 1.0f));
            Vector3 dir = posFar - posNear;
            // no intersect point
            if (dir.Z < float.MinValue && dir.Z > -float.MinValue) {
                _result = Vector3.Zero;
                return false;
            }
            // has intersect point
            float t = (_z - posNear.Z) / dir.Z;
            _result = posNear + dir * t;
            return true;
        }

        /**
         * @brief translate point in projection space to world pos
         * 
         * @param _positionInCamera projection coordinate
         * 
         * @result world coordinate
         * */
        public Vector3 CameraToWorld(Vector3 _positionInCamera) {
            Matrix inverseViewProj = Matrix.Invert(m_view * m_projection);
            Vector4 worldPosition = Vector4.Transform(
                new Vector4(_positionInCamera.X, _positionInCamera.Y, _positionInCamera.Z, 1.0f),
                inverseViewProj);
            return new Vector3(worldPosition.X, worldPosition.Y, worldPosition.Z);
        } 
    }
}
