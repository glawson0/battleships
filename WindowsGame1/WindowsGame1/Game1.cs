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
using Coding4Fun.Kinect.Wpf;

namespace WindowsGame1
{
   /// <summary>
   /// This is the main type for your game
   /// default size 800x600
   /// </summary>
   public class Game1 : Microsoft.Xna.Framework.Game
   {
      GraphicsDeviceManager graphics;
      SpriteBatch spriteBatch;
      KinectSensor kinect;
      Texture2D texture;
      Hand RightHand, LeftHand;
      const int maxSkells = 6;
      Skeleton[] allSkells = new Skeleton[maxSkells];
      List<Boat> Boats;

      public Game1()
      {
         graphics = new GraphicsDeviceManager(this);
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
         if (/*(hand.Depth > Elbow.Depth * 0.92) /*&& (hand.Y < (1.3 * Elbow.Y))*/ Shoulder.Depth >= (hand.Depth + 250)  ) 
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

      void kinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
      {
         Skeleton s = getSkeletons(e);
         if (s == null)
         {
            return;
         }
         using (DepthImageFrame depth = e.OpenDepthImageFrame())
         {
            if (depth == null)
            {
               return;
            }
            DepthImagePoint RH = depth.MapFromSkeletonPoint(s.Joints[JointType.HandRight].Position);
            DepthImagePoint RE = depth.MapFromSkeletonPoint(s.Joints[JointType.ElbowRight].Position);
            DepthImagePoint RS = depth.MapFromSkeletonPoint(s.Joints[JointType.ShoulderRight].Position);
            RightHand.updateHand(RH);
            ProcSide(RH,RE,RS, RightHand, "RightHand");

            DepthImagePoint LH = depth.MapFromSkeletonPoint(s.Joints[JointType.HandLeft].Position);
            DepthImagePoint LE = depth.MapFromSkeletonPoint(s.Joints[JointType.ElbowLeft].Position);
            DepthImagePoint LS = depth.MapFromSkeletonPoint(s.Joints[JointType.ShoulderLeft].Position);
            LeftHand.updateHand(LH);
            ProcSide(LH, LE, LS, LeftHand, "LeftHand");
             
         }
      }
      protected override void Initialize()
      {
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
         // Create a new SpriteBatch, which can be used to draw textures.
         spriteBatch = new SpriteBatch(GraphicsDevice);
         texture = Content.Load<Texture2D>(@"images/wilsun");
         RightHand = new Hand(new Vector2(200, 300), texture);
         LeftHand = new Hand(new Vector2(200, 300), texture);
         texture = Content.Load<Texture2D>(@"images/bert");
         for (int i = 0; i < 4; i++)
         {
            Boats.Add(new Boat(new Vector2(100*i,100*i), texture,0,0));

         }

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

      protected Skeleton getSkeletons(AllFramesReadyEventArgs e)
      {
         using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
         {
            if (skeletonFrameData == null)
            {
               return null;
            }

            skeletonFrameData.CopySkeletonDataTo(allSkells);
            Skeleton first = (from s in allSkells
                              where s.TrackingState == SkeletonTrackingState.Tracked
                              select s).FirstOrDefault();
            return first;
         }
      }
      protected override void Update(GameTime gameTime)
      {
         // Allows the game to exit
         if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            this.Exit();

         // TODO: Add your update logic here
         
         // public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
         // {
         //    Random rnd = new Random();
         //    return source.OrderBy<T, int>((item) => rnd.Next());
         // }
         boats = boats.OrderBy(item=>rnd.next());
         
         foreach (Boat b in Boats)
         {
            b.act(Boats);
         }
         LeftHand.act(Boats);
         RightHand.act(Boats);
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
         RightHand.Draw(spriteBatch);
         LeftHand.Draw(spriteBatch);
         foreach( Boat b in Boats)
         {
            b.Draw(spriteBatch);
         }
         base.Draw(gameTime);
         spriteBatch.End();
      }
   }
}
