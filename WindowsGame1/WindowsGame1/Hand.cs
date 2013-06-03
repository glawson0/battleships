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
   class Hand
   {
      private Vector2 m_position;
      private Texture2D m_texture;
      private DepthImagePoint P;
      public bool pressed = false;
      private Boat selected;


      public void updateHand (DepthImagePoint newP){
         P = newP;
         m_position.X = P.X;
         m_position.Y = P.Y;
      }

      public void act(List<Boat> Boats)
      {


         if (selected == null && pressed == true)
         {
            foreach (Boat b in Boats)
            {
               //make selection detect ownership
               if ( (b.m_position.X + (b.m_texture.Width / 2) >= m_position.X) && (b.m_position.X - (b.m_texture.Width / 2) <= m_position.X)
                  && (b.m_position.Y + (b.m_texture.Height / 2) >= m_position.Y) && (b.m_position.Y - (b.m_texture.Height / 2) <= m_position.Y) )
               {
                  
                  selected = b;
                  break;
               }

            }
         }
         else if (pressed==false && selected!= null)
         {
            selected.m_destination = new Vector2(m_position.X, m_position.Y);
            selected = null;
         }

      }
      public Hand(Vector2 position, Texture2D texture)
        {
            m_position = position;
            m_texture = texture;
            Color[] colors = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(colors);
        }

      public void Draw(SpriteBatch s)
      {
         if (!pressed)
         {
            s.Draw(m_texture, m_position, null, Color.White, 0f, new Vector2(m_texture.Width / 2, m_texture.Height / 2),
                1f, SpriteEffects.None, 0f);
         }
         else
         {
            s.Draw(m_texture, m_position, null, Color.White, 180f, new Vector2(m_texture.Width / 2, m_texture.Height / 2),
             1f, SpriteEffects.None, 0f);
         }
      }

   }


}
