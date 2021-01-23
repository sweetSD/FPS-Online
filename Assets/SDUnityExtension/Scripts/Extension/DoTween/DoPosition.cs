using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    namespace Tweening
    {
        public class DoPosition : DoBase
        {
            public override Tween GetTween()
            {
                return transform.DOMove(_destValue, _duration).SetEase(_ease);
            }

            public override Tween GetReversedTween()
            {
                return transform.DOMove(_originValue, _duration).SetEase(_ease);
            }

            public override void ResetToStart()
            {
                transform.position = _originValue;
            }

            public override void ResetToEnd()
            {
                transform.position = _destValue;
            }
        }

    }
}