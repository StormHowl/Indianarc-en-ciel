using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateScore : MonoBehaviour {

	public Text text;
    public Text weapon;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    text.text = "Score : " + PlayerControl.score + " Life : " + PlayerControl.life;	
        weapon.text = "Weapon :  " + GetWeapon();
	    weapon.color = PlayerControl._scoreColor;
        text.color = PlayerControl._scoreColor;
	}

    string GetWeapon()
    {
        if (PlayerControl._scoreColor == Color.red)
        {
            return "Bazooka - Left click mouse";
        } else if (PlayerControl._scoreColor == Color.magenta)
        {
            return "No usable weapon ! Just avoid touching enemies";
        }
        if (PlayerControl._scoreColor == Color.yellow)
            return "Lasso, small ranged - Right click mouse";

        if (PlayerControl._scoreColor == Color.white)
            return "No usable weapon, you need to jump on ennemies !";

        if (PlayerControl._scoreColor == Color.green)
            return "Place your mouse on an enemy and press 'S'";

        return "Nothing ?";
    }
}
