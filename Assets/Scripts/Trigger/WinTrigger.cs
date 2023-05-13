using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour {

    [SerializeField] private PlayerController player;
    [SerializeField] private CrabController crab;

    [SerializeField] private int currentLevelIndex;
    [SerializeField] private bool isEndOfGame = false;

    public void Win() {
        player.Pause();
        player.enabled = false;
        player.character.interact.PlaceCrab();
        player.character.interact.DropItem(true);
        if (isEndOfGame) {
            UIManager.main.SetCongrat(true);
        } else {
            var openedLevels = PlayerPrefs.GetInt("OpenedLevels", 0);
            var nextLevel = currentLevelIndex + 1;
            if (openedLevels < nextLevel) {
                PlayerPrefs.SetInt("OpenedLevels", nextLevel);
            }
            GameManager.ChangeScene("Level" + nextLevel);
        }
        gameObject.SetActive(false);
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
        }
    }

}
