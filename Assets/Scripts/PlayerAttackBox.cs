using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackBox : MonoBehaviour
{
    [SerializeField]UnityEngine.UI.Text scoreboard;
    PlayerCamera playerCamera;
    PlayerController2 player;
    

    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.GetComponent<PlayerController2>();    
        playerCamera = GameObject.Find("Main Camera").GetComponent<PlayerCamera>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!player.IsAttacking()) return;

        Transform parentTransform = other.transform.root;

        if (parentTransform.tag == "Chicken") {
            //print("Hit a chicken.");
            ChickenBase chickenController = parentTransform.GetComponent<ChickenBase>();

            if (!chickenController) print("chickenController is null");

            chickenController.TakeDamage(transform.parent.gameObject); // player

            if (player.GetScore() < 200)
                playerCamera.IncreaseDistance();

            player.AddScore();
            scoreboard.text = "Score: " + player.GetScore();

            //if (bigC) { // use chicken spawner
            //    if (score >= 50 && !bigC.activeInHierarchy) {
            //        bigC.SetActive(true);

            //    }
            //}
        }
        //else {
        //    print("OnTrigger: " + parentTransform.name);
        //}
    }
}
