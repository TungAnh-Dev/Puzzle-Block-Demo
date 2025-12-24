using System;
using UnityEngine;
using UnityEngine.UI;

public class UIReplay : UICanvas
{
    public Button replayButton;
    
    public event Action OnReplayButtonClicked; 

    public override void Close()
    {
        replayButton.onClick.RemoveListener(OnReplayButtonClick);
        base.Close();
    }

    public override void Open()
    {
        OnReplayButtonClicked = null;
        replayButton.onClick.AddListener(OnReplayButtonClick);
        base.Open();
    }

    public void OnReplayButtonClick()
    {
        OnReplayButtonClicked?.Invoke();
        this.Close();
    }
}
