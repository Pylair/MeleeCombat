/*
 * Created by SharpDevelop.
 * User: eric
 * Date: 8/9/2017
 * Time: 4:27 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Assets.Resources.MeleeCombat
{
	/// <summary>
	/// Description of obsolete_movement.
	/// </summary>
	public class obsolete_movement
	{
		public obsolete_movement()
		{
		}
	}
}

/*
 public void friction () {
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
		
		public bool acceptingMovementInput = true;
		
		public virtual void setMovementInput (bool value) {
			acceptingMovementInput = value;
		}
		
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
			}
			
			handleMovement(Input.GetKey(KeyCode.LeftShift) || blocking,
			  		 attacking,
					Input.GetKeyDown(KeyCode.CapsLock),
					Input.GetKeyDown(KeyCode.Space));
			
			adjustVelocity();
		
		}
		
		void capSpeed () {
			u = Math.Min(runSpeed,u);
			u = Math.Max(u,-runSpeed);
			if (dodging){
				v = Math.Max(-runSpeed,v);
				v = Math.Min(runSpeed,v);
			} else {
				v = Math.Max(-walkSpeed,v);
				v = Math.Min(walkSpeed,v);
			}
		}
		

		void decideSpeed () {
			if (dodging){
				speedMultiplier = 1.125f;
			} else if (blocking){
				speedMultiplier = .5f;
			} else if (attacking){
				speedMultiplier = .5f;
			} else if (u < 0){
				speedMultiplier = .75f;
			} else {
				speedMultiplier = 1;
			}
			
			
		}*/
