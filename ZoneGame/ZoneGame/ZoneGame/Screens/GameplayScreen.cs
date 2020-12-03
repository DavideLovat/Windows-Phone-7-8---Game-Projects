using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Phone.Shell;

namespace ZoneGame
{
    public enum AmbientType
    {
        Forest,
        Snow,
        Steppe,
        Desert,
    }

    class GameplayScreen : GameScreen
    {
        #region Objective Fields      

        List<InGameComponent> Objectives = new List<InGameComponent>();

        public static bool isAddEngineObjectives = false;
        public static bool isAddShipObjectives = false;

        SwitchComponent objButton;

        #endregion

        #region Camera Fields

        public static Vector3 cameraTranslation = Vector3.Zero;

        #endregion

        #region FlockingAIFields

        // Default value for the AI parameters
        const float detectionDefault = 50f;//80.0f;
        const float separationDefault = 40f;//60.0f;
        const float moveInOldDirInfluenceDefault = 1.0f;
        const float moveInFlockDirInfluenceDefault = 1.0f;
        const float moveInRandomDirInfluenceDefault = 0.05f;
        const float maxTurnRadiansDefault = 6.0f;
        const float perMemberWeightDefault = 1.0f;
        const float perMemberWeightAlign = 1.0f;
        const float perMemberWeightCohesion = 1.0f;
        const float perMemberWeightSeparation = 1.0f;
        const float perDangerWeightDefault = 50.0f;

        const float detectionPreyDefault = 800;
        const float perPreyWeightDefault = 25.0f;

        // Do we need to update AI parameers this Update
        bool aiParameterUpdate = false;

        Texture2D flockTexture;

        AlienManager alienManager;

        AIParameters flockParams;

        Player playerActor;

        #endregion

        #region Worlds Data

        Random rand = new Random();

        AmbientType ambient = AmbientType.Forest;

        // desired width of backbuffer
        public static readonly int graphicsWidth = 800;

        // desired height of backbuffer
        public static readonly int graphicsHeight = 480;

        // half of the backbuffer width used for spawning enemies and creating stars
        public static readonly int graphicsWidthHalf = graphicsWidth / 2;

        // half of the backbuffer height used for spawning enemies and creating stars
        public static readonly int graphicsHeightHalf = graphicsHeight / 2;

        private World world;

        bool isOutOfSafeArea;

        int currentLevel = 0;

        public int CurrentLevel
        {
            get { return currentLevel; }
            set { currentLevel = value; }
        }

        static int maxLevel;

        public int MaxLevel
        {
            get { return maxLevel; }            
        }

        Texture2D grassText, treeText, bushText;
        Color[] treeData;

        #endregion

        #region Games State Data

        bool isUserWon = false;
        bool gameEnded = false;
        bool insertInHighscore = false;
        bool moveToHighscore = false;
        bool userTapToExit = false;

        public bool IsUserWon
        {
            get { return isUserWon; }
        }

        public bool GameEnded
        {
            get { return gameEnded; }
        }

        #endregion

        #region Fields

        string gameMusic = string.Empty;

        SpriteFont font14px;
        SpriteFont noticeFont;
        SpriteFont scoreFont;
        SpriteFont portraitsFont;        
        public static SpriteFont debugFont;
        int score;

        float left_radians = 0f;
        float left_angle = 0f;

        float right_radians = 0f;
        float right_angle = 0f;

        const string animationDocPath = "Content/Documents/AnimationsDefinition.xml";

        // Thumbstick textures
        Texture2D controlstickBoundary;
        Texture2D controlstick;
        // Objective button textures
        Texture2D objButtonOnTexture;
        Texture2D objButtonOffTexture;
        // player line aim texture
        Texture2D aimLine;
        // player base for objectives punter
        Texture2D baseTexture;
        // objective punter texture
        Texture2D arrowTexture;
        // bar textures
        Texture2D barBlackBorder, barGreenTexture, barYellowTexture, barRedTexture;
        // spaceship texture
        Texture2D spaceShipTexture;
        // engine textures
        Texture2D engineOnTexture;
        Texture2D engineOffTexture;
        // bullet Texture
        Texture2D bulletTexture;
        // Portraits Texture
        // Pilot portraits texture
        Texture2D pilotPortraitsTexture;
        // Textbox Texture
        Texture2D textBoxTexture;
        // arrow texture for Textbox
        Texture2D arrowBoxTexture;

        Vector2 left_controlstickBoundaryPosition;
        Vector2 right_controlstickBoundaryPosition;
        Vector2 lastTouchPosition;
        ThumbstickComponent leftThumbstick;
        ThumbstickComponent rightThumbstick;

        // Background Rectangle
        Rectangle background;
        // TextBox
        TextBox pilotTextBox;
        // Pilot textbox dialogs
        Dictionary<string, string> pilotDialogs = new Dictionary<string,string>();
        // Other Game Components
        public static ObjectiveFinder objFinder;
        SpaceShip spaceShip;
        EngineManager engineManager;
        List<Grass> grassList = new List<Grass>();
        List<Bush> bushList = new List<Bush>();
        List<Tree> treeList = new List<Tree>();
        Text scoreText, dangerText, engineText, noticeText; 

        List<ISampleComponent> mapComponents = new List<ISampleComponent>();
        List<InanimateGameComponent> solidComponents = new List<InanimateGameComponent>();

        Vector2 leftControlOldPosition = Vector2.Zero;
        Vector2 leftControlNewPosition = Vector2.Zero;        

        bool isRightControlstickReleased;
        bool isDialogScene;

        private bool isActive;

        public new bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        Camera camera;

        #endregion

        #region Initialization

        public GameplayScreen()
        {
            isActive = true;
            /*
            if (contentDynamic == null)
                contentDynamic = new ContentManager(ScreenManager.Game.Services, "Content");
             * */
            InitializeMaxLevelNumber();
            EnabledGestures = GestureType.Tap;       
        }

        #endregion

        #region debug fields

        public static Texture2D blank;

        bool debugVisible = false;

        #endregion

        #region Loading and Unloading

        public override void LoadContent()
        {
            base.LoadContent();
            MemoryStateManager.SaveCurrentGameState(currentLevel);
            PlaySounds(true);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        #region Load Methods

        public override void LoadAssets()
        {

            if (contentDynamic == null)
                contentDynamic = new ContentManager(ScreenManager.Game.Services, "Content");
            LanguageDefinitions = Reader.LoadLanguage("Gameplay");
            
            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            camera = new Camera(graphics);                        

            LoadAmbientType();
            LoadTextures();
            LoadSpriteFont();
            InitializeThumbstick();
            InitializeMapComponents();
            InitializeTextBox();
            LoadMusic();
        }

        private void LoadMusic()
        {
            XDocument WorldDoc = XDocument.Load("Content/Documents/WorldDefinitions.xml");

            foreach (var level in WorldDoc.Element("Levels").Descendants("Level"))
            {
                if (int.Parse(level.Attribute("id").Value) == currentLevel)
                {
                    if (level.Element("music") != null)
                    {
                        gameMusic = level.Element("music").Value.ToString();
                    }
                    else
                    {
                        gameMusic = "gameplayMusic";
                    }

                    break;
                }
            }
        }

        private void LoadAmbientType()
        {
            XDocument WorldDoc = XDocument.Load("Content/Documents/WorldDefinitions.xml");

            foreach (var level in WorldDoc.Element("Levels").Descendants("Level"))
            {
                if (int.Parse(level.Attribute("id").Value) == currentLevel)
                {
                    if (level.Element("ambient") != null)
                    {
                        switch (level.Element("ambient").Value)
                        {
                            case "Forest":
                                ambient = AmbientType.Forest;
                                break;
                            case "Snow":
                                ambient = AmbientType.Snow;
                                break;
                            case "Desert":
                                ambient = AmbientType.Desert;
                                break;
                            case "Steppe":
                                ambient = AmbientType.Steppe;
                                break;
                            default:
                                ambient = AmbientType.Forest;
                                break;
                        }
                    }
                    else
                    {
                        ambient = AmbientType.Forest;
                    }

                    break;
                }
            }
        }

        private void LoadTextures()
        {
            //debug
            blank = contentDynamic.Load<Texture2D>("Textures/blank");
            
            // Textbox texture
            pilotPortraitsTexture = contentDynamic.Load<Texture2D>("Textures/Objects/Portraits/pilot");
            textBoxTexture = contentDynamic.Load<Texture2D>("Textures/GameScreens/textBox");
            arrowBoxTexture = contentDynamic.Load<Texture2D>("Textures/Buttons/arrowBox");

            aimLine = CreateAimLineTexture();
            objButtonOnTexture = contentDynamic.Load<Texture2D>("Textures/Objects/ObjectiveButtons/objectiveButtonOn");
            objButtonOffTexture = contentDynamic.Load<Texture2D>("Textures/Objects/ObjectiveButtons/objectiveButtonOff");
            baseTexture = contentDynamic.Load<Texture2D>("Textures/Objects/Player/base_trans");
            arrowTexture = contentDynamic.Load<Texture2D>("Textures/Objects/Player/arrow_trans");
            bulletTexture = contentDynamic.Load<Texture2D>("Textures/Objects/bullet");
            controlstickBoundary = contentDynamic.Load<Texture2D>("Textures/Objects/Thumbstick/controlstickBoundary");
            controlstick = contentDynamic.Load<Texture2D>("Textures/Objects/Thumbstick/controlstick");
            barBlackBorder = contentDynamic.Load<Texture2D>("Textures/Objects/ScoreBar/barBlackBorder");
            barGreenTexture = contentDynamic.Load<Texture2D>("Textures/Objects/ScoreBar/barGreen");
            barYellowTexture = contentDynamic.Load<Texture2D>("Textures/Objects/ScoreBar/barYellow");
            barRedTexture = contentDynamic.Load<Texture2D>("Textures/Objects/ScoreBar/barRed");
            spaceShipTexture = contentDynamic.Load<Texture2D>("Textures/Objects/Ambient/spaceShip");
            engineOnTexture = contentDynamic.Load<Texture2D>("Textures/Objects/Ambient/Engine/engineOn");
            engineOffTexture = contentDynamic.Load<Texture2D>("Textures/Objects/Ambient/Engine/engineOff");

            // LoadAmbientTexture
            string p= "Textures/Objects/Ambient/";

            switch (ambient)
            {
                case AmbientType.Forest:
                    p += "Forest/"; 
                    grassText = contentDynamic.Load<Texture2D>(p + "forest_grass");
                    bushText = contentDynamic.Load<Texture2D>(p + "forest_bush");
                    treeText = contentDynamic.Load<Texture2D>(p + "forest_tree");
                    break;
                case AmbientType.Snow:
                    p += "Snow/";
                    grassText = contentDynamic.Load<Texture2D>(p + "snow_grass");
                    bushText = contentDynamic.Load<Texture2D>(p + "snow_bush");
                    treeText = contentDynamic.Load<Texture2D>(p + "snow_tree");
                    break;
                case AmbientType.Steppe:
                    p += "Steppe/";
                    grassText = contentDynamic.Load<Texture2D>(p + "steppe_grass");
                    bushText = contentDynamic.Load<Texture2D>(p + "steppe_bush");
                    treeText = contentDynamic.Load<Texture2D>(p + "steppe_tree");
                    break;
                case AmbientType.Desert:
                    p += "Desert/";
                    grassText = contentDynamic.Load<Texture2D>(p + "desert_grass");
                    bushText = contentDynamic.Load<Texture2D>(p + "desert_bush");
                    treeText = contentDynamic.Load<Texture2D>(p + "desert_tree");
                    break;
            }

            treeData = new Color[treeText.Width * treeText.Height];
            treeText.GetData(treeData);
        }

        #region Aim line Creation

        private Texture2D CreateAimLineTexture()
        {
            Texture2D texture;

            int aimWidth = 5;   // 3
            int aimHeight = 60; // 60

            texture = new Texture2D(ScreenManager.Game.GraphicsDevice, aimWidth, aimHeight);

            Color[] pixels = new Color[texture.Width * texture.Height];

            for (int y = 0; y < texture.Height; y++)
            {
                //float alpha = ((1f / (float)texture.Height) * (float)y); ;
                float alpha = ((1f / (float)texture.Height) * (float)(texture.Height - (float)y));
                if (y > (aimHeight / 2))
                {
                    alpha = 0f;
                }
                else
                {
                    int pos = y % 6;
                    if (pos > 3)
                        alpha = 0f;
                }

                for (int x = 0; x < texture.Width; x++)
                {
                    Color col = Color.White;
                    
                    SetPixel(texture, pixels, x, y, col * alpha);
                }

                texture.SetData<Color>(pixels);
            }

            return texture;
        }

        private void SetPixel(Texture2D texture, Color[] pixels, int x, int y, Color clr)
        {
            pixels[y * texture.Width + x] = clr;
        }

        #endregion

        private void LoadSpriteFont()
        {
            font14px = contentDynamic.Load<SpriteFont>("Fonts/font14px");
            debugFont = contentDynamic.Load<SpriteFont>("Fonts/font14px");
            noticeFont = contentDynamic.Load<SpriteFont>("Fonts/noticeFont");
            scoreFont = contentDynamic.Load<SpriteFont>("Fonts/scoreGameFont");
            portraitsFont = contentDynamic.Load<SpriteFont>("Fonts/portraitsFont");
        }

        private void InitializeThumbstick()
        {
            left_controlstickBoundaryPosition = new Vector2(34, 347);

            leftThumbstick = new ThumbstickComponent(controlstickBoundary, controlstick);
            leftThumbstick.ControlStickBoundaryPosition = left_controlstickBoundaryPosition;

            right_controlstickBoundaryPosition = new Vector2(656, 347);
            rightThumbstick = new ThumbstickComponent(controlstickBoundary, controlstick);
            rightThumbstick.ControlStickBoundaryPosition = right_controlstickBoundaryPosition;

        }

        private void InitializeTextBox()
        {
            Viewport view = ScreenManager.GraphicsDevice.Viewport;
            pilotTextBox = new TextBox(pilotPortraitsTexture, textBoxTexture, arrowBoxTexture, portraitsFont);
            pilotTextBox.Position = new Vector2(0, view.Height - pilotTextBox.Height());            
            pilotDialogs = LoadDialogs("pilot");
            pilotTextBox.Text = pilotDialogs["intro"];
            isDialogScene = true;
        }

        private Dictionary<string, string> LoadDialogs(string name)
        {
            Dictionary<string, string> dialogs = new Dictionary<string, string>();

            XDocument WorldDoc = XDocument.Load("Content/Documents/WorldDefinitions.xml");

            foreach (var level in WorldDoc.Element("Levels").Descendants("Level"))
            {
                if (int.Parse(level.Attribute("id").Value) == currentLevel)
                {                    
                    foreach (var elemDialogs in level.Elements("Dialogs"))
                    {
                        if (elemDialogs.Attribute("id").Value == name)
                        {
                            foreach (var elemDialog in elemDialogs.Elements("dialog"))
                            {
                                dialogs.Add(elemDialog.Attribute("id").Value, elemDialog.Value);
                            }
                        }
                    }    
                }
            }

            return dialogs;
        }

        private void InitializeMapComponents()
        {
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            
            // Load World definitions
            XDocument WorldDoc = XDocument.Load("Content/Documents/WorldDefinitions.xml");

            foreach (var level in WorldDoc.Element("Levels").Descendants("Level"))
            {
                if (int.Parse(level.Attribute("id").Value) == currentLevel)
                {
                    if (level.Element("worldSize") != null)
                    {
                        int x = int.Parse(level.Element("worldSize").Attribute("width").Value);
                        int y = int.Parse(level.Element("worldSize").Attribute("height").Value);

                        Vector2 worldOffset = new Vector2(100);
                        Vector2 innerWorld = new Vector2(graphicsWidth, graphicsHeight);

                        if (x >= graphicsWidth)
                        {
                            innerWorld.X = x;
                        }
                        if (y >= graphicsHeight)
                        {
                            innerWorld.Y = y;
                        }

                        world = new World(innerWorld, worldOffset);

                        if(level.Element("safeAreaRegion") != null)
                        {   
                            string str = level.Element("safeAreaRegion").Value.ToString();
                            switch(str)
                            {
                                case "Top":
                                    world.SafeRegion = SafeAreaRegion.Top;
                                    break;
                                case "TopLeft":
                                    world.SafeRegion = SafeAreaRegion.TopLeft;
                                    break;
                                case "TopRight":
                                    world.SafeRegion = SafeAreaRegion.TopRight;
                                    break;
                                case "Left":
                                    world.SafeRegion = SafeAreaRegion.Left;
                                    break;
                                case "Center":
                                    world.SafeRegion = SafeAreaRegion.Center;
                                    break;
                                case "Right":
                                    world.SafeRegion = SafeAreaRegion.Right;
                                    break;
                                case "Bottom":
                                    world.SafeRegion = SafeAreaRegion.Bottom;
                                    break;
                                case "BottomLeft":
                                    world.SafeRegion = SafeAreaRegion.BottomLeft;
                                    break;
                                case "BottomRight":
                                    world.SafeRegion = SafeAreaRegion.BottomRight;
                                    break;
                                default:
                                    world.SafeRegion = SafeAreaRegion.TopLeft;
                                    break;
                            }                            
                        }
                        else
                        {                            
                            world.SafeRegion = SafeAreaRegion.TopLeft;
                        }
                    }                    
                    
                    List<Engine> engines = new List<Engine>();

                    foreach (var engine in level.Elements("engine"))
                    {
                        ScoreBar scoreBar = new ScoreBar(barBlackBorder, barGreenTexture, barYellowTexture, barRedTexture,
                        0, 100, 10, 78, Color.White,
                        ScoreBar.ScoreBarOrientation.Horizontal, 0);

                        int x = int.Parse(engine.Attribute("X").Value);
                        int y = int.Parse(engine.Attribute("Y").Value);

                        Engine eg = new Engine(engineOnTexture, engineOffTexture, scoreBar, false);
                        eg.Position = new Vector2(world.InnerWorldBound.X, world.InnerWorldBound.Y) + new Vector2(x, y);
                        eg.LayerDepth = 0f;
                        engines.Add(eg);
                    }

                    engineManager = new EngineManager(engines);
                }
            }

            objFinder = new ObjectiveFinder(baseTexture, arrowTexture);

            List<InGameComponent> objs = new List<InGameComponent>();

            foreach (InGameComponent e in engineManager.Engines)
            {
                objs.Add(e);
            }
            AddObjectives(objs);
            isAddEngineObjectives = true;

            objButton = new SwitchComponent(objButtonOnTexture, objButtonOffTexture, false);
            objButton.Position = new Vector2(20, 30);            

            // Load Characters definitions
            XDocument charactersDoc = XDocument.Load("Content/Documents/CharactersDefinition.xml");

            foreach (XElement elemCharacter in charactersDoc.Element("Characters").Elements("Character"))
            {
                switch (elemCharacter.Attribute("id").Value)
                {
                    case "player":
                        LoadPlayerCharacter(elemCharacter);
                        break;
                    case "enemy":
                        LoadAlienCharacter(elemCharacter);
                        break;
                }

            }

            spaceShip = new SpaceShip(spaceShipTexture, world.WorldSize);
            spaceShip.TextureData = ExtractCollisionData(spaceShipTexture);
            mapComponents.Add(spaceShip);
            solidComponents.Add(spaceShip);

            InitializateWorldComponentsPosition();
            camera.Position = playerActor.Position + playerActor.CurrentFrameCenter;

            engineText = new Text("", scoreFont);
            dangerText = new Text("", scoreFont);
            scoreText = new Text("", scoreFont);
            noticeText = new Text("", noticeFont);

            GenerateAmbientComponents();

            UpdateTextComponents();
        }

        private void GenerateAmbientComponents()
        {
            Vector2 size = world.InnerWorldSize;
            int screenX = (int)size.X / graphicsWidth;
            int screenY = (int)size.Y / graphicsHeight;
            int numScreen = screenX * screenY;
            Rectangle bound = world.InnerWorldBound;

            for (int i = 0; i < screenX; i++)
            {
                for (int j = 0; j < screenY; j++)
                {
                    int num = rand.Next(4, 6);
                    for (int n = 0; n < num; n++)
                    {
                        Vector2 pos = new Vector2(
                            rand.Next(bound.X + i * graphicsWidth, bound.X + (i + 1) * graphicsWidth),
                            rand.Next(bound.Y + j * graphicsHeight, bound.Y + (j + 1) * graphicsHeight));

                        int elem = rand.Next(1,4);
                        if (elem == 1)
                        {
                            bool add = true;
                            Grass g = new Grass(grassText, world.WorldSize);
                            g.Position = pos;                            
                            foreach (Engine e in engineManager.Engines)
                            {
                                if (e.Bounds.Intersects(g.BottomCollisionArea))
                                {
                                    add = false;
                                }
                            }
                            if(add)
                                grassList.Add(g);
                        }
                        else if(elem == 2)
                        {
                            bool add = true;
                            Bush b = new Bush(bushText, world.WorldSize);
                            if (world.SafeArea.Intersects(b.BottomCollisionArea))
                            {
                                add = false;
                            }

                            foreach (Engine e in engineManager.Engines)
                            {
                                if (e.Bounds.Intersects(b.BottomCollisionArea))
                                {
                                    add = false;
                                }

                            }
                            b.Position = pos;
                            if(add)
                                bushList.Add(b);
                        }
                        else if (elem == 3)
                        {
                            bool add = true;
                            Tree t = new Tree(treeText, world.WorldSize);
                            if (world.SafeArea.Intersects(t.BottomCollisionArea))
                            {
                                add = false;
                            }

                            foreach (Engine e in engineManager.Engines)
                            {
                                if (e.Bounds.Intersects(t.BottomCollisionArea))
                                {
                                    add = false;
                                }

                            }
                            t.Position = pos;
                            t.TextureData = treeData;
                            if (add)
                            {
                                solidComponents.Add(t);
                                treeList.Add(t);
                            }
                        }
                    }
                }
            }
        }

        private void LoadPlayerCharacter(XElement elemCharacter)
        {
            AnimatingSprite idleSprite = new AnimatingSprite(), walkingSprite = new AnimatingSprite(), dyingSprite = new AnimatingSprite(), aimSprite = new AnimatingSprite();
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;            
            LoadStandardRilflemanAnimations(ref idleSprite, ref walkingSprite, ref dyingSprite, ref aimSprite, elemCharacter.Element("Animations"));
            playerActor = new Player(idleSprite, walkingSprite, dyingSprite, aimSprite, world.WorldSize, aimLine, bulletTexture);
            playerActor.ResetAnimation(false);            
        }


        private void LoadAlienCharacter(XElement elemCharacter)
        {
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;            
            AnimatingSprite idleSprite = new AnimatingSprite(), walkingSprite = new AnimatingSprite(), dyingSprite = new AnimatingSprite(), attackSprite = new AnimatingSprite();
            LoadMonsterCharacterAnimations(ref idleSprite, ref walkingSprite, ref dyingSprite, ref attackSprite, elemCharacter.Element("Animations"));
            alienManager = null;
            flockParams = new AIParameters();
            ResetAIParams();
            if (alienManager == null)
            {
                alienManager = new AlienManager(idleSprite, walkingSprite, dyingSprite, attackSprite, world, camera, flockParams, playerActor);
                alienManager.IsSpawn = false;
            }
        }        

        #endregion

        #endregion

        #region Handle Input

        public override void HandleInput(InputState input)
        {
            if (IsActive)
            {
                if (input == null)
                {
                    throw new ArgumentNullException("input");
                }

                if(!gameEnded)
                    VirtualThumbsticks.Update(input);

                if (input.IsPauseGame())
                {
                    PauseCurrentGame();
                } 
            }

            if (input.TouchState.Count > 0)
            {
                foreach (TouchLocation touch in input.TouchState)
                {
                    lastTouchPosition = touch.Position;
                }
            }

            if (input.Gestures.Count > 0)
            {
                if (gameEnded)
                {
                    if (input.Gestures[0].GestureType == GestureType.Tap)
                    {
                        userTapToExit = true;
                    }
                }
                
                if(IsActive)
                {
                    if (!gameEnded)
                    {
                        foreach (GestureSample gesture in input.Gestures)
                        {
                            if (gesture.GestureType == GestureType.Tap)
                            {
                                Point tapLocation = new Point(
                                        (int)gesture.Position.X,
                                        (int)gesture.Position.Y);

                                // Touch Bounds
                                Rectangle touchRectangle = new Rectangle((int)tapLocation.X,
                                                                        (int)tapLocation.Y,
                                                                        1, 1);                                

                                if (!isDialogScene)
                                {
                                    // Button Bounds
                                    Rectangle buttonRectangle = new Rectangle((int)objButton.Position.X, (int)objButton.Position.Y,
                                                                                objButton.Width(), objButton.Height());
                                    // If the touch is in the button
                                    if (buttonRectangle.Contains(touchRectangle))
                                    {
                                        objButton.SwitchOn = !objButton.SwitchOn;
                                    }
                                }

                                if (isDialogScene)
                                {
                                    if (!pilotTextBox.IsDialogEnded)
                                    {
                                        if (!pilotTextBox.MoveNext)
                                            pilotTextBox.MoveNext = true;
                                    }
                                    else
                                    {
                                        isDialogScene = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Update

        #region Update Text

        private void UpdateTextComponents()
        {
            // Update Text contents
            engineText.TextContents = UpdateEngineText(engineManager);
            scoreText.TextContents = UpdateScoreText(score);
            dangerText.TextContents = UpdateDangerText(alienManager);
            // Update Text position
            UpdateTextComponentsPosition();
        }


        private void UpdateTextComponentsPosition()
        {
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            engineText.Position = new Vector2(
                viewport.Width - engineText.Width(),
                0);
            scoreText.Position = new Vector2(0, 0);
            dangerText.Position = new Vector2(viewport.Width / 2 - dangerText.Font.MeasureString(LanguageDefinitions["Danger"]).X, 0); 
        }

        private String UpdateEngineText(EngineManager engineManager)
        {
            return String.Format("{0}: {1}/{2}",LanguageDefinitions["EngineScore"], engineManager.CurrentEnginesOn, engineManager.MaxEngines);
        }

        private String UpdateScoreText(int score)
        {
            return String.Format("{0} {1}",LanguageDefinitions["Score"], score);
        }

        private String UpdateDangerText(AlienManager am)
        {
            return String.Format("{0}: {1}", LanguageDefinitions["Danger"], am.DangerState);
        }

        #endregion

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (!IsActive)
            {
                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                return;
            }            

            if (moveToHighscore)
            {
                foreach (GameScreen screen in ScreenManager.GetScreens())
                    ScreenManager.RemoveScreen(screen);

                ReplaceAllScreens(
                    new List<GameScreen>()
                    {
                        new BackgroundScreen("titleScreen"),
                        new HighScoreScreen()
                    });

                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                return;
            }

            if (insertInHighscore)
            {
                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                return;
            }
            
            if (CheckIfCurrentGameFinished())
            {
                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                return;
            }
            
            if (gameEnded)
            {
                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                return;
            }            

            // Update components        
            if (engineManager.AllEnginesOn)
            {
                if (isAddEngineObjectives)
                {
                    List<InGameComponent> objs = new List<InGameComponent>();

                    foreach (InGameComponent e in engineManager.Engines)
                    {
                        objs.Add(e);
                    }
                    RemoveObjectives(objs);
                    isAddEngineObjectives = false;
                }

                if (!isAddShipObjectives)
                {
                    List<InGameComponent> objs = new List<InGameComponent>();
                    objs.Add(spaceShip);
                    AddObjectives(objs);
                    isAddShipObjectives = true;
                }
            }            
            UpdateCameraPosition();
            if (isDialogScene)
            {
                pilotTextBox.Update(gameTime);                
                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                return;
            }
            HandleThumbStick(gameTime);
            HandleCollision();

            spaceShip.Update(gameTime);
            
            engineManager.Update(gameTime);

            if (playerActor != null)
            {
                playerActor.Update(gameTime);
            }

            if (alienManager != null)
            {
                alienManager.Update(gameTime);
            }/*
            else
            {
                SpawnFlock();
            }*/

            //Update Text Components
            if (objButton.SwitchOn)
            {
                objFinder.Position =
                    playerActor.basePosition - objFinder.Center;
                /*new Vector2(
                playerActor.BottomCollisionArea.X,
                playerActor.BottomCollisionArea.Y / 2);*/
                objFinder.Update(gameTime);
            }
            UpdateTextComponents();            

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            spriteBatch.Begin();

            Color terrain;

            switch(ambient)
            {
                case AmbientType.Forest:
                    terrain = Color.ForestGreen;//new Color(150, 60, 0);
                    break;
                case AmbientType.Snow:
                    terrain = Color.ForestGreen;
                    break;
                case AmbientType.Steppe:
                    terrain = Color.SaddleBrown;//new Color(255, 233, 127);
                    break;
                case AmbientType.Desert:
                    terrain = Color.Yellow;
                    break;
                default:
                    terrain = Color.DarkOliveGreen;
                    break;
            }
            spriteBatch.Draw(blank, world.WorldBound, terrain);

            spriteBatch.End();

            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                null,
                null,
                null,
                null,
                camera.Transform());

            if (!isOutOfSafeArea)
            {
                //spriteBatch.Draw(blank, world.SafeArea, Color.AliceBlue * 0.5f);
            }

            foreach (Grass g in grassList)
            {
                g.Draw(spriteBatch, gameTime);
            }

            engineManager.Draw(spriteBatch, gameTime);

            if (playerActor != null && !isUserWon)
            {
                if (objButton.SwitchOn)
                {
                    objFinder.Draw(spriteBatch, gameTime);
                }
            }

            spriteBatch.End();

            // begin drawing with SpriteBatch using our camera transformation
            //CalcolateLayerDepth(playerActor);
            //CalcolateLayerDepth(spaceShip);

            spriteBatch.Begin(
                SpriteSortMode.FrontToBack,
                BlendState.AlphaBlend,                
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise,
                null,
                camera.Transform());
            // Draw Safe Area
            //spriteBatch.Draw(blank, world.SafeArea, Color.White);

            foreach (Bush b in bushList)
            {
                b.Draw(spriteBatch, gameTime);
            }

            foreach (Tree t in treeList)
            {
                t.Draw(spriteBatch, gameTime);
            }

            if (playerActor != null && !isUserWon)
            {
                playerActor.Draw(spriteBatch, gameTime);                
            }

            if (spaceShip != null)
            {
                spaceShip.Draw(spriteBatch, gameTime);
            }            

            if (alienManager != null)
            {
                alienManager.Draw(spriteBatch, gameTime);
            }

            foreach (Grass grass in grassList)
                grass.Draw(spriteBatch, gameTime);

            spriteBatch.End();

            spriteBatch.Begin();

            if (IsActive && !gameEnded && !isDialogScene)
            {
                leftThumbstick.Draw(spriteBatch, gameTime);
                rightThumbstick.Draw(spriteBatch, gameTime);
                objButton.Draw(spriteBatch, gameTime);
                // Draw Text Component
                engineText.Draw(spriteBatch, gameTime);
                scoreText.Draw(spriteBatch, gameTime);
                dangerText.Draw(spriteBatch, gameTime);
            }

            if (isDialogScene)
            {
                pilotTextBox.Draw(spriteBatch, gameTime);                
            }

            if (leftControlNewPosition != VirtualThumbsticks.LeftThumbstick)
            {
                leftControlOldPosition = leftControlNewPosition;
                leftControlNewPosition = VirtualThumbsticks.LeftThumbstick;
            }

            Vector2 pos = Vector2.Zero;
            if (debugVisible)
            {
                spriteBatch.DrawString(font14px, String.Format("old position left stick: {0}", leftControlOldPosition), pos, Color.White);
                pos.Y += 20;
                spriteBatch.DrawString(font14px, String.Format("left stick: {0}", leftControlNewPosition), pos, Color.White);
                pos.Y += 20;
            }

            if(!isDialogScene)
                DrawGameEndIfNecessary(spriteBatch, gameTime);            

            float left_angle = 45;
            // Convert direction to an angle
            float adjustedAngle = MathHelper.ToRadians(left_angle);
            if (debugVisible)
            {
                spriteBatch.DrawString(font14px, String.Format("raggio {0}: {1}", left_angle, adjustedAngle), pos, Color.White);
                pos.Y += 20;
                spriteBatch.DrawString(font14px, String.Format("aim: angle{0} radius{1}", MathHelper.ToDegrees(playerActor.AimLineRotation), playerActor.AimLineRotation), pos, Color.White);
                pos.Y += 20;
                float vect = (float)Math.Atan2(VirtualThumbsticks.RightThumbstick.Y, VirtualThumbsticks.RightThumbstick.X);
                spriteBatch.DrawString(font14px, String.Format("rightThumbstick: angle{0} radius{1} vector{2}", MathHelper.ToDegrees(vect), vect, VirtualThumbsticks.RightThumbstick), pos, Color.White);
                pos.Y += 20;
                Vector2 vectNorm = Vector2.Normalize(VirtualThumbsticks.RightThumbstick);
                float radiantNorm = (float)Math.Atan2(vectNorm.Y, vectNorm.X);
                spriteBatch.DrawString(font14px, String.Format("normailzed: \nangle{0} \nradius{1} \nvector{2}", MathHelper.ToDegrees(radiantNorm), radiantNorm, vectNorm), pos, Color.White);
                pos.Y += 20 * 5;
                spriteBatch.DrawString(font14px, String.Format("aimAngle: {0}", playerActor.AimAngle), pos, Color.White);
                pos.Y += 20;
                spriteBatch.DrawString(font14px, String.Format("PlayerPosition: {0}", playerActor.Position), pos, Color.White);
                pos.Y += 20;
                spriteBatch.DrawString(font14px, String.Format("PlayerLayer: {0}", playerActor.LayerDepth), pos, Color.White);
                pos.Y += 20;
                spriteBatch.DrawString(font14px, String.Format("ShipLayer: {0}", spaceShip.LayerDepth), pos, Color.White);
                pos.Y += 20;                
                spriteBatch.DrawString(font14px, String.Format("Viewport: {0}", viewport), pos, Color.White);
                pos.Y += 20;                
            }
            //spriteBatch.DrawString(font14px, String.Format("enemy collision: {0}", enemy.hasCollisionWithPlayer), pos, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        #endregion

        #region Characters Animation Methods

        private void LoadStandardCharacterAnimations(ref AnimatingSprite idleSprite, ref AnimatingSprite walkingSprite, ref AnimatingSprite dyingSprite, XElement elemAnimations)
        {
            string characterType = "standard";
            foreach (XElement elemAnimation in elemAnimations.Elements("Animation"))
            {
                if (String.Compare(elemAnimation.Attribute("character").Value, characterType) == 0)
                {
                    string state = elemAnimation.Attribute("state").Value;
                    string textureName = elemAnimation.Attribute("path").Value;
                    Texture2D animationTexture = contentDynamic.Load<Texture2D>(textureName);
                    Point frameDimensions = new Point(int.Parse(elemAnimation.Attribute("width").Value), int.Parse(elemAnimation.Attribute("height").Value));
                    int framesPerRow = int.Parse(elemAnimation.Attribute("framesPerRow").Value);
                    int interval = int.Parse(elemAnimation.Attribute("interval").Value);

                    AnimatingSprite animatingSprite = new AnimatingSprite();

                    //animatingSprite.currentAnimation = currentAnimation;
                    //animatingSprite.currentFrame = currentFrame;
                    //animatingSprite.elapsedTime = elapsedTime;
                    animatingSprite.FrameDimensions = frameDimensions;
                    //animatingSprite.frameOrigin = frameOrigin;
                    animatingSprite.FramesPerRow = framesPerRow;
                    animatingSprite.SourceOffset = Vector2.Zero;
                    //animatingSprite.sourceRectangle = sourceRectangle;
                    animatingSprite.Texture = animationTexture;
                    animatingSprite.TextureName = textureName;

                    switch (state)
                    {
                        case "idle":
                            animatingSprite.Animations.AddRange(StandardCharacterIdleAnimations(framesPerRow, interval));
                            idleSprite = animatingSprite;
                            break;
                        case "walk":
                            animatingSprite.Animations.AddRange(StandardCharacterWalkingAnimations(framesPerRow, interval));
                            walkingSprite = animatingSprite;
                            break;
                        case "die":
                            animatingSprite.Animations.AddRange(StandardCharacterDieAnimations(framesPerRow, interval));
                            dyingSprite = animatingSprite;
                            break;
                    }

                }
                else
                {
                    continue;
                }
            }
        }

        private void LoadStandardRilflemanAnimations(
            ref AnimatingSprite idleSprite, ref AnimatingSprite walkingSprite,
            ref AnimatingSprite dyingSprite, ref AnimatingSprite aimSprite,
            XElement elemAnimations)
        {
            LoadStandardCharacterAnimations(ref idleSprite, ref walkingSprite, ref dyingSprite, elemAnimations);

            string characterType = "rifleman";
            foreach (XElement elemAnimation in elemAnimations.Elements("Animation"))
            {
                if (String.Compare(elemAnimation.Attribute("character").Value, characterType) == 0)
                {
                    string type = elemAnimation.Attribute("state").Value;
                    string textureName = elemAnimation.Attribute("path").Value;
                    Texture2D animationTexture = contentDynamic.Load<Texture2D>(textureName);
                    Point frameDimensions = new Point(int.Parse(elemAnimation.Attribute("width").Value), int.Parse(elemAnimation.Attribute("height").Value));
                    int framesPerRow = int.Parse(elemAnimation.Attribute("framesPerRow").Value);
                    int interval = int.Parse(elemAnimation.Attribute("interval").Value);

                    AnimatingSprite animatingSprite = new AnimatingSprite();

                    //animatingSprite.currentAnimation = currentAnimation;
                    //animatingSprite.currentFrame = currentFrame;
                    //animatingSprite.elapsedTime = elapsedTime;
                    animatingSprite.FrameDimensions = frameDimensions;
                    //animatingSprite.frameOrigin = frameOrigin;
                    animatingSprite.FramesPerRow = framesPerRow;
                    animatingSprite.SourceOffset = Vector2.Zero;
                    //animatingSprite.sourceRectangle = sourceRectangle;
                    animatingSprite.Texture = animationTexture;
                    animatingSprite.TextureName = textureName;

                    switch (type)
                    {
                        case "aim":
                            animatingSprite.Animations.AddRange(RiflemanCharacterAimAnimations(1, interval));
                            aimSprite = animatingSprite;
                            break;
                    }

                }
                else
                {
                    continue;
                }
            }
        }

        private void LoadMonsterCharacterAnimations(
            ref AnimatingSprite idleSprite, ref AnimatingSprite walkingSprite,
            ref AnimatingSprite dyingSprite, ref AnimatingSprite attackSprite,
            XElement elemAnimations)
        {
            LoadStandardCharacterAnimations(ref idleSprite, ref walkingSprite, ref dyingSprite, elemAnimations);

            string characterType = "monster";
            foreach (XElement elemAnimation in elemAnimations.Elements("Animation"))
            {
                if (String.Compare(elemAnimation.Attribute("character").Value, characterType) == 0)
                {
                    string type = elemAnimation.Attribute("state").Value;
                    string textureName = elemAnimation.Attribute("path").Value;
                    Texture2D animationTexture = contentDynamic.Load<Texture2D>(textureName);
                    Point frameDimensions = new Point(int.Parse(elemAnimation.Attribute("width").Value), int.Parse(elemAnimation.Attribute("height").Value));
                    int framesPerRow = int.Parse(elemAnimation.Attribute("framesPerRow").Value);
                    int interval = int.Parse(elemAnimation.Attribute("interval").Value);

                    AnimatingSprite animatingSprite = new AnimatingSprite();

                    //animatingSprite.currentAnimation = currentAnimation;
                    //animatingSprite.currentFrame = currentFrame;
                    //animatingSprite.elapsedTime = elapsedTime;
                    animatingSprite.FrameDimensions = frameDimensions;
                    //animatingSprite.frameOrigin = frameOrigin;
                    animatingSprite.FramesPerRow = framesPerRow;
                    animatingSprite.SourceOffset = Vector2.Zero;
                    //animatingSprite.sourceRectangle = sourceRectangle;
                    animatingSprite.Texture = animationTexture;
                    animatingSprite.TextureName = textureName;

                    switch (type)
                    {
                        case "attack":
                            animatingSprite.Animations.AddRange(MonsterCharacterAttackAnimations(framesPerRow, interval));
                            attackSprite = animatingSprite;
                            break;
                    }

                }
                else
                {
                    continue;
                }
            }
        }

        private List<Animation> StandardCharacterWalkingAnimations(int frameRange, int interval)
        {
            string[] animationsName = 
            {
                "WalkSouth",
                "WalkSouthwest",
                "WalkWest",
                "WalkNorthwest",
                "WalkNorth",
                "WalkNortheast",
                "WalkEast",
                "WalkSoutheast"
            };

            List<Animation> animations = new List<Animation>();
            int index = 0;
            for (int i = 0; i < animationsName.Length; i++)
            {
                animations.Add(new Animation(animationsName[i], index + 1, index + frameRange,
                    interval, true));
                index += frameRange;
            }

            return animations;
        }

        private List<Animation> StandardCharacterIdleAnimations(int frameRange, int interval)
        {
            string[] animationsName = 
            {
                "IdleSouth",
                "IdleSouthwest",
                "IdleWest",
                "IdleNorthwest",
                "IdleNorth",
                "IdleNortheast",
                "IdleEast",
                "IdleSoutheast"
            };

            List<Animation> animations = new List<Animation>();
            int index = 0;
            for (int i = 0; i < animationsName.Length; i++)
            {
                animations.Add(new Animation(animationsName[i], index + 1, index + frameRange,
                    interval, true));
                index += frameRange;
            }

            return animations;
        }

        private List<Animation> StandardCharacterDieAnimations(int frameRange, int interval)
        {
            string[] animationsName = 
            {
                "DieSouth",
                "DieSouthwest",
                "DieWest",
                "DieNorthwest",
                "DieNorth",
                "DieNortheast",
                "DieEast",
                "DieSoutheast"
            };

            List<Animation> animations = new List<Animation>();
            int index = 0;
            for (int i = 0; i < animationsName.Length; i++)
            {
                animations.Add(new Animation(animationsName[i], index + 1, index + frameRange,
                    interval, false));
                index += frameRange;
            }

            return animations;
        }

        private List<Animation> RiflemanCharacterAimAnimations(int frameRange, int interval)
        {
            string[] cardinalNames =
            {
                "AimSouth",
                "AimSouthSouthwest",
                "AimSouthWest",
                "AimWestSouthwest", 
                "AimWest",
                "AimWestNorthwest",
                "AimNorthWest",
                "AimNorthNorthwest",
                "AimNorth",
            };

            List<Animation> animations = new List<Animation>();
            int index = 0;
            for (int i = 0; i < cardinalNames.Length; i++)
            {
                animations.Add(new Animation(cardinalNames[i], index + 1, index + frameRange,
                    interval, true));
                index += frameRange;
            }

            return animations;
        }

        private List<Animation> MonsterCharacterAttackAnimations(int frameRange, int interval)
        {
            string[] animationsName = 
            {
                "AttackSouth",
                "AttackSouthwest",
                "AttackWest",
                "AttackNorthwest",
                "AttackNorth",
                "AttackNortheast",
                "AttackEast",
                "AttackSoutheast"
            };

            List<Animation> animations = new List<Animation>();
            int index = 0;
            for (int i = 0; i < animationsName.Length; i++)
            {
                animations.Add(new Animation(animationsName[i], index + 1, index + frameRange,
                    interval, false));
                index += frameRange;
            }

            return animations;
        }

        #endregion

        #region Private Methods

        private void CalcolateLayerDepth(InGameComponent component)
        {
            Rectangle rect = component.LayerDepthRectangle;
            float layer = (rect.Y + rect.Height) /  world.WorldSize.Y;
            component.LayerDepth = MathHelper.Clamp(layer, 0f, 1f);            
        }

        private float CalcolateLayerDepth(Vector2 position, Texture2D text)
        {            
            float layer = (position.Y + (float)text.Height) / world.WorldSize.Y;
            layer =  (float)MathHelper.Clamp(layer, 0f, 1f);
            return layer;
        }

        #region Handle Game State

        private bool CheckIfCurrentGameFinished()
        {
            if (isUserWon || playerActor.State == StandardCharacter.CharacterState.Dead)
            {
                gameEnded = true;
            }

            // if gameEnded, game is over
            if (gameEnded)
            {
                if(userTapToExit)
                {
                    if (!insertInHighscore)
                    {
                        if (CheckIsInHighScore() && isUserWon)
                        {
                            if (!Guide.IsVisible)
                            {
                                Guide.BeginShowKeyboardInput(PlayerIndex.One,
                                    "Player Name", "Insert your name (max 15 characters)!", "Player",
                                    AfterPlayerEnterName, isUserWon);
                                insertInHighscore = true;
                            }
                        }
                        else
                        {
                            insertInHighscore = true;
                            MoveToNextScreen(isUserWon);
                        }
                    }                    
                }
            }

            return false;
        }

        private void MoveToNextScreen(bool isWon)
        {
            PlaySounds(false);

            if (isWon)
            {
                if (currentLevel < maxLevel - 1)
                {
                    currentLevel++;

                    MemoryStateManager.SaveCurrentGameState(currentLevel);

                    GameplayScreen gmpScreen = new GameplayScreen();
                    gmpScreen.currentLevel = this.currentLevel;

                    string strInstruction = "instructionBackground_" + SettingsManager.Language;

                    ReplaceAllScreens(
                    new List<GameScreen>() 
                    { 
                        new BackgroundScreen(strInstruction),
                        new LoadingScreen(gmpScreen)
                    });
                }
                else
                {
                    if (PhoneApplicationService.Current.State.ContainsKey("CurrentLevel"))
                        PhoneApplicationService.Current.State.Remove("CurrentLevel");

                    ReplaceAllScreens(
                    new List<GameScreen>() 
                    { 
                        new BackgroundScreen("titleScreen"),
                        new MainMenuScreen(),
                    });
                }
            }
            else
            {
                ReplaceAllScreens(
                    new List<GameScreen>() 
                    { 
                        new BackgroundScreen("titleScreen"),
                        new PauseScreen()
                    });
            }
        }


        private bool CheckIsInHighScore()
        {
            return HighScoreScreen.IsInHighscores(score, currentLevel);
        }

        private void AfterPlayerEnterName(IAsyncResult result)
        {
            // Get the name entered by the user
            string playerName = Guide.EndShowKeyboardInput(result);

            if (!string.IsNullOrEmpty(playerName))
            {
                // Ensure that it is valid
                if (playerName != null && playerName.Length > 15)
                {
                    playerName = playerName.Substring(0, 15);
                }

                // Puts it in high score
                HighScoreScreen.PutHighScore(playerName, score, currentLevel);
            }

            // Moves to the next screen
            MoveToNextScreen((bool)result.AsyncState);
        }

        private void DrawGameEndIfNecessary(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (gameEnded && !userTapToExit)
            {
                StringBuilder stringToDisplay = new StringBuilder();

                if (isUserWon)
                {
                    stringToDisplay.Append(LanguageDefinitions["UserWon"]);
                    /*
                    if (CheckIsInHighScore())
                    {
                        stringToDisplay.Append(LanguageDefinitions["NewScore"]);
                    }*/
                }
                else
                {
                    stringToDisplay.Append(LanguageDefinitions["UserLost"]); 
                }

                noticeText.TextContents = stringToDisplay.ToString();

                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

                noticeText.Position = new Vector2(
                    (viewport.Width - noticeText.Width())/2,
                    (viewport.Height - noticeText.Height())/2);
                noticeText.Color = Color.Red;
                noticeText.Draw(spriteBatch, gameTime);
            }
        }

        /// <summary>
        /// Pause the game.
        /// </summary>
        private void PauseCurrentGame()
        {
            IsActive = false;

            // Pause sounds
            PauseResumeSounds(false);
            
            ScreenManager.AddScreen(new BackgroundScreen("titleScreen"));
            ScreenManager.AddScreen(new PauseScreen());
            
        }

        #endregion

        #region Handle Thumbstick Methods

        private void HandleThumbStick(GameTime gameTime)
        {
            HandleLeftThumbStick(gameTime);
            HandleRightThumbStick(gameTime);
        }

        private void HandleLeftThumbStick(GameTime gameTime)
        {
            // Calculate the rectangle of the outer circle of the thumbstick
            Rectangle boundaryDimension = leftThumbstick.ControlStickBoundaryDimension;
            Rectangle outerControlstick = new Rectangle(0, (int)boundaryDimension.Y - 35,
                boundaryDimension.Width + 60, boundaryDimension.Height + 60);

            // Reset the thumbstick position when it is idle
            if (VirtualThumbsticks.LeftThumbstick == Vector2.Zero)
            {
                leftThumbstick.ResetControlStickPosition();
                playerActor.Move(Vector2.Zero, false, gameTime);
            }
            else
            {
                // If not in motion and the touch point is not in the control bounds - there is no movement
                Rectangle touchRectangle = new Rectangle((int)lastTouchPosition.X, (int)lastTouchPosition.Y, 1, 1);

                if (!outerControlstick.Contains(touchRectangle))
                {
                    leftThumbstick.ResetControlStickPosition();
                    playerActor.Move(Vector2.Zero, false, gameTime);
                    return;
                }

                // Moves the thumbstick's inner circle
                float radious = leftThumbstick.ControlStickDimension.Width / 2 + 35;
                leftThumbstick.ControlStickPosition = leftThumbstick.ControlStickStartedPosition + (VirtualThumbsticks.LeftThumbstick * radious);
                // Move the player
                SetMotion(gameTime);
            }
        }

        private void SetMotion(GameTime gameTime)
        {
            // Calculate the beekeeper's location
            Vector2 leftThumbstickVector = Vector2.Zero;
            leftThumbstickVector = VirtualThumbsticks.LeftThumbstick;
            if (leftThumbstickVector == Vector2.Zero)
            {               
                playerActor.Move(Vector2.Zero, false, gameTime);
            }
            else
            {
                if (!playerActor.IsDeadOrDying)
                {
                    Vector2 nextPlayerPosition =
                        new Vector2(
                            playerActor.CentralCollisionArea.X,
                            playerActor.CentralCollisionArea.Y) +
                            playerActor.CalcolateDirection(leftThumbstickVector);

                    if (!CheckPlayerSolidComponentCollision(nextPlayerPosition) /*&& CheckPlayerInnerWorldBoundsCollision(playerActor.CalcolateDirection(leftThumbstickVector))*/)
                    {                      
                        playerActor.Move(leftThumbstickVector, true, gameTime);                        
                    }
                    else
                    {
                        playerActor.Move(Vector2.Zero, false, gameTime);
                        
                    }
                }
                else
                {                   
                    playerActor.Move(Vector2.Zero, false, gameTime);
                }
                    /*
                    if (!CheckPlayerCollision(playerCalculatedPosition, enemy))
                    {
                        player.Move(leftThumbstickVector, true);
                    }
                    else
                    {
                        Vector2 repulsion = enemy.Velocity * enemy.Speed;
                        Vector2 movement = -(leftThumbstickVector * player.Speed - repulsion);
                        player.Move(movement, false);
                        break;
                    }
                     * */
                
            }
        }

        private void HandleRightThumbStick(GameTime gameTime)
        {
            Rectangle boundaryDimension = leftThumbstick.ControlStickBoundaryDimension;
            // Calculate the rectangle of the outer circle of the thumbstick
            Rectangle outerControlstick = new Rectangle(622, (int)boundaryDimension.Y - 35,
                boundaryDimension.Width + 60, boundaryDimension.Height + 60);
            // Reset the thumbstick position when it is idle
            if (VirtualThumbsticks.RightThumbstick == Vector2.Zero)
            {
                if (playerActor.IsRotating && !playerActor.IsDeadOrDying)
                    playerActor.GenerateBullet();
                playerActor.IsRotating = false;
                playerActor.AimLineVelocity = -Vector2.UnitY;
                rightThumbstick.ResetControlStickPosition();
            }
            else
            {
                // If not in motion and the touch point is not in the control bounds - there is no movement
                Rectangle touchRectangle = new Rectangle((int)lastTouchPosition.X, (int)lastTouchPosition.Y, 1, 1);

                if (!outerControlstick.Contains(touchRectangle))
                {
                    if (playerActor.IsRotating && !playerActor.IsDeadOrDying)
                        playerActor.GenerateBullet();
                    playerActor.IsRotating = false;
                    playerActor.AimLineVelocity = -Vector2.UnitY;
                    rightThumbstick.ResetControlStickPosition();
                    return;
                }

                // Rotate the player
                SetRotation(gameTime);

                // Moves the thumbstick's inner circle
                float radious = controlstick.Width / 2 + 35;
                rightThumbstick.ControlStickPosition = rightThumbstick.ControlStickStartedPosition + (VirtualThumbsticks.RightThumbstick * radious);
            }
        }

        private void SetRotation(GameTime gameTime)
        {
            Vector2 rightThumbstickVector = Vector2.Zero;
            rightThumbstickVector = VirtualThumbsticks.RightThumbstick;

            if (!(rightThumbstickVector.Length() > .1f) || playerActor.IsDeadOrDying)
            {
                playerActor.IsRotating = false;
            }
            else
            {
                //float radians = (float)Math.Atan2(rightThumbstickVector.Y, rightThumbstickVector.X) + MathHelper.PiOver2;
                //float angle = MathHelper.ToDegrees(right_radians);
                // update our ship's rotation based on the right thumbstick
                float addedAngle = MathHelper.PiOver2;
                playerActor.IsRotating = true;
                playerActor.AimLineVelocity = Vector2.Normalize(rightThumbstickVector);
                playerActor.SetRotation(rightThumbstickVector);
            }
        }
        #endregion

        #region Handle Collision Methods

        private void HandleCollision()
        {
            HandlePlayerOutOfSafeAreaCollsion();
            HandlePlayerInnerWorldCollision();
            HandleEngineCollision();
            CheckPlayerBulletCollision();
        }

        private void HandleEngineCollision()
        {
            if(!playerActor.IsDeadOrDying)
                engineManager.Intersect(playerActor.BottomCollisionArea);
        }

        private bool CheckPlayerSolidComponentCollision(Vector2 nextPlayerPosition)
        {
            Rectangle TempCollisionArea = new Rectangle(
                (int)nextPlayerPosition.X,
                (int)nextPlayerPosition.Y,
                playerActor.CentralCollisionArea.Width,
                playerActor.CentralCollisionArea.Height);

            foreach (InanimateGameComponent component in solidComponents)
            {
                if (TempCollisionArea.Intersects(component.CentralCollisionArea))
                {
                    if (engineManager.AllEnginesOn)
                    {
                        if (component is SpaceShip && !playerActor.IsDeadOrDying)
                            isUserWon = true;
                    }
                    return true;
                }
            }

            return false;
        }

        public bool CheckPlayerInnerWorldBoundsCollision(Vector2 moveAmount)
        {
            Rectangle innerSize = world.InnerWorldBound;
            Vector2 hitPos = playerActor.HitPosition + moveAmount;


            if (hitPos.X < innerSize.X ||
                hitPos.Y < innerSize.Y ||
                hitPos.X > innerSize.X + innerSize.Width ||
                hitPos.Y > innerSize.Y + innerSize.Height)
            {
                return true;
            }
            else
                return false;

        }

        public void HandlePlayerOutOfSafeAreaCollsion()
        {
            if (!isOutOfSafeArea)
            {
                Rectangle rect = world.SafeArea;
                if (!rect.Intersects(playerActor.BottomCollisionArea))
                {
                    isOutOfSafeArea = true;
                    alienManager.IsSpawn = true;
                }
            }
        }

        public void HandlePlayerInnerWorldCollision()
        {
            Rectangle innerSize = world.InnerWorldBound;
            Vector2 centralPos = playerActor.HitPosition;
            if (centralPos.X < innerSize.X)
            {
                centralPos.X = innerSize.X; 
            }
            if (centralPos.Y < innerSize.Y)
            {
                centralPos.Y = innerSize.Y; 
            }
            if (centralPos.X > innerSize.X + innerSize.Width)
            {
                centralPos.X = innerSize.X + innerSize.Width;
            }
            if (centralPos.Y > innerSize.Y + innerSize.Height)
            {
                centralPos.Y = innerSize.Y + innerSize.Height;
            }

            playerActor.HitPosition = centralPos;
        }

        private void CheckPlayerBulletCollision()
        {
            List<Bullet> bulletsToRemove = new List<Bullet>();

            foreach (var b in playerActor.BulletsMang.Bullets)
            {
                foreach (var a in alienManager.Aliens)
                {
                    if (!a.IsHit)
                    {
                        if (b.BoundsTransform.Intersects(a.BodyBounds))
                        {                            
                            // Check collision with person
                            if (IntersectPixels(b.MatrixTransform, b.Width(),
                                                b.Height(), b.TextureData,
                                                a.MatrixTransform, a.Width(),
                                                a.Height(), a.CurrentAnimatingSprite.currentSpriteTextureData))
                            {
                                // Set alien IsHit variable True
                                a.HandleHitState();
                                bulletsToRemove.Add(b);
                                score += alienManager.Score;
                                break;
                            }
                        }
                    }
                }

                foreach (var s in solidComponents)
                {
                    if(b.BoundsTransform.Intersects(s.CentralCollisionArea))
                    {
                        // Check collision with person
                        if (IntersectPixels(b.MatrixTransform, b.Width(),
                                            b.Height(), b.TextureData,
                                            s.MatrixTransform, s.Width(),
                                            s.Height(), s.TextureData))
                        {
                            b.PlayBulletCollision();
                            bulletsToRemove.Add(b);
                            break;
                        }
                    }
                }

                // Remove bullets out of camera view
                Rectangle camRect = new Rectangle((int)camera.VisibleArea.X, (int)camera.VisibleArea.Y, (int)camera.VisibleArea.Width, (int)camera.VisibleArea.Height);
                if (!b.BoundsTransform.Intersects(camRect))
                {
                    bulletsToRemove.Add(b);                    
                }
            }

            // remove all marked bullets
            foreach (var b in bulletsToRemove)
                playerActor.BulletsMang.Bullets.Remove(b);
        }        
        
        #endregion

        #endregion

        #region Public Methods

        public bool CheckPlayerCollision(Vector2 playerPosition, Alien enemy)
        {
            
            Rectangle TempCollisionArea = new Rectangle(
                (int)playerPosition.X,
                (int)playerPosition.Y,
                playerActor.CentralCollisionArea.Width,
                playerActor.CentralCollisionArea.Height);

            return  TempCollisionArea.Intersects(enemy.CentralCollisionArea);
            
        }        

        public void PauseResumeSounds(bool pause)
        {
            AudioManager.PauseResumeSounds(pause);
            if (pause)
            {
                AudioManager.PlayMusic("gameplayMusic");
            }
            else
            {
                AudioManager.StopMusic(); 
            }
        }

        public void PlaySounds(bool play)
        {
            if (play)
            {
                AudioManager.PlayMusic(gameMusic);
            }
            else
            {
                AudioManager.StopMusic();
                AudioManager.StopSounds();
            }
        }

        #endregion

        #region Flock Methods        

        private void ResetAIParams()
        {
            flockParams.DetectionDistance = detectionDefault;
            flockParams.SeparationDistance = separationDefault;
            flockParams.MoveInOldDirectionInfluence = moveInOldDirInfluenceDefault;
            flockParams.MoveInFlockDirectionInfluence = moveInFlockDirInfluenceDefault;
            flockParams.MoveInRandomDirectionInfluence = moveInRandomDirInfluenceDefault;
            flockParams.MaxTurnRadians = maxTurnRadiansDefault;
            flockParams.PerMemberWeight = perMemberWeightDefault;
            flockParams.PerDangerWeight = perDangerWeightDefault;
            flockParams.PerPreyWeight = perPreyWeightDefault;
        }

        #endregion

        #region Transformed Collision

        // <summary>
        /// Determines if there is overlap of the non-transparent pixels
        /// between two sprites.
        /// </summary>
        /// <param name="rectangleA">Bounding rectangle of the first sprite</param>
        /// <param name="dataA">Pixel data of the first sprite</param>
        /// <param name="rectangleB">Bouding rectangle of the second sprite</param>
        /// <param name="dataB">Pixel data of the second sprite</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static bool IntersectPixels(Rectangle rectangleA, Color[] dataA,
                                           Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }


        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels between two
        /// sprites.
        /// </summary>
        /// <param name="transformA">World transform of the first sprite.</param>
        /// <param name="widthA">Width of the first sprite's texture.</param>
        /// <param name="heightA">Height of the first sprite's texture.</param>
        /// <param name="dataA">Pixel color data of the first sprite.</param>
        /// <param name="transformB">World transform of the second sprite.</param>
        /// <param name="widthB">Width of the second sprite's texture.</param>
        /// <param name="heightB">Height of the second sprite's texture.</param>
        /// <param name="dataB">Pixel color data of the second sprite.</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static bool IntersectPixels(
                            Matrix transformA, int widthA, int heightA, Color[] dataA,
                            Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }

        #endregion

        protected virtual Color[] ExtractCollisionData(Texture2D texture)
        {
            // Extract collision data
            Color[] newTextureData =
                new Color[texture.Width * texture.Height];
            texture.GetData(newTextureData);

            return newTextureData;

        }
      
        protected void UpdateCameraPosition()
        {
            float cameraBoundWidth = camera.ViewingWidth / 4;
            float cameraBoundHeight = camera.ViewingHeight / 4;

            Vector2 playerCentralPos = playerActor.Position + playerActor.CurrentFrameCenter;
            Vector2 moveAmount = playerActor.MoveAmount;
            Vector2 newPos = camera.Position + moveAmount;
            Vector2 newVisPos = camera.ViewingPosition + moveAmount;
            Rectangle innerSize = world.InnerWorldBound;
            // mantieni la telecamera entro i bordi dello schermo
            if (newVisPos.X < innerSize.X)
            {
                camera.ViewingPositionX = innerSize.X;
            }
            if (newVisPos.Y < innerSize.Y)
            {
                camera.ViewingPositionY = innerSize.Y;
            }

            if (newVisPos.X + camera.ViewingWidth > innerSize.X + innerSize.Width)
            {
                camera.ViewingPositionX = innerSize.X + innerSize.Width - camera.ViewingWidth;
            }
            if (newVisPos.Y + camera.ViewingHeight > innerSize.Y + innerSize.Height)
            {
                camera.ViewingPositionY = innerSize.Y + innerSize.Height - camera.ViewingHeight;
            }


            // Muovi la telecamera
            if (playerCentralPos.X > camera.Position.X + cameraBoundWidth &&
                moveAmount.X > 0)
            {
                if (!(newVisPos.X + camera.ViewingWidth > innerSize.X + innerSize.Width))
                    camera.Position = new Vector2(
                        camera.Position.X + moveAmount.X,
                        camera.Position.Y);
            }
            if (playerCentralPos.Y > camera.Position.Y + cameraBoundHeight &&
                moveAmount.Y > 0)
            {
                if (!(newVisPos.Y + camera.ViewingHeight > innerSize.Y + innerSize.Height))
                    camera.Position = new Vector2(
                        camera.Position.X,
                        camera.Position.Y + moveAmount.Y);
            }

            if (playerCentralPos.X < camera.Position.X - cameraBoundWidth &&
                moveAmount.X < 0)
            {
                if (!(newVisPos.X < innerSize.X))
                    camera.Position = new Vector2(
                        camera.Position.X + moveAmount.X,
                        camera.Position.Y);
            }
            if (playerCentralPos.Y < camera.Position.Y - cameraBoundHeight &&
                moveAmount.Y < 0)
            {
                if (!(newVisPos.Y < innerSize.Y))
                    camera.Position = new Vector2(
                        camera.Position.X,
                        camera.Position.Y + moveAmount.Y);
            }
        }

        protected void InitializateWorldComponentsPosition()
        {
            Vector2 playerPos = Vector2.Zero;
            Vector2 shipPos = Vector2.Zero;
            Rectangle rect = Rectangle.Empty;
            /*
            Vector2 middleArea = new Vector2(safeArea.X + safeArea.Width / 2, safeArea.Y + safeArea.Height / 2);
            spaceShip.Position = middleArea - new Vector2(spaceShip.Width() / 2, spaceShip.Height() / 2);
            playerActor.Position = spaceShip.Position + new Vector2(spaceShip.Width(), spaceShip.Height() / 2) + new Vector2(10, -playerActor.Height() / 2);
            */

            // Inizializzazione direzion componenti area sicura
            HelpMethodsSafeAreaComponentsDirection(world.SafeRegion);

            // Inizializzazione dimensioni area sicura            
            HelpMethodSafeArea(ref rect, ref playerPos, ref shipPos, world.SafeRegion);

            world.SafeArea = rect;
            playerActor.Position = playerPos;
            spaceShip.Position = shipPos;
        }

        protected void HelpMethodsSafeAreaComponentsDirection(SafeAreaRegion safeRegion)
        {
            switch (safeRegion)
            {
                case SafeAreaRegion.Top:
                    playerActor.CardDirection = CardinalDirection.South;
                    playerActor.ResetAnimation(false);
                    spaceShip.Effects = SpriteEffects.None;
                    break;
                case SafeAreaRegion.Bottom:
                    playerActor.CardDirection = CardinalDirection.North;
                    playerActor.ResetAnimation(false);
                    spaceShip.Effects = SpriteEffects.None;
                    break;
                case SafeAreaRegion.Left:
                    playerActor.CardDirection = CardinalDirection.East;
                    playerActor.ResetAnimation(false);
                    spaceShip.Effects = SpriteEffects.FlipHorizontally;
                    break;
                case SafeAreaRegion.Right:
                    playerActor.CardDirection = CardinalDirection.West;
                    playerActor.ResetAnimation(false);
                    spaceShip.Effects = SpriteEffects.None;
                    break;
                case SafeAreaRegion.TopLeft:
                    playerActor.CardDirection = CardinalDirection.SouthEast;
                    playerActor.ResetAnimation(false);
                    spaceShip.Effects = SpriteEffects.FlipHorizontally;
                    break;
                case SafeAreaRegion.TopRight:
                    playerActor.CardDirection = CardinalDirection.SouthWest;
                    playerActor.ResetAnimation(false);
                    spaceShip.Effects = SpriteEffects.None;
                    break;
                case SafeAreaRegion.BottomLeft:
                    playerActor.CardDirection = CardinalDirection.NorthEast;
                    playerActor.ResetAnimation(false);
                    spaceShip.Effects = SpriteEffects.FlipHorizontally;
                    break;
                case SafeAreaRegion.BottomRight:
                    playerActor.CardDirection = CardinalDirection.NorthWest;
                    playerActor.ResetAnimation(false);
                    spaceShip.Effects = SpriteEffects.None;
                    break;
            }
        }

        protected void HelpMethodSafeArea(ref Rectangle rect, ref Vector2 pPos, ref Vector2 sPos, SafeAreaRegion safeRegion )
        {
            int emptySpace = 30;
            int rectWidth = playerActor.Width() + spaceShip.Width() + emptySpace * 3;
            int rectHeight = playerActor.Height() + spaceShip.Height() + emptySpace * 3;
            rect = new Rectangle(0, 0, rectWidth, rectHeight);

            Vector2 innerSize = world.InnerWorldSize;
            Vector2 worldOffset = world.WorldOffset;
            Vector2 halfRectPos; 
            Rectangle innerRect = world.InnerWorldBound;

            // Impostare la posizione del rettangolo su Y
            if (safeRegion == SafeAreaRegion.Top ||
                safeRegion == SafeAreaRegion.TopLeft ||
                safeRegion == SafeAreaRegion.TopRight)
            {                
                rect.Y = innerRect.Y;
            }
            if (safeRegion == SafeAreaRegion.Left ||
                safeRegion == SafeAreaRegion.Right ||
                safeRegion == SafeAreaRegion.Center)
            {
                rect.Y = innerRect.Y + (innerRect.Width - rect.Height) / 2;
            }
            if (safeRegion == SafeAreaRegion.Bottom ||
                safeRegion == SafeAreaRegion.BottomLeft ||
                safeRegion == SafeAreaRegion.BottomRight)
            {
                rect.Y = innerRect.Y + innerRect.Width - rect.Height;
            }


            if (safeRegion == SafeAreaRegion.Top ||
                safeRegion == SafeAreaRegion.Bottom ||
                safeRegion == SafeAreaRegion.Center)
            {
                rect.X = innerRect.X + (innerRect.Width - rect.Width) / 2;
            }
            if (safeRegion == SafeAreaRegion.Left ||
                safeRegion == SafeAreaRegion.TopLeft ||
                safeRegion == SafeAreaRegion.BottomLeft)
            {
                rect.X = innerRect.X;
            }
            if (safeRegion == SafeAreaRegion.Right ||
                safeRegion == SafeAreaRegion.TopRight ||
                safeRegion == SafeAreaRegion.BottomRight)
            {
                rect.X = innerRect.X + innerRect.Width;
            }

            halfRectPos =
                new Vector2(rect.X, rect.Y) +
                new Vector2(rect.Width / 2, rect.Height / 2);
            if (safeRegion == SafeAreaRegion.Top ||
                safeRegion == SafeAreaRegion.Center)
            {
                sPos.X =
                    halfRectPos.X -
                    spaceShip.Width() / 2;
                sPos.Y =
                    rect.Y + emptySpace;

                pPos.X =
                    halfRectPos.X -
                    playerActor.CurrentFrameDimension.X / 2;
                pPos.Y =
                    sPos.Y +
                    spaceShip.Height() +
                    emptySpace;
            }
            if (safeRegion == SafeAreaRegion.TopRight ||
                safeRegion == SafeAreaRegion.Right ||
                safeRegion == SafeAreaRegion.BottomRight)
            {
                sPos.X =
                    rect.X -
                    emptySpace;
                sPos.Y =
                    halfRectPos.Y -
                    spaceShip.Height() / 2;

                pPos.X =
                    sPos.X -
                    spaceShip.Width() - 
                    emptySpace;
                pPos.Y =
                    halfRectPos.Y -
                    playerActor.CurrentFrameDimension.Y / 2;
            }
            if (safeRegion == SafeAreaRegion.TopLeft ||
                safeRegion == SafeAreaRegion.Left||
                safeRegion == SafeAreaRegion.BottomLeft)
            {
                sPos.X =
                    rect.X +
                    emptySpace;
                sPos.Y =
                    halfRectPos.Y -
                    spaceShip.Height() / 2;

                pPos.X =
                    sPos.X +
                    spaceShip.Width() +
                    emptySpace;
                pPos.Y =
                    halfRectPos.Y -
                    playerActor.CurrentFrameDimension.Y / 2;
            }
            if (safeRegion == SafeAreaRegion.Bottom)
            {
                sPos.X =
                    halfRectPos.X -
                    spaceShip.Width() / 2;
                sPos.Y =
                    rect.Y - emptySpace;

                pPos.X =
                    halfRectPos.X -
                    playerActor.CurrentFrameDimension.X / 2;
                pPos.Y =
                    sPos.Y -
                    spaceShip.Height() -
                    emptySpace;
            }
        }

        #region Helps Methods Objectives

        public void RemoveObjectives(List<InGameComponent> objectives)
        {
            if (objectives != null && objectives.Count > 0)
            {
                foreach (InGameComponent obj in objectives)
                {
                    objFinder.RemoveObjectives(obj);
                }
            }
        }

        public void AddObjectives(List<InGameComponent> objectives)
        {
            if (objectives != null && objectives.Count > 0)
            {
                foreach (InGameComponent obj in objectives)
                {
                    objFinder.AddObjectives(obj);
                }
            }
        }

        #endregion

        public static void InitializeMaxLevelNumber()
        {
            // Load World definitions
            XDocument WorldDoc = XDocument.Load("Content/Documents/WorldDefinitions.xml");

            int count = 0;

            foreach (var level in WorldDoc.Element("Levels").Descendants("Level"))
            {
                count++;
            }

            maxLevel = count;
        }
    }
}
