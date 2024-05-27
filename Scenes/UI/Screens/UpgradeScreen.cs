using Godot;
using System;
namespace DuchyOfThorns;


public partial class UpgradeScreen : Control
{

    [Export] private Button MeleeDamageButton;
    [Export] private Button RangedDamageButton;
    [Export] private Button ArmorButton;
    [Export] private Button ArrowButton;
    [Export] private Button PotionButton;

    public Player Player { get; set; }


	public void UpdateButtons()
	{
		MeleeDamageButton.Disabled = Player.Stats.Gold < 50;
		RangedDamageButton.Disabled = Player.Stats.Gold < 70;
        ArmorButton.Disabled = Player.Stats.Gold < 35;
		PotionButton.Disabled = Player.Stats.Gold < 30;
    }

	public void CloseButtonPressed()
	{
		this.Visible = false;
	}

	public void UpgradeMeleeDamageButton()
	{
		//Player.Stats.DamageMultiplier += 0.1f;
		Player.WeaponsManager.AdjustMultiplier(5, 0f);
        Player.GetGold(-50);
		UpdateButtons();
    }

	public void UpgradeBowDamageButton()
	{
        Player.WeaponsManager.AdjustMultiplier(0f, 5);
        Player.GetGold(-70);
		UpdateButtons();
    }

	public void UpgradeArmorButton()
	{
		Player.Stats.Armour += 0.5f;
        Player.GetGold(-35);
		UpdateButtons();
    }

	public void AddArrowsButton()
	{

        Player.GetGold(-20);
		UpdateButtons();
    }

	public void RefillPotionButton()
	{
		GetNode<HealingPotion>("/root/World/GUI/MarginContainer/Rows/MiddleRow/VBoxContainer/MarginContainer/Button").pressCount = 0;
		GetNode<HealingPotion>("/root/World/GUI/MarginContainer/Rows/MiddleRow/VBoxContainer/MarginContainer/Button").UpdatePotion();
        Player.GetGold(-30);
		UpdateButtons();
    }

	


}
