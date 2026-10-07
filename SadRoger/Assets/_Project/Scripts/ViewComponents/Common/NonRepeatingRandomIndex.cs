using UnityEngine;

namespace ViewComponents.Common
{
    internal sealed class NonRepeatingRandomIndex
    {
        private const int SingleOption = 1;

        private int _previousIndex;
        private bool _hasPrevious;

        internal int NextIndex(int count)
        {
            int index = RandomIndex(count);

            _previousIndex = index;
            _hasPrevious = true;

            return index;
        }

        private int RandomIndex(int count)
        {
            bool canAvoidRepeat = _hasPrevious && count > SingleOption;

            if (!canAvoidRepeat)
            {
                return Random.Range(0, count);
            }

            int index = Random.Range(0, count - 1);

            return index >= _previousIndex
                ? index + 1
                : index;
        }
    }
}
