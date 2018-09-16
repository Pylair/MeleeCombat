using UnityEngine;
using System;
using Random = System.Random;
using System.Collections.Generic;
using MeleeCombat.InventoryClasses;
using MeleeCombat.Tracers;
using MeleeCombat.AI;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;


namespace MeleeCombat
{

	public enum AttackDirection : int{BLOCK  = -2,LTR = 1, RTL = 0,UPWARD = 3,DOWN = 2, THRUST = 4,NODIRECTION = -1, KICK = 5};
	public enum AttackClass : int{MELEE = 0, RANGED = 1, COMBO = 2};
	public enum WeightClass : int{ZERO = 0, LIGHT = 1, MEDIUM = 2, HEAVY = 3};
	public enum Grip : int{MAINHAND = 0, TWOHANDED = 1,UNEQUIPPED = -1};
	public enum Bodypart : int{LEFTHAND = 0, RIGHTHAND = 1,HEAD = 2,SPINE = 3,RIGHTFOOT = 4, LEFTFOOT = 5};
	
	
	public delegate void EnableWeaponEventHandler (Weapon weapon);
	public delegate void WeaponSwingEventHandler (MeleeController sender);
	public delegate void BlockEventHAndler (object sender,EventArgs e);
	public delegate void DamageAppliedEventHandler (DamageManager manager);
	
	public delegate void MeleeControllerEventHandler (MeleeController sender);

	
	public class MeleeController : MonoBehaviour
	{
		
		public static bool isAi(MeleeController controller){
			return controller.GetType().Equals(typeof(AIController));
		}

		internal bool receivingTeamInstructions = false;
		
		public TeamControl Team {get;set;}
	
		
		public static Random r = new Random();
	
		public event MeleeControllerEventHandler OnDeathEvent;
		public event EnableWeaponEventHandler EnableWeapon;
		public event DamageAppliedEventHandler DamageApplied;
		public event WeaponSwingEventHandler SwingBegan;
		public event WeaponSwingEventHandler SwingFinished;
		public event BlockEventHAndler BlockBegin;
		public event BlockEventHAndler BlockEnd;
		
		public bool isAttacking;
		
		public string Name;

		public const string rootColliderTag = "Root Collider";
		
		public void recursiveEnableColliders (GameObject g,bool b, bool affectRootCollider){
			var col = g.GetComponent<Collider>();
			if (col != null && g.GetComponent<Equipment>() == null){
				if (g.tag.Equals(rootColliderTag) && affectRootCollider){
					col.enabled = b;
				} else {
					col.enabled = b;
				}
			}
			foreach (Transform t in g.transform){
				recursiveEnableColliders(t.gameObject,b,affectRootCollider);
			}
		}
		
		public void OnDeath () {
			if (OnDeathEvent != null){
				OnDeathEvent(this);
			}
		}
		
		public void OnDamageApplied (DamageManager manager){
			if (DamageApplied != null){
				DamageApplied(manager);
			}
		}
		
		public void OnBlockBegin (EventArgs e){
			blocking = true;
			if (BlockBegin != null){
				BlockBegin(this,e);
			}
		}
		
		public void OnBlockEnd (EventArgs e){
			blocking = false;
			if (BlockEnd != null){
				BlockEnd(this,e);
			}
		}
	
		public void EnableFeint () {
	
		}
		
		public void OnWeaponSwing(){
			canFeint = false;
			if (SwingBegan != null){
				SwingBegan(this);
				isAttacking = true;
			}
		}
		
		public void OnEndWeaponSwing(){
			if (SwingFinished != null){
				SwingFinished(this);
				isAttacking = false;
			}
			hasFeinted = false;
		}
		
		public void enableNonFlinchActions () {
			animator.SetBool("flinching",false);
		}
	
		public void EnableWeaponAtBodyPart (Bodypart part) {
			if (EnableWeapon != null){
				var bodyPartManager = skeletonMap[part];
				if (bodyPartManager == null) return;
				var obj = bodyPartManager.objectOccupyingBodyPart;
				if (obj == null) return;
				var weapon = obj.GetComponentInChildren<Weapon>();
				if (weapon == null) return;
				var tracer = weapon.GetComponentInChildren<Tracer>();
				if (tracer != null){
					weaponTracer = tracer;
				}
				if (! AIController.isAi(this)) canFeint = true;
				EnableWeapon(weapon);
			}
		}
		
		public Dictionary<Bodypart,HandManager> skeletonMap;
		
		Camera cam;
		MouseLook mLook;
		internal Rigidbody rBody;
		internal Animator animator;
		public bool alive = true;
		
		public AudioSource audioSource;
		public List<AudioClip> hitClips;
		

		
		public DamageManager damageManager;
		
		public virtual void Awake()
		{
			cam = Camera.main;
			mLook = new MouseLook();
			mLook.Init(gameObject.transform,cam.transform);
			setUp();
		}
		
		internal void setUp(){
			r = new Random();
			animator = GetComponent<Animator>();
			rBody = gameObject.GetComponent<Rigidbody>();
			audioSource = gameObject.GetComponent<AudioSource>();
			skeletonMap = new Dictionary<Bodypart, HandManager>();
			damageManager = gameObject.GetComponent<DamageManager>();
			constructEquipmentMap(gameObject);
			recursiveEnableColliders(gameObject,true,false);
		}
		

		public virtual void Start(){
			SceneControl.sceneCont.addCharacter(gameObject);
		}

		internal void constructEquipmentMap (GameObject g){
			foreach (Transform childTransform in g.transform){
				var child = childTransform.gameObject;
				var handManager = child.GetComponent<HandManager>();
				if (handManager!= null){
					skeletonMap[handManager.part] = handManager;
				}
				constructEquipmentMap(child);
			}
		}
		
		const float attackTransitionDuration = .5f;
		public const int attackLayer = 2;
	
		internal Grip grip;		
		public Bodypart hand = Bodypart.RIGHTHAND;
		
		public virtual void drawEquipment (){
			if (Input.GetKeyDown(KeyCode.R)){
				var inventory = gameObject.GetComponent<Inventory>();
				var count = inventory.itemPaths.Count;
				while (inventory.itemPaths.Count > 0){
					drawEquipment(inventory.loadItem(0));
				}
			}
		}
		
		public Weapon getWeapon (Bodypart part) {
			var mainHandManager = skeletonMap[part].GetComponent<HandManager>();
			var weap = mainHandManager.getOccupyingEquipment();
			return weap.GetComponent<Weapon>();
		}

		void OnAnimatorIK () {
			var rightHand = skeletonMap[Bodypart.RIGHTHAND];
			var rightWeapon = skeletonMap[Bodypart.RIGHTHAND].objectOccupyingBodyPart;
			var leftWeapon = skeletonMap[Bodypart.LEFTHAND].objectOccupyingBodyPart;
			/*if (rightWeapon != null){
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
				animator.SetIKRotationWeight(AvatarIKGoal.RightHand,1);
				animator.SetIKPosition(AvatarIKGoal.RightHand,rightWeapon.transform.position);
				animator.SetIKRotation(AvatarIKGoal.RightHand,skeletonMap[Bodypart.RIGHTHAND].transform.rotation);
			} else {
				 animator.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
               	 animator.SetIKRotationWeight(AvatarIKGoal.RightHand,0); 
			}*/
			if (rightWeapon != null){
				animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,1);
				animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,1);
				animator.SetIKPosition(AvatarIKGoal.LeftHand,rightHand.transform.position);
				animator.SetIKRotation(AvatarIKGoal.LeftHand,rightHand.transform.rotation);
			} else {
				 animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,0); 

			}
			
			
		}
		
		public void drawEquipment (GameObject weapon){
			var mainHandManager = skeletonMap[Bodypart.RIGHTHAND];
			var offHandManager =  skeletonMap[Bodypart.LEFTHAND];
			var weaponManager = weapon.GetComponentInChildren<Equipment>();
			var tracer = weapon.GetComponentInChildren<Tracer>();
			if (tracer != null) weaponTracer = tracer;
			
			if (weaponManager.weight == WeightClass.HEAVY){
				grip = Grip.TWOHANDED;
				mainHandManager.equipTwoHander(weapon,offHandManager);
			} else {
				grip = Grip.MAINHAND;
				var array =  weaponManager.equippableSlots;
				for (int i = 0; i < array.Length;i++){
					var manager = skeletonMap[array[i]].GetComponent<HandManager>();
					if (manager.getOccupyingEquipment() == null){
						manager.equip(weapon,true);
						
						
						
						
						return;
					}
				}
			}		
		}

		const string attackBlendTreeName = "Attacks";
		
		public bool dodging {
			get{return animator.GetBool("dodging");}
		}
		public bool blocking {get{return animator.GetBool("blocking");}  set{animator.SetBool("blocking",value);}}
		
		public bool attacking {
			get{return animator.GetBool("attacking");}
		}
		
		public const string dodgeState = "Dodge";
		//public const float stackingAttackSpeed = 0f;
		//public const float defaultAttackSpeed = 1;
		public const string defaultAttackState = "Default";
		public const string attackIndex = "attackIndex";
	//	public const string attackSpeedStacks = "attackSpeedStacks";
	//	public const string attackSpeed = "attackSpeed";
		public const int maxSpeedStacks = 3;
		public const string attackInterruptedAnim = "Interrupted";
		public const string kickState = "kick";
		
		public bool flinching {get {return animator.GetBool("flinching");}}
		public bool parryRecoil {get {return animator.GetCurrentAnimatorStateInfo(attackLayer).IsName(attackInterruptedAnim);}}
		
		public AttackDirection getCurrentAttackDirection {
			get { return (AttackDirection)animator.GetInteger(attackIndex);}
		}
		
		public float attackNormalizedTime () {
			if (! attacking) return -1;
			return animator.GetCurrentAnimatorStateInfo(attackLayer).normalizedTime;
		}
		
		public Faction faction (){
			return GetComponent<Faction>();
		}
		
		public void attackInterrupted (){
			if (animator.GetCurrentAnimatorStateInfo(attackLayer).IsName("Interrupted")) return;
			animator.SetInteger(attackIndex,(int)AttackDirection.NODIRECTION);
			OnEndWeaponSwing();
			animator.SetTrigger("interrupted trigger");
		}
		
		public void playDamageAudio () {
			audioSource.clip = 	hitClips[r.Next(0,hitClips.Count)];
			if (! audioSource.isPlaying){
				audioSource.Play();
			}
		}
		
		public void playDamageAnim(){			
			if (dodging) return;
			animator.CrossFade("Damaged",.05f,attackLayer);
			
		}
		
		
		internal bool canFeint = false;
		internal bool hasFeinted = false;
		float timeSinceFeint = 0;
		const float timeForFeintReset = 2;
		public bool feinting;
		
		public bool feint(){
			if (timeSinceFeint > timeForFeintReset) hasFeinted = false;
			if (! canFeint || ! attacking || hasFeinted) return false;
			animator.SetBool("attacking",false);
			OnEndWeaponSwing();
			animator.CrossFade("Default",.15f,attackLayer);
			feinting = true;
			hasFeinted = true;
			return true;
		}

		public void resetAttackInput () {
			receivingAttackInput = true;
		}
		
		AttackDirection current;
	
		public void executeAttack (AttackDirection d,Bodypart part){
			if (dodging || flinching || blocking) {
				return;
			}
			current = getCurrentAttackDirection;
			if ( receivingAttackInput) {
				if ((d != AttackDirection.NODIRECTION)){
					
					isAttacking = true;
					receivingAttackInput = false;
					
					OnBlockEnd(new EventArgs());
					EnableWeaponAtBodyPart(part);
					animator.SetInteger(MeleeController.attackIndex,(int)d);	
					
				}
			} 
		}
		
		public bool receivingAttackInput = true;
		
		public void handleMainHandResponse () {
			var mainHandAction = Input.GetMouseButtonDown(0);
			var mouseScrollDelta = Input.mouseScrollDelta.y;
			var currentAttackDirection = getCurrentAttackDirection;
			var kicking = Input.GetKeyDown(KeyCode.E);
			
			if (Input.GetKeyDown(KeyCode.Q)){
				feint();
				return;
			}
	
			AttackDirection response = AttackDirection.NODIRECTION;
			if (kicking){
				response = AttackDirection.KICK;
				executeAttack(response,Bodypart.RIGHTFOOT);
				return;
			} else {
				if (mainHandAction){
					if (currentAttackDirection == AttackDirection.NODIRECTION || currentAttackDirection == AttackDirection.LTR){
						response = AttackDirection.RTL;
					} else {
						response = AttackDirection.LTR;
					}
				} else if (mouseScrollDelta > 0){
					response = AttackDirection.UPWARD;
				} else if (mouseScrollDelta < 0){
					response = AttackDirection.DOWN;
				} else if (Input.GetMouseButtonDown(2)){
					response = AttackDirection.THRUST;
				}
			}
			executeAttack(response,Bodypart.RIGHTHAND);
		}
		
		
		public virtual void handleOffhandResponse (){
			executeBlock(Input.GetMouseButton(1));
		}
			
		public virtual void executeBlock (bool b){
	
			if (flinching || dodging || attacking) {
				return;
			}

			if (!blocking && b && ! attacking ){
				OnBlockBegin(new EventArgs());
				return;
			} 
			
			if (!b && blocking){
				OnBlockEnd(new EventArgs());
			}
			
		}
		
		public static int weaponIdleLayer = 1;

		int getBlockEquipmentType (){
			var lH = skeletonMap[Bodypart.LEFTHAND].GetComponent<HandManager>();
			var rH = skeletonMap[Bodypart.RIGHTHAND].GetComponent<HandManager>();
			int blockEquipmentType = -1;

			if (lH.Occupied()){
				var equipment = lH.getOccupyingEquipment();
				if (equipment != null){
					var etype = equipment.equipmentType;
					if (etype == EquipmentType.SHIELD){
						blockEquipmentType = 0;
					} else if (etype == EquipmentType.MELEE){
						blockEquipmentType = 1;
					}
				}
			} else {
				blockEquipmentType = 1;
			}
			return blockEquipmentType;
		}
		
		public const int blockLayer = 3;

		void handleEquipmentIdles (){
			if (attacking){
				animator.SetLayerWeight( weaponIdleLayer,0);
			} else if (grip == Grip.UNEQUIPPED){
				animator.SetLayerWeight( weaponIdleLayer,0);
			} else {
				if (grip == Grip.TWOHANDED){
					animator.SetLayerWeight( weaponIdleLayer,1 );
				} else if (grip == Grip.MAINHAND){
					animator.SetLayerWeight( weaponIdleLayer,.5f );
				}
			}
			animator.SetInteger("grip",(int)grip);
			animator.SetFloat("gripBlend",(int)grip);
		}
		
		public bool crouching {get { return animator.GetBool("crouching");} set {animator.SetBool("crouching",value);}}
		public bool walking {get { return animator.GetBool("walking");} set {animator.SetBool("walking",value);}}
		
		public const float defaultAcceleration = .5f;
		public const float cowardsPenalty = .5f;
		public const int sideSpeed = 2;
		public const int walkSpeed = 2;
		public const int runSpeed = 4;
		public const int sprintSpeed = 6;	

		public float u;
		public float v;

		public float speedMultiplier;
		public float jumpVelocity;

		internal float acceleration = defaultAcceleration;
		internal bool acceptingMovementInput = true;
		internal bool airborne = false;
		
		
		public virtual void movementInput(){
			if (acceptingMovementInput) {
				if (! dodging){
					if (Input.GetKey(KeyCode.W)){
						u += acceleration;
					} else if (Input.GetKey(KeyCode.S)){
						u += -acceleration;
					} 
					if (Input.GetKey(KeyCode.A)){
						v += -acceleration;
					} else if (Input.GetKey(KeyCode.D)){
						v +=  acceleration;	

					}
					friction();
				}
				checkDodge();
				jump();
				crouching = Input.GetKey(KeyCode.LeftControl);
				walking = Input.GetKey(KeyCode.LeftShift);
			}
			handleMovement();
		}

		public virtual void handleBodyDirection () {
			mLook.LookRotation(gameObject.transform,cam.transform);
			var forward = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
			var up = skeletonMap[Bodypart.SPINE].transform.up;
			var angle = Vector3.Angle(forward,Vector3.up);
			animator.SetFloat("y",angle);
		}
		
		internal void friction () {
			if (Math.Abs(u) > acceleration/2f){
				u += -Math.Sign(u) * acceleration/2f;
			} else {
				u = 0;
			}
			if (Math.Abs(v) >  acceleration/2f){
				v += -Math.Sign(v) * acceleration/2f;
			} else {
				v = 0;
			}
		}

		internal void decideSpeed () {
			if (dodging){
				speedMultiplier = 1.125f;
			} else if (blocking || walking){
				speedMultiplier = .5f;
			} else if (attacking){
				speedMultiplier = .5f;
			} else if (u < 0){
				speedMultiplier = .5f;
			} else if (crouching ){
				speedMultiplier = .35f;
			} else {
				speedMultiplier = 1;
			}
			
			if (damageManager.isLowStamina){
				speedMultiplier *= .75f;
			}
			
		}
		
		internal void setMovementParameters () {
			animator.SetFloat("forward",u * speedMultiplier);
			animator.SetFloat("right",v * speedMultiplier);
			animator.SetFloat("flightStage",rBody.velocity.y);
			animator.SetBool("grounded", !airborne);
		}
		
		internal void handleMovement(){
			capSpeed();
			checkGround();
			decideSpeed();
			adjustVelocity();
			setMovementParameters();
		}
		
		internal virtual void adjustVelocity(){
			if (dodging) return;
			var forward = gameObject.transform.forward.normalized * u ;
			var right = gameObject.transform.right.normalized * v;
			var up = gameObject.transform.up;
			var velocityVector = forward + right + new Vector3(0,rBody.velocity.y,0);
			rBody.velocity = velocityVector * speedMultiplier;
		}
		
		internal virtual void capSpeed () {
			u = Math.Min(runSpeed,u);
			u = Math.Max(u,-runSpeed);
			if (dodging){
				v = Math.Max(-runSpeed,v);
				v = Math.Min(runSpeed,v);
			} else {
				v = Math.Max(-runSpeed * .75f,v);
				v = Math.Min(runSpeed * .75f,v);
			}
		}
		
		public virtual void setMovementInput (bool value) {
			acceptingMovementInput = value;
		}
		
		
		internal void checkDodge () {
			if (Input.GetKeyDown(KeyCode.F)){
				dodge(u,v);
			}
		}

		public static float dodgeStamina = .25f;
		
		public bool canDodge () { 
			return animator.GetBool((DefaultBehavior.isDefaultStateBool)) && damageManager.stamina >= dodgeStamina;
		}
		
		
		public void dodge (float u1, float v1){
			if (dodging || ! canDodge()) {
				return;
			} else {
			
				u = u1;
				v = v1;
				if (u == 0 && v == 0){
					u = -3;
				}
				var forward = transform.forward.normalized * u;
				var right = transform.right.normalized * v;
				animator.Play("Dodge",attackLayer);		
				damageManager.changeStamina(-dodgeStamina);

			}
		}
		
		internal void checkGround (){
			var up = gameObject.transform.up;
			var feet = gameObject.transform.position + up * .1f;
			airborne = ! Physics.Raycast(feet,-up,1);
		}
		
		void jump (){
			if (airborne || ! Input.GetKeyDown(KeyCode.Space)) return;
			animator.Play("Airborne",0);
			rBody.velocity += new Vector3(0,jumpVelocity,0);
		}
		
		public void Update (){
			if (alive){
				drawEquipment();
				handleEquipmentIdles();
				handleMainHandResponse();
				handleOffhandResponse();
				handleBodyDirection();
				movementInput();
				timeSinceFeint += Time.deltaTime;
			}
		}
		
		
		
		void holdWeapon () {
			if (Input.GetKey(KeyCode.C)){
				animator.SetFloat("attackSpeed",0);
			} else {
				animator.SetFloat("attackSpeed",1);
			}
		}
		
		public Weapon GetRightWeapon (){
			return (Weapon)skeletonMap[Bodypart.RIGHTHAND].GetComponent<HandManager>().getOccupyingEquipment();
			
		}
		public Weapon GetLeftWeapon (){
			return (Weapon)skeletonMap[Bodypart.LEFTHAND].GetComponent<HandManager>().getOccupyingEquipment();
			
		}
		
		public Tracer weaponTracer;
		
		public static int deathLayer = 4;
		public static string deathAnimName = "Death";
		
		public virtual void die () {
			alive = false;
			animator.Play(deathAnimName,deathLayer);
			v = 0;
			u = 0;
			rBody.constraints = RigidbodyConstraints.FreezeAll;
			recursiveEnableColliders(gameObject,false,true);
			var ob = GetComponent<NavMeshObstacle>();
			if (ob != null){
				ob.carving = false;
			}
			OnDeath();
		}
		
		public void OnStepBegin () {
			
		}

		
		public void OnStepEnd () {
			
		}
	}
}
