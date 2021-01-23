using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    namespace Tweening
    {
        public class DoColor : DoBase
        {
            [SerializeField] protected SpriteRenderer _renderer;
            [SerializeField] protected Color _originColor = Color.white;
            [SerializeField] protected Color _destColor = Color.white;

            private void Awake()
            {
                if (_renderer == null) _renderer = GetComponent<SpriteRenderer>();
            }

            public override Tween GetTween()
            {
                return _renderer.DOColor(_destColor, _duration).SetEase(_ease);
            }

            public override Tween GetReversedTween()
            {
                return _renderer.DOColor(_originColor, _duration).SetEase(_ease);
            }

            public override void ResetToStart()
            {
                _renderer.color = _originColor;
            }

            public override void ResetToEnd()
            {
                _renderer.color = _destColor;
            }
        }
    }
}