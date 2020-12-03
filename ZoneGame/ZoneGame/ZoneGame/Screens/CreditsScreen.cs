using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace ZoneGame
{
    class CreditsScreen : GameScreen
    {
        #region Fields

        const string CreditsDocPath = "Content/Documents/CreditsContent.xml";

        //Texture2D replayTexture;

        SpriteFont font14px;

        List<KeyValuePair<Text, List<Text>>> actors = new List<KeyValuePair<Text, List<Text>>>();

        //ImageSelectable replayButton;

        Vector2 position;

        Vector2 speed;

        int actorSpace = 40;
        int actornameSpace = 10;
        int nameSpace = 5;
        int creditsHeight;

        bool isPlay;

        Vector2 creditsStartPosition;
        

        #endregion

        #region Initialization

        public CreditsScreen()
        {
            EnabledGestures = GestureType.Tap; 
        }

        public override void LoadContent()
        {
            base.LoadContent();
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            //replayTexture = contentDynamic.Load<Texture2D>("Textures/Buttons/replayButton");
            font14px =  contentDynamic.Load<SpriteFont>("Fonts/font14px");
            /*
            replayButton = new ImageSelectable(replayTexture);
            replayButton.AlphaChannel = 0.9f;
            replayButton.Position = new Vector2((viewport.Width - replayButton.Width()) / 2, (viewport.Height - replayButton.Height()) / 2);
            replayButton.Selected += OnReplaySelected;
            */
            actors = LoadCreditsContent(font14px);
            creditsStartPosition = new Vector2(viewport.Width / 2, viewport.Height + actorSpace);
            InitializeCreditsPosition(creditsStartPosition, font14px);
            creditsHeight = GetHeight(font14px);

            position = new Vector2(0, 0);
            speed = new Vector2(0f,2f);

            isPlay = true;
        }

        #endregion

        #region Handle Input

        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.IsNewButtonPress(Buttons.Back))
            {
                OnCancel();
            }
            /*
            if (!isPlay)
            {
                foreach (GestureSample gesture in input.Gestures)
                {
                    if (gesture.GestureType == GestureType.Tap)
                    {
                        Point tapLocation = new Point((int)gesture.Position.X, (int)gesture.Position.Y);

                        if (replayButton.Bounds.Contains(tapLocation))
                        {
                            replayButton.OnSelected();
                        }
                    }
                }
            }
            */
            base.HandleInput(input);
        }

        #endregion

        #region Update and Draw

        public override void  Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (!isPlay)
            {
                OnCancel();
                return;
            }

            Vector2 pixelChange = speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            position += speed;
            if (position.Y > creditsStartPosition.Y + creditsHeight)
            {
                isPlay = false;
            }
            

 	        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.Black);

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // figure out a camera transformation based on the player's position
            Matrix cameraTransform = Matrix.CreateTranslation(
                -position.X,
                -position.Y,
                0f);
            
            // Disegna debug
            Vector2 debugPosition = Vector2.Zero;
            String stringDebug = String.Format("Posizione: {0} \n Matrice: {1}", position, cameraTransform);
            spriteBatch.Begin();
                
                spriteBatch.DrawString(font14px, stringDebug, debugPosition, Color.White);

            //Disegna bottone replay
            if (!isPlay)
            {
                //replayButton.Draw(spriteBatch, gameTime);                
            }

            spriteBatch.End();

            if (isPlay)
            {
                // begin drawing with SpriteBatch using our camera transformation
                // Disegna crediti 
                spriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.LinearClamp,
                    DepthStencilState.Default,
                    RasterizerState.CullNone,
                    null,
                    cameraTransform);
                /*
                for (int i = 0; i < actors.Count; i++)
                {
                    string actor = actors[i].Key;
                    Vector2 actorSize = font14px.MeasureString(actor);
                    creditsPosition.X = (viewport.Width - actorSize.X) / 2;
                    spriteBatch.DrawString(font14px, actor, creditsPosition, Color.White);
                
                    creditsPosition.Y += actorSize.Y + actorSpace;
                    for (int j = 0; j < actors[i].Value.Count; j++)
                    {
                        string name = actors[i].Value[j];
                        Vector2 nameSize = font14px.MeasureString(name);
                        creditsPosition.X = (viewport.Width - actorSize.X) / 2;
                        spriteBatch.DrawString(font14px, name, creditsPosition, Color.White);
                        creditsPosition.Y += nameSize.Y + 5;
                    }
                }
                */
                for (int i = 0; i < actors.Count; i++)
                {
                    actors[i].Key.Draw(spriteBatch, gameTime); ;

                    for (int j = 0; j < actors[i].Value.Count; j++)
                    {
                        actors[i].Value[j].Draw(spriteBatch, gameTime); ;
                    }
                }

                spriteBatch.End();
            }
        }

        #endregion

        #region Privare Methods
        /*
        List<KeyValuePair<string, List<string>>> LoadCreditsContent()
        {
            List<KeyValuePair<string, List<string>>> listActors = new List<KeyValuePair<string,List<string>>>();
            XDocument creditsDoc = XDocument.Load(CreditsDocPath);
            foreach (XElement actor in creditsDoc.Document.Element("Actors").Elements("Actor"))
            {
                List<string> nameList = new List<string>();
                string actorText = actor.Element(SettingsManager.Language.ToString()).Value;
                foreach (XElement name in actor.Elements("Name"))
                {
                    nameList.Add(name.Value);
                }
                listActors.Add(new KeyValuePair<string, List<string>>(actorText, nameList));
            }
            return listActors;
        }
        */
        List<KeyValuePair<Text, List<Text>>> LoadCreditsContent(SpriteFont font)
        {
            List<KeyValuePair<Text, List<Text>>> listActors = new List<KeyValuePair<Text, List<Text>>>();
            XDocument creditsDoc = XDocument.Load(CreditsDocPath);
            foreach (XElement actor in creditsDoc.Document.Element("Actors").Elements("Actor"))
            {
                List<Text> nameList = new List<Text>();
                Text actorText = new Text(actor.Element(SettingsManager.Language.ToString()).Value, font);
                foreach (XElement name in actor.Elements("Name"))
                {
                    nameList.Add(new Text(name.Value, font));
                }
                listActors.Add(new KeyValuePair<Text, List<Text>>(actorText, nameList));
            }
            return listActors;
        }

        void InitializeCreditsPosition(Vector2 position, SpriteFont font)
        {
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            Vector2 creditsPosition = position; 

            for (int i = 0; i < actors.Count; i++)
            {
                Text textActor = actors[i].Key;

                textActor.Position = new Vector2(creditsPosition.X - (textActor.Width()) / 2, creditsPosition.Y); 
                creditsPosition.Y += textActor.Height() + actornameSpace;
                for (int j = 0; j < actors[i].Value.Count; j++)
                {
                    Text textName = actors[i].Value[j];
                    textName.Position = new Vector2(creditsPosition.X - (textName.Width()) / 2, creditsPosition.Y);
                    creditsPosition.Y += textName.Height();
                    if (j < actors[i].Value.Count - 1)
                    {
                        creditsPosition.Y += nameSpace;
                    }
                    else
                    {
                        creditsPosition.Y += actorSpace;
                    }
                }
            }
        }

        int GetHeight(SpriteFont font)
        {
            int height = 0;

            for (int i = 0; i < actors.Count; i++)
            {
                Text textActor = actors[i].Key;
                height += textActor.Height() + actornameSpace;
                for (int j = 0; j < actors[i].Value.Count; j++)
                {
                    Text textName = actors[i].Value[j];
                    if (j < actors[i].Value.Count - 1)
                    {
                        height += textName.Height() + nameSpace;
                    }
                    else
                    {
                        height += textName.Height() + actorSpace;
                    }
                }
            }

            return height;
        }

        /*
        void OnReplaySelected(object sender, EventArgs e)
        {
            position = Vector2.Zero;
            isPlay = true;
        }
        */

        protected virtual void OnCancel()
        {
            ExitScreen();
            
            ReplaceAllScreens(new List<GameScreen>{new BackgroundScreen("titleScreen"), new MainMenuScreen()});
        }

        #endregion

        
    }
}
