using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIMessage : MonoBehaviour {

    [SerializeField] private Text label;
    private void Awake() {
        Show("");
    }

    public void Show(string text) {
        if (gameObject == null) return;
        if (text == "") {
            gameObject.SetActive(false);
        } else {
            gameObject.SetActive(true);
            label.text = text;
        }
    }

}