using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;

namespace Gameplay.UI
{
    public class InGameHUD: MonoBehaviour
    {
        [SerializeField] Button _restartButton;
        [SerializeField] TextMeshProUGUI _timeText;

        [SerializeField] LocalizeStringEvent _levelStringEvent;

        private void Start()
        {
            Variable<int> levelVariable = new()
            {
                Value = 2
            };
            _levelStringEvent.StringReference["Level"] = levelVariable;
        }
    }
}
