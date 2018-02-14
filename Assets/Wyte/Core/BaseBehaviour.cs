using UnityEngine;

/// <summary>
/// リソース読み込みなど、ゲーム起動時に利用可能になるまで時間がかかる処理を行うオブジェクトはこれを実装します。
/// </summary>
public interface ILoadable
{
	/// <summary>
	/// 必要なリソースの読み込みが完了したかどうかを取得します。
	/// </summary>
	/// <value><c>true</c> if ready; otherwise, <c>false</c>.</value>
	bool Ready { get; }
}

/// <summary>
/// Wyte Engine によるカスタムされた MonoBehaviour。
/// </summary>
public abstract class BaseBehaviour : MonoBehaviour
{
	protected Keys KeyBind => KeyBinding.Instance.Binding;
	protected SfxManager Sfx => SfxManager.Instance;
	protected MusicManager Bgm => MusicManager.Instance;
	protected FlagManager Flag => FlagManager.Instance;
	protected GameMaster Wyte => GameMaster.Instance;
	protected EventController Novel => EventController.Instance;
	protected CoroutineHelper Coroutine => CoroutineHelper.Instance;
	protected MapManager Map => MapManager.Instance;
	protected PlayerCamera Camera => PlayerCamera.Instance;
	protected WyteEvent Event => WyteEvent.Instance;
	protected FPSCounter Debugger => FPSCounter.Instance;
	protected AnimationManager AnimMan => AnimationManager.Instance;

	/// <summary>
	/// タッチパネルをサポートしているかどうか。
	/// </summary>
	protected bool IsSmartDevice => EnvironmentFlag.IsSmartDevice;
}
