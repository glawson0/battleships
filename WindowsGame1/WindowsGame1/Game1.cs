using System;
using System.Collections.Generic;
using System.Linq;
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
   /// <summary>
   /// This is the main type for your game
   /// default size 800x600
   /// </summary>
   public class Game1 : Microsoft.Xna.Framework.Game
   {
       Boolean exiting = false;
       int menuTimer = 0;
       Boolean inMenu = false;
       int gameOverTimer = 0;
       Boolean gameOver = false;
       int playerWins = -1;
       int height = 700;
       int width = 900;
      GraphicsDeviceManager graphics;
      SpriteBatch spriteBatch;
      KinectSensor kinect;
      Texture2D texture;
      Texture2D textureAttack;
      Hand P1RightHand, P1LeftHand, P2RightHand, P2LeftHand;
      const int maxSkells = 6;
      Skeleton[] allSkells = new Skeleton[maxSkells];
      List<Boat> Boats;
      SoundEffect soundEffect;
      SoundEffectInstance soundEffectInstance;

      public Game1()
      {
         graphics = new GraphicsDeviceManager(this);
         graphics.PreferredBackBufferHeight = height;
         graphics.PreferredBackBufferWidth = width;
         Content.RootDirectory = "Content";
      }

      /// <summary>
      /// Allows the game to perform any initialization it needs to before starting to run.
      /// This is where it can query for any required services and load any non-graphic
      /// related content.  Calling base.Initialize will enumerate through any components
      /// and initialize them as well.
      /// </summary>
      /// 
      void ProcSide(DepthImagePoint hand, DepthImagePoint Elbow, DepthImagePoint Shoulder, Hand obj, String name)
      {
         double UpArm = Math.Sqrt(((Elbow.X - Shoulder.X) * (Elbow.X - Shoulder.X)) + ((Elbow.Y - Shoulder.Y) * (Elbow.Y - Shoulder.Y)) + ((Elbow.Depth - Shoulder.Depth) * (Elbow.Depth - Shoulder.Depth)));
         double forArmD = (Elbow.Depth - hand.Depth);
         //(hand.Y < (.8 * Elbow.Y) || Elbow.Y < (Shoulder.Y + (UpArm / 2)))
         //Console.Write(" {0}, {1}, {2}\n", hand.Depth > Elbow.Depth * 0.92, hand.Depth, Elbow.Depth);
         if (inMenu == true)
         {
             
             if (hand.X <= 400 && hand.X >= 100 && hand.Y <= 400 && hand.Y >= 300)
             {
                 menuTimer++;
                 if (menuTimer >= 100)
                 {
                     inMenu = false;
                 }
             }
             else if (hand.X <= 720 && hand.X >= 420 && hand.Y <= 400 && hand.Y >= 300)
             {
                 menuTimer++;
                 if (menuTimer >= 100)
                 {
                     exiting = true;
                 }
             }
             else
             {
                 
             }
         }
         else
         {
             if (hand.X <= Shoulder.X + 50 && hand.X >= Shoulder.X - 50
                 && hand.Y <= Shoulder.Y + 50 && hand.Y >= Shoulder.Y - 50
                 && hand.Depth <= Shoulder.Depth + 50 && hand.Depth >= Shoulder.Depth - 50) {
                     inMenu = true;
             }
             else if (/*(hand.Depth > Elbow.Depth * 0.92) /*&& (hand.Y < (1.3 * Elbow.Y))*/ Shoulder.Depth >= (hand.Depth + 250))
             {
                 if (Shoulder.Depth >= (hand.Depth + 325))
                 {
                     obj.pressed = true;
                 }
                 else
                 {
                     obj.pressed = false;
                 }
             }
             else
             {
                 //Console.Write("{0} Rest\n", name);
             }
             //Console.Write("UpArm: {0}, ForArmD: {1}, Y: {2}\n", UpArm, forArmD, hand.Y);
         }
      }

      void kinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
      {
         getSkeletons(e);
         List<Skeleton> trackedSkells= new List<Skeleton>();
         foreach(Skeleton s in allSkells) {
             if (s.TrackingState == SkeletonTrackingState.Tracked)
                 trackedSkells.Add(s);
         }

         Skeleton p1s=null, p2s=null;
         if (trackedSkells.Count > 0)
          p1s = trackedSkells.ElementAt(0);

         if (trackedSkells.Count > 1)
            p2s = trackedSkells.ElementAt(1);

         if (p1s == null)
         {
            return;
         }

         using (DepthImageFrame depth = e.OpenDepthImageFrame())
         {
            if (depth == null)
            {
               return;
            }
            DepthImagePoint RH = depth.MapFromSkeletonPoint(p1s.Joints[JointType.HandRight].Position);
            DepthImagePoint RE = depth.MapFromSkeletonPoint(p1s.Joints[JointType.ElbowRight].Position);
            DepthImagePoint RS = depth.MapFromSkeletonPoint(p1s.Joints[JointType.ShoulderRight].Position);
            P1RightHand.updateHand(RH);
            ProcSide(RH,RE,RS, P1RightHand, "RightHand");

            DepthImagePoint LH = depth.MapFromSkeletonPoint(p1s.Joints[JointType.HandLeft].Position);
            DepthImagePoint LE = depth.MapFromSkeletonPoint(p1s.Joints[JointType.ElbowLeft].Position);
            DepthImagePoint LS = depth.MapFromSkeletonPoint(p1s.Joints[JointType.ShoulderLeft].Position);
            P1LeftHand.updateHand(LH);
            ProcSide(LH, LE, LS, P1LeftHand, "LeftHand");
             
         }

         if (p2s == null)
         {
             return;
         }
         using (DepthImageFrame depth = e.OpenDepthImageFrame())
         {
             if (depth == null)
             {
                 return;
             }
             DepthImagePoint RH = depth.MapFromSkeletonPoint(p2s.Joints[JointType.HandRight].Position);
             DepthImagePoint RE = depth.MapFromSkeletonPoint(p2s.Joints[JointType.ElbowRight].Position);
             DepthImagePoint RS = depth.MapFromSkeletonPoint(p2s.Joints[JointType.ShoulderRight].Position);
             P2RightHand.updateHand(RH);
             ProcSide(RH, RE, RS, P2RightHand, "RightHand");

             DepthImagePoint LH = depth.MapFromSkeletonPoint(p2s.Joints[JointType.HandLeft].Position);
             DepthImagePoint LE = depth.MapFromSkeletonPoint(p2s.Joints[JointType.ElbowLeft].Position);
             DepthImagePoint LS = depth.MapFromSkeletonPoint(p2s.Joints[JointType.ShoulderLeft].Position);
             P2LeftHand.updateHand(LH);
             ProcSide(LH, LE, LS, P2LeftHand, "LeftHand");

         }
      }
      protected override void Initialize()
      {
          soundEffect = Content.Load<SoundEffect>(@"sounds/PipsqueakPirateShip");
          soundEffectInstance = soundEffect.CreateInstance();
          soundEffectInstance.IsLooped = true;
          soundEffectInstance.Play();

         // TODO: Add your initialization logic here
         if (KinectSensor.KinectSensors.Count > 0)
         {
            kinect = KinectSensor.KinectSensors[0];
            if (kinect.Status == KinectStatus.Connected)
            {
               kinect.DepthStream.Enable();
               kinect.SkeletonStream.Enable();
               kinect.Start();
               kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(kinect_AllFramesReady);
            }
         }
         Boats = new List<Boat>();
         base.Initialize();
      }

      /// <summary>
      /// LoadContent will be called once per game and is the place to load
      /// all of your content.
      /// </summary>
      protected override void LoadContent()
      {
          SoundEffect tempSounds = Content.Load<SoundEffect>(@"sounds/bomb");

         // Create a new SpriteBatch, which can be used to draw textures.
         spriteBatch = new SpriteBatch(GraphicsDevice);
         texture = Content.Load<Texture2D>(@"images/p2target");
         P1RightHand = new Hand(new Vector2(200, 300), texture,0);
         P1LeftHand = new Hand(new Vector2(200, 300), texture,0);

         texture = Content.Load<Texture2D>(@"images/p1target");
         P2RightHand = new Hand(new Vector2(100, 300), texture,1);
         P2LeftHand = new Hand(new Vector2(100, 300), texture,1);
         
         texture = Content.Load<Texture2D>(@"images/p1battleship");
         textureAttack = Content.Load<Texture2D>(@"images/p1battleshipshooting");
         Boats.Add(new Boat(new Vector2(100,100), texture, textureAttack, 0, 0, tempSounds));
         Boats.Add(new Boat(new Vector2(100, 475), texture, textureAttack, 0, 0, tempSounds));

         texture = Content.Load<Texture2D>(@"images/p1destroyer");
         textureAttack = Content.Load<Texture2D>(@"images/p1destroyershooting");
         Boats.Add(new Boat(new Vector2(100, 175), texture, textureAttack, 0, 0, tempSounds));
         Boats.Add(new Boat(new Vector2(100, 400), texture, textureAttack, 0, 0, tempSounds));

         texture = Content.Load<Texture2D>(@"images/p1sub");
         textureAttack = Content.Load<Texture2D>(@"images/p1subshooting");
         Boats.Add(new Boat(new Vector2(100, 250), texture, textureAttack, 0, 0, tempSounds));
         Boats.Add(new Boat(new Vector2(100, 325), texture, textureAttack, 0, 0, tempSounds));

         texture = Content.Load<Texture2D>(@"images/p2battleship");
         textureAttack = Content.Load<Texture2D>(@"images/p2battleshipshooting");
         Boats.Add(new Boat(new Vector2(600, 100), texture, textureAttack, 0, 1, tempSounds));
         Boats.Add(new Boat(new Vector2(600, 475), texture, textureAttack, 0, 1, tempSounds));

         texture = Content.Load<Texture2D>(@"images/p2destroyer");
         textureAttack = Content.Load<Texture2D>(@"images/p2destroyershooting");
         Boats.Add(new Boat(new Vector2(600, 175), texture, textureAttack, 0, 1, tempSounds));
         Boats.Add(new Boat(new Vector2(600, 400), texture, textureAttack, 0, 1, tempSounds));

         texture = Content.Load<Texture2D>(@"images/p2sub");
         textureAttack = Content.Load<Texture2D>(@"images/p2subshooting");
         Boats.Add(new Boat(new Vector2(600, 250), texture, textureAttack, 0, 1, tempSounds));
         Boats.Add(new Boat(new Vector2(600, 325), texture, textureAttack, 0, 1, tempSounds));
         // TODO: use this.Content to load your game content here
      }

      /// <summary>
      /// UnloadContent will be called once per game and is the place to unload
      /// all content.
      /// </summary>
      protected override void UnloadContent()
      {
         // TODO: Unload any non ContentManager content here
         //kinect.Stop();
      }

      /// <summary>
      /// Allows the game to run logic such as updating the world,
      /// checking for collisions, gathering input, and playing audio.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>

      protected void getSkeletons(AllFramesReadyEventArgs e)
      {
         using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
         {
         /*
            if (skeletonFrameData == null)
            {
               return null;
            }
         */
            skeletonFrameData.CopySkeletonDataTo(allSkells);
         /*   Skeleton first = (from s in allSkells
                              where s.TrackingState == SkeletonTrackingState.Tracked
                              select s).FirstOrDefault(); */
         }
      }
      protected override void Update(GameTime gameTime)
      {
         // Allows the game to exit
         if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            this.Exit();

         // TODO: Add your update logic here

         if (gameOver == true) { }
         else if (inMenu == true)
         {

         }
         else
         {
             // public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
             // {
             //    Random rnd = new Random();
             //    return source.OrderBy<T, int>((item) => rnd.Next());
             // }
             /*Random rnd = new Random();
             Boats = Boats.OrderBy(item=>rnd.Next());*/
             int p1Boats = 0;
             int p2Boats = 0;

             for (int i = Boats.Count - 1; i > -1; i--)
             {
                 Boat b = Boats.ElementAt(i);
                 if (b.health > 0)
                 {
                     b.act(Boats);
                     if (b.owner == 0)
                         p1Boats++;
                     else
                         p2Boats++;
                 }
                 else
                     Boats.Remove(b);
             }

             if (p1Boats == 0)
                 playerWins = 2;

             if (p2Boats == 0)
                 playerWins = 1;

             P1LeftHand.act(Boats);
             P1RightHand.act(Boats);
             P2LeftHand.act(Boats);
             P2RightHand.act(Boats);
         }
         if (exiting == true)
         {
             this.Exit();
         }
         base.Update(gameTime);
      }

      /// <summary>
      /// This is called when the game should draw itself.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>
      protected override void Draw(GameTime gameTime)
      {         
         GraphicsDevice.Clear(Color.CornflowerBlue);

         // TODO: Add your drawing code here
         spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
         P1RightHand.Draw(spriteBatch);
         P1LeftHand.Draw(spriteBatch);
         P2RightHand.Draw(spriteBatch);
         P2LeftHand.Draw(spriteBatch);
         foreach( Boat b in Boats)
         {
            b.Draw(spriteBatch);
         }

         if (gameOver == true)
         {
             if (gameOverTimer >= 500)
             {
                 this.Exit();
             }
             gameOverTimer++;

             if (gameOverTimer > 150)
             {
                 texture = Content.Load<Texture2D>(@"images/credits");
                 spriteBatch.Draw(texture, new Vector2(0, 0), null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
             }
         }

         if (inMenu == true)
         {
             texture = Content.Load<Texture2D>(@"images/resume");
             spriteBatch.Draw(texture, new Vector2(100, 300), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
             texture = Content.Load<Texture2D>(@"images/exit");
             spriteBatch.Draw(texture, new Vector2(420, 300), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
         }

         if (playerWins == 2) {
             texture = Content.Load<Texture2D>(@"images/p2wins");
            spriteBatch.Draw(texture, new Vector2(450,400), null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
         }
         if (playerWins == 1) {
            texture = Content.Load<Texture2D>(@"images/p1wins");
            spriteBatch.Draw(texture, new Vector2(450,400), null, Color.White, 0f, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
         }

         base.Draw(gameTime);
         spriteBatch.End();

      }
   }
}
