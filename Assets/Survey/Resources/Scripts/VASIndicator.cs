using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Edia.Survey {
	public class VASIndicator : MonoBehaviour {
		
		[Header("References")]
		public Image Bg = null;
		public Image Line = null;
		public TextMeshProUGUI myText = null;
		
		[Header("Settings")]
		public Color ColorBand01 = new ();
		public Color ColorBand02 = new();
		
		public void SetIndicator(Constants.IndicatorTypes type, int index, string numberValue = null) {
			switch (type) {
				case Constants.IndicatorTypes.None:
					// Nothing
					break;
				case Constants.IndicatorTypes.Bands:
					Bg.gameObject.SetActive(true);
					Bg.color = index%2 == 0 ? ColorBand01: ColorBand02;
					break;
				case Constants.IndicatorTypes.Numbers:
					myText.text = numberValue;
					myText.gameObject.SetActive(true);
					break;
				case Constants.IndicatorTypes.Lines:
					Line.gameObject.SetActive(true);
					break;
			}
		}
	}
}