using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Edia.Survey {

    public class LikertFaceToggle : MonoBehaviour {
        public List<Sprite> SpritesFaces;

        private void Start() {
            foreach (Image img in GetComponentsInChildren<Image>()) 
                img.sprite = SpritesFaces[int.Parse(this.name)];
        }
    }
}