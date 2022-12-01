using UnityEngine;
public class TestInteractable : Interactable
{
    public override void OnFocus()
    {
        print("Looking at " + gameObject.name);
    }

    public override void OnInteract()
    {
        print("INTERACTED with " + gameObject.name);
    }

    public override void OnLoseFocus()
    {
        print("STOPPED looking at " + gameObject.name);
    }
}
