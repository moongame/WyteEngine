using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

/// <summary>
/// プレイヤーキャラの制御を行います．
/// </summary>
public class PlayerController : LivableEntity
{

	[Header("ANIMATION_NAME")]
	[SerializeField]
	string jumpAnimationId = "entity.player.jump";
	[SerializeField]
	string stayAnimationId = "entity.player.stay";
	[SerializeField]
	string walkAnimationId = "entity.player.walk";
	[SerializeField]
	protected float charaMoveSpeed = 5.0f;
	[SerializeField]
	protected float charaDashMultiplier = 2.0f;

	Gauge hpGauge;
	Text hpNumeric;

	public override string JumpAnimationId => jumpAnimationId;
	public override string StayAnimationId => stayAnimationId;
	public override string WalkAnimationId => walkAnimationId;

	public override string LandSfxId => "entity.player.land";
	public override string JumpSfxId => "entity.player.jump";
	public override string DeathSfxId => "entity.player.death";

	private int maxHealth;

	public override int MaxHealth => maxHealth;

	/// <summary>
	/// 開始処理
	/// </summary>
	protected override void Start()
	{
		maxHealth = Wyte.Player.MaxLife;
		hpGauge = GameObject.FindGameObjectWithTag("HpGauge").GetComponent<Gauge>();
		hpNumeric = GameObject.FindGameObjectWithTag("HpNumeric").GetComponent<Text>();
		base.Start();
		Debugger.DebugRendering += Debugger_DebugRendering;
	}

	protected void UpdateUI()
	{
		if (hpGauge != null)
			hpGauge.Progress = HealthRatio;
		
		if (hpNumeric != null)
			hpNumeric.text = $"{ToFullWidthString(Health, 3)}／{ToFullWidthString(MaxHealth, 3)}";
	}

	private string ToFullWidthString(int value, int padding = 0)
		=> string.Concat(value.ToString().Select(c => char.IsDigit(c) ? (char)(c - '0' + '０') : c)).PadLeft(padding);

	private void OnDestroy()
	{
		Debugger.DebugRendering -= Debugger_DebugRendering;
	}

	void Debugger_DebugRendering(System.Text.StringBuilder d)
	{

		d.AppendLine($"pp{(int)transform.position.x},{(int)transform.position.y}")
		 .AppendLine($"pv{(int)rigid.velocity.x},{(int)rigid.velocity.y} ")
		 .AppendLine($"p{(Dying ? "DEAD" : "ALIVE")} ");
	}

	protected override void OnFixedUpdate()
	{
		// 移動可能時に処理を行う
		if (Wyte.CanMove)
			InputKey();
		
		UpdateUI();
		base.OnFixedUpdate();
	}

	bool GetJumpKeyPushed(bool down = false) =>
		IsSmartDevice
		// Android iOS など
		? GamePadBehaviour.Instance.Get(GamePadButtons.Action, down)
		// PC
		: (down ? Input.GetKeyDown(KeyBind.Jump) : Input.GetKey(KeyBind.Jump));
	
	/// <summary>
	/// キー入力
	/// </summary>
	void InputKey()
	{
		IsJumping &= (!IsGrounded() || (int)Velocity.y != 0);
		if (GetJumpKeyPushed(true))
		{
			Jump();
		}

		AnimationMultiplier = DashMultiplier;

		Move(KeyBind.Arrow.x * charaMoveSpeed * DashMultiplier, !GetJumpKeyPushed());
	}

	public float DashMultiplier =>
		Input.GetKey(KeyBind.Dash) ? charaDashMultiplier 
		: IsSmartDevice ? charaDashMultiplier
		: 1;

	/// <summary>
	/// 移動
	/// </summary>
	public override void Move(float rightSpeed, bool hold = true)
	{
		base.Move(rightSpeed, hold);

		// 左端処理
		if (transform.position.x < Map.CurrentMapSize.xMin)
		{
			transform.position = new Vector3(Map.CurrentMapSize.xMin, transform.position.y, transform.position.z);
			Velocity = new Vector2(Velocity.x < 0 ? 0 : Velocity.x, Velocity.y);
		}

		// 右端処理
		if (transform.position.x > Map.CurrentMapSize.xMax)
		{
			transform.position = new Vector3(Map.CurrentMapSize.xMax, transform.position.y, transform.position.z);
			Velocity = new Vector2(Velocity.x > 0 ? 0 : Velocity.x, Velocity.y);
		}
	}

	protected override IEnumerator OnDamaged(Object interacter)
	{
		return base.OnDamaged(interacter);
	}

	protected override IEnumerator OnDeath(Object killer)
	{
		StartCoroutine(Bgm.Stop(0.5f));
		Sfx.Play(DeathSfxId);
		ChangeSprite("entity.player.drown");
		var targetY = transform.position.y + 300;
		
		UpdateUI();		

		for (int y = (int)transform.position.y; y < targetY; y += 4)
		{
			transform.Rotate(Vector3.forward * 180 * Time.deltaTime);
			transform.position = new Vector3(transform.position.x, y, transform.position.z);
			
			yield return null;
		}
		yield return new WaitForSeconds(3);
		Wyte.Initalize();
	}
}
