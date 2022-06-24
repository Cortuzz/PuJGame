using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UIInteractions.Settings
{
    public class GraphicsSettings : MonoBehaviour
    {
        public TMP_Dropdown resolutionDropdown;

        private Resolution[] _resolutions;

        private void Start()
        {
            _resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();

            var options = new List<string>();
            var currentResIndex = 0;
            
            for (var i = 0; i < _resolutions.Length; ++i)
            {
                var option = _resolutions[i].width + " x " + _resolutions[i].height;
                options.Add(option);

                if (_resolutions[i].width != Screen.currentResolution.width ||
                    _resolutions[i].height != Screen.currentResolution.height)
                {
                    continue;
                }

                currentResIndex = i;
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResIndex;
            resolutionDropdown.RefreshShownValue();
        }

        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
            Debug.Log(QualitySettings.GetQualityLevel());
        }

        public void SetDisplayMode(int displayMode)
        {
            Screen.fullScreenMode = (FullScreenMode)displayMode;
        }

        public void SetResolution(int resolutionIndex)
        {
            Screen.SetResolution(_resolutions[resolutionIndex].width, _resolutions[resolutionIndex].height,
                Screen.fullScreenMode);
        }
    }
}