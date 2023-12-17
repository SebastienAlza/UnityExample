namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using GameCreator.Variables;

    [AddComponentMenu("")]
    public class IgniterCollision2DEnter : Igniter 
	{
        public Collider2D otherCollider2D;

#if UNITY_EDITOR
        public new static string NAME = "Object/On Collision 2D Enter";
        public new static string COMMENT = "Leave empty to collide with any object";
		public new static bool REQUIRES_COLLIDER = true;
		#endif

        [Space][VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty storeSelf = new VariableProperty(Variable.VarType.GlobalVariable);

        [Space][VariableFilter(Variable.DataType.GameObject)]
        public VariableProperty storeCollider = new VariableProperty(Variable.VarType.GlobalVariable);

        private void OnCollisionEnter2D(Collision2D c)
		{
            if (this.otherCollider2D == null)
            {
                this.storeSelf.Set(gameObject);
                this.storeCollider.Set(c.gameObject);

                this.ExecuteTrigger(c.gameObject);
            }
			else if (this.otherCollider2D.gameObject.GetInstanceID() == c.gameObject.GetInstanceID())
			{
                this.storeSelf.Set(gameObject);
                this.storeCollider.Set(c.gameObject);

                this.ExecuteTrigger(c.gameObject);
			}
		}
	}
}