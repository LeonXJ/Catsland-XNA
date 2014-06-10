using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

/**
 * @file the list of Collider
 * 
 * @author LeonXie
 * */

namespace Catsland.Core {
    /**
     * @brief the list of Collider
     * */
    public class ColliderList : RepeatableList<Collider> {
        List<Collider> m_removeList;

        // TODO: make it a quad-tree

        // skip iCollider and the boundingBox belongs to iCollider, however, 
        // boundingBox may be different from iCollider.m_rootBoundingBox, because boundingBox maybe different offset

        /**
         * @brief test whether boundingBox collide with any others, return the first collider it collides with
         * 
         * @param boundingBox the boundingBox want to test
         * @param XYOffset the offset of boundingBox on XY
         * @param ZOffset the offset of boundingBox on Z
         * @param iCollider the collider boundingBox belongs to, in order to avoid testing itself
         * 
         * @result the first colliding Collider if exists
         * */
        public Collider JudgeCollide(CatsBoundingBox boundingBox, Vector2 XYOffset, float ZOffset = 0.0f, Collider iCollider = null) {
            if (contentList == null) {
                return null;
            }
            foreach (Collider collider in contentList) {
                // skip itself
                if (collider == iCollider) {
                    continue;
                }
                bool result = collider.JudgeCollide(boundingBox, XYOffset, ZOffset);
                if (result == true) {
                    if (collider.ColliderTypeAttribute == Collider.ColliderType.Collider) {
                        return collider;
                    }
                }
            }
            return null;
        }

        public HitInfoPack VerticalDownRayHit(Vector2 _rayXY, float _rayHeight, Collider _ignore) {
            if (contentList == null) {
                return HitInfoPack.NoHit;
            }
            HitInfoPack resHitInfoPack = HitInfoPack.NoHit;
            float maxHeightHitPoint = 0.0f;
            foreach (Collider collider in contentList) {
                if (collider == _ignore) {
                    continue;
                }
                if (collider.ColliderTypeAttribute != Collider.ColliderType.Collider) {
                    continue;
                }
                HitInfoPack hitInfoPack = collider.VerticalDownRayHit(_rayXY, _rayHeight);
                if (hitInfoPack.IsHit && hitInfoPack.HitPoint.Z > maxHeightHitPoint) {
                    maxHeightHitPoint = hitInfoPack.HitPoint.Z;
                    resHitInfoPack = hitInfoPack;
                }
            }
            
            return resHitInfoPack;
        }

        /**
         * @brief test whether boundingBox collide with any others, return all colliding Colliders
         * 
         * @param boundingBox the boundingBox want to test
         * @param XYOffset the offset of boundingBox on XY
         * @param ZOffset the offset of boundingBox on Z
         * @param iCollider the collider boundingBox belongs to, in order to avoid testing itself
         * 
         * @result the colliding Colliders if exists
         * */
        public Collider[] JudgeCollides(CatsBoundingBox boundingBox, Vector2 XYOffset, float ZOffset = 0.0f, Collider iCollider = null) {

            if (contentList == null) {
                return null;
            }
            List<Collider> feedback = new List<Collider>();
            foreach (Collider collider in contentList) {
                if (collider == iCollider) {
                    continue;
                }
                bool result = collider.JudgeCollide(boundingBox, XYOffset, ZOffset);
                if (result == true) {
                    feedback.Add(collider);
                }
            }
            return feedback.ToArray();
        }


        /**
         * @brief add collider to remove list. the collider will be removed async
         * 
         * @param item the Collider to be removed
         * */
        public override void RemoveItem(Collider item) {
            if (m_removeList == null) {
                m_removeList = new List<Collider>();
            }
            m_removeList.Add(item);
        }


        /**
         * @brief remove Colliders in removelist async, invoked by engine
         * */
        public void UpdateRemove() {
            if (m_removeList != null) {
                foreach (Collider collider in m_removeList) {
                    contentList.Remove(collider);
                }
                m_removeList.Clear();
            }
        }
    }
}
