using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Reflection;
using System.Linq.Expressions;

namespace XNA_ScreenManager.PlayerClasses.StatusClasses
{
    [Serializable]
    public class StatusUpdateClass 
    {
        private float Timer;
        private int getvalue;
        private bool Permanent; 
        private string Attribute;

        public bool Remove = false;
        
        public StatusUpdateClass(float timer, string attr, int value, bool permanent)
        {
            this.Timer = timer;
            this.Permanent = permanent; // e.g. healing potions
            this.Attribute = attr;

            // Try Get/Set Player Stat Value
            try
            {
                Object player = PlayerStore.Instance.activePlayer;
                //PropertyInfo info = PropertyHelper<PlayerInfo>.GetProperty(x => x.GetType().GetProperties().);
                PropertyInfo info = typeof(PlayerInfo).GetProperty(Attribute);

                int getvalue = (int)info.GetValue(player, null);

                player.GetType().GetProperty(Attribute).SetValue(player, getvalue + value, null);
                
            }
            catch
            {
                throw new Exception("property cannot be found! Or is perhaps inaccessible!");
            }
        }

        public void Update(GameTime gameTime)
        {
            Timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Timer <= 0 || Permanent)
            {
                // Set Remove to cleanup this instance
                Remove = true;

                if (!Permanent)
                {
                    // Get Stat Value
                    Object player = PlayerStore.Instance.activePlayer;
                    getvalue = (int)player.GetType().GetProperty(Attribute).GetValue(player, null);
                    player.GetType().GetProperty(Attribute).SetValue(player, getvalue, null);
                }
            }
        }
    }

    public static class PropertyHelper<T>
    {
        public static PropertyInfo GetProperty<TValue>(
            Expression<Func<T, TValue>> selector)
        {
            Expression body = selector;
            if (body is LambdaExpression)
            {
                body = ((LambdaExpression)body).Body;
            }
            switch (body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return (PropertyInfo)((MemberExpression)body).Member;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
