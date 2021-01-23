using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DG
{
    namespace Tweening
    {
        public abstract class DoBase : MonoBehaviour
        {
            [SerializeField] protected Vector3 _originValue;
            [SerializeField] protected Vector3 _destValue;
            [SerializeField] protected bool _playOrigin = true;
            [SerializeField] protected bool _playOnEnable = true;
            [SerializeField] protected float _startDelay = 0f;
            [SerializeField] protected float _startReverseDelay = 0f;
            [SerializeField] protected float _duration = 1f;
            [SerializeField] protected int _loopCount = 0;
            [SerializeField] protected LoopType _loopType = LoopType.Restart;
            [SerializeField] protected Ease _ease = Ease.Linear;
            [SerializeField] protected UnityEvent _onComplete;
            protected Sequence _sequence = null;

            public Tween Tween => _sequence;
            public UnityEvent onComplete => _onComplete;

            private void OnEnable()
            {
                if (_playOnEnable) DOPlay();
            }

            public virtual void DOPlay()
            {
                InitializeTween();
                if (_playOrigin) ResetToStart();
                _sequence.SetDelay(_startDelay);
                _sequence.SetLoops(_loopCount, _loopType);
                _sequence.OnComplete(() => _onComplete?.Invoke());
                _sequence.Play();
            }

            public virtual void DoPlayReverse()
            {
                InitializeReversedTween();
                if (_playOrigin) ResetToEnd();
                _sequence.SetDelay(_startReverseDelay);
                _sequence.SetLoops(_loopCount, _loopType);
                _sequence.OnComplete(() => _onComplete?.Invoke());
                _sequence.Play();
            }

            public virtual void DOPause()
            {
                if (_sequence != null) _sequence.Pause();
            }

            public virtual void DOResume()
            {
                if (_sequence != null) _sequence.Play();
            }

            public virtual void DOStop()
            {
                if (_sequence != null) _sequence.Kill();
                _sequence = null;
            }

            public void InitializeTween()
            {
                if (_sequence != null) _sequence.Kill();
                _sequence = DOTween.Sequence();
                _sequence.Append(GetTween());
            }

            public void InitializeReversedTween()
            {
                if (_sequence != null) _sequence.Kill();
                _sequence = DOTween.Sequence();
                _sequence.Append(GetReversedTween());
            }

            public abstract Tween GetTween();

            public abstract Tween GetReversedTween();

            public abstract void ResetToStart();

            public abstract void ResetToEnd();
        }
    }
}
