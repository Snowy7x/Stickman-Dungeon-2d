using System;
using UnityEngine;

namespace Map
{
    [Serializable]
    public abstract class RoomChallenge : MonoBehaviour
    {
        [SerializeField] protected string challengeName;
        [SerializeField] protected string challengeDescription;
        [SerializeField] protected int baseReward;
        [SerializeField] protected int challengeTime;
        [SerializeField] protected int maxScore;
        [SerializeField] protected int currentScore;

        protected Room room;
        protected bool isComplete = false;
        protected int challengeDifficulty = 0;
        protected int challengeReward = 0;

        public virtual void Initialize(Room nRoom, int nDifficulty)
        {
            challengeDifficulty = nDifficulty;
            challengeReward = baseReward * (challengeDifficulty + 1);
            
            room = nRoom;
            isComplete = false;
        }

        public virtual void OnPlayerEnter()
        {
            Debug.Log("Player entered room with challenge: " + challengeName);
            PopupWindow.Instance.TitleAddToQueue(challengeName);
            PopupWindow.Instance.AddToQueue(challengeDescription);
        }

        public virtual void OnUpdate()
        {
            if (currentScore >= maxScore)
            {
                OnEnd();
            }
        }

        public virtual void OnEnd()
        {
            isComplete = true;
        }
        public abstract void OnPlayerExit();
        
        public void AddScore(int score)
        {
            currentScore += score;
            if (currentScore > maxScore) currentScore = maxScore;
        }
        
        public void RemoveScore(int score)
        {
            currentScore -= score;
            if (currentScore < 0) currentScore = 0;
        }
    }
}