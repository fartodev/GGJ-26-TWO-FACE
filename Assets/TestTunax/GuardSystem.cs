using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Can;

namespace TestTunax
{
    public class GuardSystem : MonoBehaviour
    {
        public Transform[] movePoints;

        [Tooltip("Fallback character reference - will use PossessionManager if available")]
        public Transform character;

        public float minDistance = 3;
        public float characterSpeed = 0.25f;
        public float characterToPlayerSpeed = 0.25f;
        private int currentIndex = 0;
        private bool moveToCharacter = false;

        private Tween _moveTween;

        /// <summary>
        /// Gets the current target - either from PossessionManager (possessed body) or fallback character
        /// </summary>
        private Transform CurrentTarget
        {
            get
            {
                // If PossessionManager exists, track the currently possessed object
                if (PossessionManager.Instance != null && PossessionManager.Instance.CurrentPossessed != null)
                {
                    var possessed = PossessionManager.Instance.CurrentPossessed as MonoBehaviour;
                    if (possessed != null && possessed.gameObject.activeInHierarchy)
                    {
                        return possessed.transform;
                    }
                }
                // Fallback to direct reference
                return character;
            }
        }

        private void Awake()
        {
            MoveStart();
        }

        [Button]
        public void MoveStart()
        {
            CheckMovePoint();
        }

        private void CheckMovePoint()
        {
            _moveTween?.Kill();
            _moveTween = transform.DOMove(movePoints[currentIndex % movePoints.Length].position, characterSpeed)
                .SetSpeedBased(true).OnComplete(CheckMovePoint);
            currentIndex++;
        }

        [Button]
        public void StopMove()
        {
            _moveTween?.Kill();
        }

        private void Update()
        {
            CheckTargetCharacter();
        }

        private void CheckTargetCharacter()
        {
            Transform target = CurrentTarget;
            if (target == null) return;

            float distance = Vector3.Distance(target.position, transform.position);
            if (distance < minDistance)
            {
                if (moveToCharacter)
                {
                    transform.position = Vector3.MoveTowards(transform.position, target.position, characterToPlayerSpeed * Time.deltaTime);
                }
                else
                {
                    moveToCharacter = true;
                    StopMove();
                }
            }
            else
            {
                if (moveToCharacter)
                {
                    moveToCharacter = false;
                    MoveStart();
                }
            }
        }
    }
}