using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIMessage : MonoBehaviour {

    [SerializeField] private Text label;

    public void Show(string text) {
        if (text == string.Empty) {
            gameObject.SetActive(false);
        } else {
            gameObject.SetActive(true);
            label.text = text;
        }
    }

}