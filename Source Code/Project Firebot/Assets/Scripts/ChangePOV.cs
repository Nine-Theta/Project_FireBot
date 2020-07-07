using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePOV : MonoBehaviour
{
    public string[] commands = { "Up", "Down" };

    public void ExecuteCommand(string command)
    {
        Vector3 cameraPosition = transform.position;
        switch (command)
        {
            case "Up":
                UIConsole.Instance.SetMessage("Move up");
                transform.position = new Vector3(cameraPosition.x, cameraPosition.y + 3, cameraPosition.z);
                break;
            case "Down":
                if (cameraPosition.y != 0) {
                    UIConsole.Instance.SetMessage("Move down");
                    transform.position = new Vector3(cameraPosition.x, cameraPosition.y - 3, cameraPosition.z);
                }
                break;
        }
    }
}
