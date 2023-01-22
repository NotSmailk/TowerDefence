using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginWindow : MonoBehaviour
{
    [field: SerializeField] private TMP_InputField _nameField;
    [field: SerializeField] private Button _facebookLogin;
    [field: SerializeField] private Button _simpleLogin;

    private TaskCompletionSource<UserInfoContainer> _loginCompletionSource;

    private const int NAME_MIN_LENGTH = 3;

    private void Awake()
    {
        _simpleLogin.onClick.AddListener(OnSimpleLoginClicked);
        _facebookLogin.onClick.AddListener(OnFacebookLoginClicked);
    }

    public async Task<UserInfoContainer> ProccesLogin()
    {
        _loginCompletionSource = new TaskCompletionSource<UserInfoContainer>();
        return await _loginCompletionSource.Task;
    }

    private void OnSimpleLoginClicked()
    {
        if (_nameField.text.Length < NAME_MIN_LENGTH)
            return;

        _loginCompletionSource.SetResult(new UserInfoContainer 
        { 
            Name = _nameField.text 
        });
    }

    private void OnFacebookLoginClicked()
    {
        // NONE
    }
}
