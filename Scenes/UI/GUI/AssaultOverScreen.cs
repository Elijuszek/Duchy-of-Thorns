namespace DuchyOfThorns;

/// <summary>
/// Class for assault over screen functionality
/// </summary>
public partial class AssaultOverScreen : CanvasLayer
{
    [Signal] public delegate void ContinueEventHandler();
    private Label title;
    private Label lootLabel;
    private Label rewardLabel;
    private Label earnedLabel;
    private Label totalLabel;

    public override void _Ready()
    {
        title = GetNode<Label>("PanelContainer/MarginContainer/Rows/Top/Title");
        lootLabel = GetNode<Label>("PanelContainer/MarginContainer/Rows/HBoxContainer/Right/Loot");
        rewardLabel = GetNode<Label>("PanelContainer/MarginContainer/Rows/HBoxContainer/Right/Reward");
        earnedLabel = GetNode<Label>("PanelContainer/MarginContainer/Rows/HBoxContainer/Right/Earned");
        totalLabel = GetNode<Label>("PanelContainer/MarginContainer/Rows/HBoxContainer/Right/Total");
    }
    public void Initialize(bool victory, int lootGold, int rewardGold, int safeGold)
    {
        int earnedGold = lootGold + rewardGold;
        GetTree().Paused = true;
        if (victory)
        {
            title.Text = "Victory!";
            title.Modulate = Color.Color8(6, 125, 26);
        }
        else
        {
            title.Text = "Defeat!";
            title.Modulate = Color.Color8(158, 9, 22);
        }
        Tween goldTween = CreateTween();
        goldTween.SetTrans(Tween.TransitionType.Linear);
        goldTween.SetEase(Tween.EaseType.Out);
        goldTween.SetParallel(true);
        goldTween.TweenMethod(new Callable(this, "ChangeLootText"), 0, lootGold, 2f);
        goldTween.TweenMethod(new Callable(this, "ChangeRewardText"), 0, rewardGold, 2f);
        goldTween.TweenMethod(new Callable(this, "ChangeEarnedText"), 0, earnedGold, 2f);
        goldTween.TweenMethod(new Callable(this, "ChangeTotalText"), 0, safeGold, 2f);

    }
    private void ChangeLootText(int value)
    {
        lootLabel.Text = string.Format("Loot: {0}", value);
        //if (value >= 100000)
        //{
        //    currentGold.Text = string.Format("{0}k", value / 1000);
        //}
        //else
        //{
        //    currentGold.Text = value.ToString();
        //}
    }
    private void ChangeRewardText(int value)
    {
        rewardLabel.Text = string.Format("Reward: {0}", value);
    }
    private void ChangeEarnedText(int value)
    {
        earnedLabel.Text = string.Format("Earned: {0}", value);
    }
    private void ChangeTotalText(int value)
    {
        totalLabel.Text = string.Format("Total: {0}", value);
    }
    private void ContinueButtonPressed()
    {
        GetTree().Paused = false;
        EmitSignal("Continue");
        QueueFree();
    }
}
