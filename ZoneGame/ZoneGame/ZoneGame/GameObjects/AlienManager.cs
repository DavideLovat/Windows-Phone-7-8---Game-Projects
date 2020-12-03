using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace ZoneGame
{
    enum Danger
    {
        Low,
        Medium,
        High
    }

    class AlienManager : ISampleComponent
    {
        Random random;

        #region Fields spawn alien

        TimeSpan spawnTimer = TimeSpan.FromSeconds(5);
        TimeSpan timeChangeDanger = TimeSpan.FromSeconds(20);
        TimeSpan changeDangerState = TimeSpan.FromSeconds(20);

        public Danger DangerState
        {
            get { return dangerState; }
        }
        Danger dangerState = Danger.Low;

        int maxNumAlien = 10;

        #endregion

        #region Constants
        // Number of Flock members
        const int flockSize = 1;
        #endregion

        #region Fields

        protected AnimatingSprite
            idleSprite,
            walkingSprite,
            dyingSprite,
            attackSprite;

        //birds that fly out of the boundry(screen) will wrap around to 
        //the other side

        public bool IsSpawn
        {
            get { return isSpawn; }
            set { isSpawn = value; }
        }
        bool isSpawn = false;

        public int Score
        {
            get { return score; }            
        }
        int score = 10;

        World world;
        Camera camera;

        List<Alien> aliens = new List<Alien>();

        public List<Alien> Aliens
        {
            get { return aliens; }
        }

        /// <summary>
        /// Parameters flock members use to move and think
        /// </summary>
        public AIParameters FlockParams
        {
            get
            {
                return FlockParams;
            }

            set
            {
                flockParams = value;
            }
        }
        protected AIParameters flockParams;

        protected Player playerActor;

        #endregion

        #region Initialization

        public AlienManager(
            AnimatingSprite idleSprite,
            AnimatingSprite walkingSprite,
            AnimatingSprite dyingSprite,
            AnimatingSprite attackSprite,
            World world,
            Camera camera,
            AIParameters flockParameters, Player playerActor)
        {           
            this.idleSprite = idleSprite;
            this.walkingSprite = walkingSprite;
            this.dyingSprite = dyingSprite;
            this.attackSprite = attackSprite;
            this.world = world;
            this.camera = camera;

            aliens = new List<Alien>();
            this.flockParams = flockParameters;
            this.playerActor = playerActor;
            random = new Random();
            //ResetFlock();
        }

        #endregion

        #region Update and Draw

        public void Update(GameTime gameTime)
        {
            if(isSpawn)
                Spawn(gameTime);

            ChangeState(gameTime);

            List<Alien> alienToRemove = new List<Alien>();

            foreach (Alien thisAlien in aliens)
            {                
                if (thisAlien.RemoveAlien)
                {
                    alienToRemove.Add(thisAlien);
                    continue;
                }

                thisAlien.ResetThink();                
                               
                foreach (Alien otherAlien in aliens)
                {
                    if (thisAlien != otherAlien)
                    {
                        thisAlien.ReactTo(otherAlien, ref flockParams);
                    }
                }

                thisAlien.ReactTo(playerActor, ref flockParams);

                thisAlien.Update(gameTime, ref flockParams);
            }

            foreach (Alien a in alienToRemove)
            {
                Aliens.Remove(a);
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (Alien theAlien in aliens)
            {
                theAlien.Draw(spriteBatch, gameTime);
            }
        }

        #endregion

        #region Methods

        public void ResetFlock()
        {
            aliens.Clear();
            aliens.Capacity = flockSize;

            Alien tempAlien;
            Vector2 tempDir;
            Vector2 tempLoc;

            Random random = new Random();

            for (int i = 0; i < flockSize; i++)
            {
                tempLoc = new Vector2((float)
                    random.Next((int)world.WorldSize.X), (float)random.Next((int)world.WorldSize.Y));
                tempDir = new Vector2((float)
                    random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f);
                tempDir = playerActor.Position - tempLoc;

                tempDir.Normalize();                

                tempAlien = new Alien(idleSprite.DeepCopy(),
                    walkingSprite.DeepCopy(),
                    dyingSprite.DeepCopy(),
                    attackSprite.DeepCopy(),    
                    world.WorldSize, playerActor);
                tempLoc = new Vector2(tempLoc.X, tempLoc.Y);
                tempAlien.Position = tempLoc;
                tempAlien.CardDirection = CardinalDirection.NorthWest;
                tempAlien.ResetAnimation(false);
                aliens.Add(tempAlien);
            }
        }

        #endregion

        #region Change Danger State

        public void ChangeState(GameTime gameTime)
        {
            changeDangerState -= gameTime.ElapsedGameTime;
            if (changeDangerState <= TimeSpan.Zero)
            {
                double val = random.NextDouble();

                if (val >= 0.9)
                {
                    dangerState = Danger.High;
                }
                else if (val >= 0.4)
                {
                    dangerState = Danger.Medium;
                }
                else
                {
                    dangerState = Danger.Low;
                }

                changeDangerState = timeChangeDanger;
            }
        }

        #endregion

        #region Spawn Alien Methods

        public void Spawn(GameTime gameTime)
        {
            spawnTimer -= gameTime.ElapsedGameTime;
            if(spawnTimer <= TimeSpan.Zero)
            {
                int activeAlien = 0;
                foreach (Alien a in aliens)
                {
                    if (!a.IsDeadOrDying)
                        activeAlien++;
                }

                if (activeAlien < maxNumAlien)
                {
                    int numToSpawn = 0;

                    switch (dangerState)
                    {
                        case Danger.Low:
                            numToSpawn = random.Next(1, 3);
                            break;
                        case Danger.Medium:
                            numToSpawn = random.Next(5, 10);
                            break;
                        case Danger.High:
                            numToSpawn = random.Next(8, 10);
                            break;
                    }

                    if(maxNumAlien - aliens.Count < numToSpawn)
                        numToSpawn -= maxNumAlien - aliens.Count;

                    Rectangle view = new Rectangle(
                        (int)camera.VisibleArea.X,
                        (int)camera.VisibleArea.Y,
                        (int)camera.VisibleArea.Width,
                        (int)camera.VisibleArea.Height);

                    for (int i = 0; i < numToSpawn; i++)
                    {
                        Alien tempAlien = new Alien(idleSprite.DeepCopy(),
                            walkingSprite.DeepCopy(),
                            dyingSprite.DeepCopy(),
                            attackSprite.DeepCopy(),
                            world.WorldSize, playerActor);
                        tempAlien.CardDirection = CardinalDirection.North;
                        tempAlien.ResetAnimation(false);
                        tempAlien.CardDirection = SetCardinalDirection();

                        if (random.Next() % 2 == 0)
                        {
                            if (random.Next() % 2 == 0)
                            {
                                tempAlien.Position = new Vector2(
                                    tempAlien.CurrentFrameSize.X +
                                    view.X + view.Width + 10,
                                    view.Y +
                                    (float)random.Next((int)view.Height)); 
                            }
                            else
                            {
                                tempAlien.Position = new Vector2(
                                    -tempAlien.CurrentFrameSize.X +
                                    view.X - 10,
                                    view.Y +
                                    (float)random.Next((int)view.Height));
                            }
                            AdjustPositionY(tempAlien);
                        }
                        else
                        {
                            if (random.Next() % 2 == 0)
                            {
                                tempAlien.Position = new Vector2(
                                    view.X +
                                    (float)random.Next((int)view.Width),
                                    tempAlien.CurrentFrameSize.Y +
                                    view.Y + 10 +
                                    view.Height);
                            }
                            else
                            {
                                tempAlien.Position = new Vector2(
                                    view.X +
                                    (float)random.Next((int)view.Width),
                                    -tempAlien.CurrentFrameSize.Y +
                                    view.Y - 10);
                            }
                            AdjustPositionX(tempAlien);
                        }

                        aliens.Add(tempAlien);
                    }

                    SetSpawnTimer();
                }
                else
                {
                    SetSpawnTimer();
                }
            }
        }

        protected void SetSpawnTimer()
        {
            int time = 0;

            switch (dangerState)
            {
                case Danger.Low:
                    time = random.Next(5, 10);
                    break;
                case Danger.Medium:
                    time = random.Next(7, 10);
                    break;
                case Danger.High:
                    time = random.Next(10, 15);
                    break;
            }

            spawnTimer = TimeSpan.FromSeconds(time);
        }

        protected CardinalDirection SetCardinalDirection()
        {
            CardinalDirection card;
            int num = random.Next(1, 8);
            switch(num)
            {
                case 1:
                    card = CardinalDirection.North;
                    break;
                case 2:
                    card = CardinalDirection.NorthWest;
                    break;
                case 3:
                    card = CardinalDirection.NorthEast;
                    break;
                case 4:
                    card = CardinalDirection.West;
                    break;
                case 5:
                    card = CardinalDirection.East;
                    break;
                case 6:
                    card = CardinalDirection.South;
                    break;
                case 7:
                    card = CardinalDirection.SouthWest;
                    break;
                case 8:
                    card = CardinalDirection.SouthEast;
                    break;
                default:
                    card = CardinalDirection.North;
                    break;
            }

            return card;
        }

        protected void AdjustPositionY(Alien tempAlien)
        {
            Rectangle rect = world.SafeArea;
            Vector2 pos = tempAlien.Position;

            if (rect.Contains(new Point((int)pos.X, (int)pos.Y)))
            {
                if (pos.Y < rect.Y + rect.Height / 2)
                {
                    pos.Y = rect.Y - tempAlien.CurrentFrameSize.Y;
                }
                else
                {
                    pos.Y = rect.Y + rect.Height;
                }
            }

            tempAlien.Position = pos;
        }

        protected void AdjustPositionX(Alien tempAlien)
        {
            Rectangle rect = world.SafeArea;
            Vector2 pos = tempAlien.Position;

            if (rect.Contains(new Point((int)pos.X, (int)pos.Y)))
            {
                if (pos.X < rect.X + rect.Width / 2)
                {
                    pos.X = rect.X - tempAlien.CurrentFrameSize.X;
                }
                else
                {
                    pos.Y = rect.X + rect.Width;
                }
            }

            tempAlien.Position = pos;
        }

        #endregion
    }
}
