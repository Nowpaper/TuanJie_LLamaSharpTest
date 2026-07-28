using UnityEngine;
using UnityEngine.UI;

public class UITestInput : MonoBehaviour
{
    public Text dialogueText;
    public InputField inputField;
    public Button btnSubmit;
    public LocalLLMNPC npc;
    void Start()
    {
        if (!npc)
        {
            Debug.LogError("必须有个测试LLMNPC");
            return;
        }
        inputField.onEndEdit.AddListener((s) =>
        {
            submit(s);
        });
        btnSubmit.onClick.AddListener(() =>
        {
            submit(inputField.text);
        });
    }
    private async void submit(string s)
    {
        Debug.Log("Input: " + s);
        inputField.text = "";
        inputField.enabled = btnSubmit.enabled = false;
        await npc.SpeakToNPC(s, dialogueText);
        inputField.enabled = btnSubmit.enabled = true;

    }
}
