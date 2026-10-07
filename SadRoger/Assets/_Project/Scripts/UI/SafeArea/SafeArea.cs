using UnityEngine;

namespace UI.SafeArea
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    internal sealed class SafeArea : MonoBehaviour
    {
        [SerializeField] private bool _updateInEditor;
        [SerializeField] private bool _ignoreIosBottom;

        private RectTransform _rectTransform;
        private Rect _lastAppliedSafeArea;
        private Vector2Int _lastScreenSize;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            ApplySafeArea();
        }

        private void LateUpdate()
        {
            if (!Application.isPlaying
             && !_updateInEditor)
            {
                return;
            }

            if (SafeAreaOrResolutionChanged())
            {
                ApplySafeArea();
            }
        }

        private bool SafeAreaOrResolutionChanged()
        {
            Rect safeArea = Screen.safeArea;
            var screenSize = new Vector2Int(Screen.width, Screen.height);
            return _lastAppliedSafeArea != safeArea || _lastScreenSize != screenSize;
        }

        private void ApplySafeArea()
        {
            if (_rectTransform == null)
            {
                throw new SafeAreaRectMissingRectTransformException();
            }

            Rect safeArea = Screen.safeArea;
            _lastAppliedSafeArea = safeArea;
            _lastScreenSize = new Vector2Int(Screen.width, Screen.height);

            float width = _lastScreenSize.x;
            float height = _lastScreenSize.y;

            if (width <= 0f
             || height <= 0f)
            {
                return;
            }

            var anchorMin = new Vector2(safeArea.xMin / width, safeArea.yMin / height);
            var anchorMax = new Vector2(safeArea.xMax / width, safeArea.yMax / height);

            if (_ignoreIosBottom && Application.platform == RuntimePlatform.IPhonePlayer)
            {
                anchorMin.y = 0f;
                anchorMax.y = 1f;
            }

            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
        }
    }
}
