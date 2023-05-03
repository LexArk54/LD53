using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour {

    private PlayerController player;
    private CrabController crab;

    [SerializeField] private bool isEndOfGame = false;

    private void Awake() {
        player = null;
        crab = null;
    }

    public void Win() {
        player.Pause();
        player.enabled = false;
        player.character.interact.DropCrab();
        player.character.interact.DropAllItems();
        if (isEndOfGame) {
            UIManager.main.SetCongrat(true);
        } else {
            UIManager.main.ScreenFade(() => {
                PlayerPrefs.SetInt("OpenedLevels", 2);
                var loader = SceneManager.LoadSceneAsync("Level2");
                loader.completed += (AsyncOperation obj) => {
                    GameManager.main.RebuildManagers();
                };
            });
        }
    }



    private void OnTriggerEnter(Collider other) {
        if (other.tag == Tag.Interactive) {
            var actor = other.GetComponentInParent<Interactive>();
            if (actor.isCrab) {
                crab = actor.GetComponent<CrabController>();
            }
            if (actor.isPlayer) {
                player = actor.GetComponent<PlayerController>();
            }
            if (player && crab) {
                Win();
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.tag == Tag.Interactive) {
            var actor = other.GetComponentInParent<Interactive>();
            if (actor.isCrab) {
                crab = null;
            }
            if (actor.isPlayer) {
                player = null;
            }
            if (player && crab) {
                Win();
            }
        }
    }

}
