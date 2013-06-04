using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;

namespace WindowsGame1
{
   class Boat
   {
      public Vector2 m_position;
      public Vector2 m_destination;
      public Texture2D m_texture;
      public int direction; // 0 north, 1 northeast, 2 east ...
      public int health;
      public int armor;
      public int dmg1;
      public int dmg2;
      public int dmg3;
      public int range;
      public int owner;
      public int cooldown;
      public int attackTime;

      // type: 0 = battleship, 1 = destroyer, 2 = submarine
      public Boat(Vector2 position, Texture2D texture, int type, int owner)
      {
         m_position = position;
         m_destination = position;
         m_texture = texture;
         direction = 0;
         Color[] colors = new Color[texture.Width * texture.Height];
         texture.GetData<Color>(colors);
         this.owner = owner;
         cooldown = 0;
         health = 30;
         attackTime = 50;
         range = 20;
         switch (type)
         {
            case 0:
               armor = 3;
               dmg1 = 6;
               dmg2 = 15;
               dmg3 = 10;      
               break;
            case 1:
               armor = 2;
               dmg1 = 15;
               dmg2 = 10;
               dmg3 = 6;
               break;
            case 2:
               armor = 1;
               dmg1 = 6;
               dmg2 = 10;
               dmg3 = 15;
               break;
         }
      }

      public void act(List<Boat> Boats)
      {
         // try to move toward destination from position
         // update direction based on movement
         // update texture to direction moving
         // attack any units within range
         bool up = false, down = false, left = false, right = false;
         if (m_position.X < m_destination.X)
         {
            m_position.X += 1;
            right = true;
         }
         else if (m_position.X > m_destination.X)
         {
            m_position.X -= 1;
            left = true;
         }
         
         if (m_position.Y < m_destination.Y)
         {
            m_position.Y += 1;
            down = true;
         }
         else if (m_position.Y > m_destination.Y)
         {
            m_position.Y -= 1;
            up = true;
         }

         if (up && !down && !left && !right)
            direction = 0;
         if (up && !down && !left && right)
            direction = 1;
         if (!up && !down && !left && right)
            direction = 2;
         if (!up && down && !left && right)
            direction = 3;
         if (!up && down && !left && !right)
            direction = 4;
         if (!up && down && left && !right)
            direction = 5;
         if (!up && !down && left && !right)
            direction = 6;
         if (up && !down && left && !right)
            direction = 7;

         if (cooldown != 0)
            cooldown--;

         Boat closestAttackableBoat = null;
         float closestBoatDistance = 0.0;
         foreach(Boat b in Boats)
         {
            if (b != this)
            {
               if (b.owner != this.owner && this.cooldown == 0)
               {
                  float tempDistance = Vector2.Distance(b.m_position, this.m_position);
                  if (tempDistance <= this.range)
                  {
                     if (closestAttackableBoat == null) {
                        closestAttackableBoat = b;
                        closestBoatDistance = tempDistance;
                     }
                     else if (tempDistance < closestBoatDistance) {
                        closestAttackableBoat = b;
                        closestBoatDistance = tempDistance;
                     }
                  }
               }
            }
         }
         
         if (closestAttackableBoat != null) {
            switch (closestAttackableBoat.armor)
            {
               case 1:
                  closestAttackableBoat.health -= this.dmg1;                        
                  break;
               case 2:
                  closestAttackableBoat.health -= this.dmg2;
                  break;
               case 3:
                  closestAttackableBoat.health -= this.dmg3;
                  break;
            }
            this.cooldown = this.attackTime;
         }
      }


      public void setDest(Vector2 dest)
      {
         m_destination = dest;
      }

      public void Draw(SpriteBatch s)
      {
         s.Draw(m_texture, m_position, null, Color.White, 0f, new Vector2(m_texture.Width / 2, m_texture.Height / 2),
                1f, SpriteEffects.None, 0f);
      }


   }
}
