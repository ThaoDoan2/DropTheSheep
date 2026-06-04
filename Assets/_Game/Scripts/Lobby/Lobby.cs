using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    [SerializeField] Button _playButton;


    private void Start()
    {
        _playButton.onClick.AddListener(OnPlayButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("GameScene");
    }
}
