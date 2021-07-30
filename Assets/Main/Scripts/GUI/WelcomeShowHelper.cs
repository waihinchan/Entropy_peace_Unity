using System;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeShowHelper : MonoBehaviour
{
    private UserManager _userManager;
    private Text _moneyText;
    private void Start()
    {
        _userManager = GameObject.Find("Manager").GetComponent<UserManager>();
        _moneyText = GameObject.Find("Canvas/持有金币数/金币数").GetComponent<Text>();
    }

    private void Update()
    {
        _moneyText.text = _userManager.LocalUserInfo.GameMoney.ToString();
    }
}