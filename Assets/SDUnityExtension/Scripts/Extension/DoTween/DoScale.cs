namespace DG
{
    namespace Tweening
    {
        public class DoScale : DoBase
        {
            public override Tween GetTween()
            {
                return transform.DOScale(_destValue, _duration).SetEase(_ease);
            }

            public override Tween GetReversedTween()
            {
                return transform.DOScale(_originValue, _duration).SetEase(_ease);
            }

            public override void ResetToStart()
            {
                transform.localScale = _originValue;
            }

            public override void ResetToEnd()
            {
                transform.localScale = _destValue;
            }
        }
    }
}