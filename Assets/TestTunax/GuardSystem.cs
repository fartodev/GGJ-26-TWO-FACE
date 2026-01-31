using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TestTunax
{
    public class GuardSystem : MonoBehaviour
    {
        public Transform[] movePoints;
        public Transform character;

        public float minDistance = 3;
        public float characterSpeed = 0.25f;
        public float characterToPlayerSpeed = 0.25f;
        private int currentIndex = 0;
        private bool moveToCharacter = false;

        private Tween _moveTween;

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
            float distance = Vector3.Distance(character.position, transform.position);
            if (distance<minDistance)
            {
                if (moveToCharacter)
                {
                    transform.position = Vector3.MoveTowards(transform.position, character.position, characterToPlayerSpeed*Time.deltaTime);
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