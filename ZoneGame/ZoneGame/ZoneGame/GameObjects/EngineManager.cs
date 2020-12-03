using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    class EngineManager : ISampleComponent
    {        
        List<Engine> engines = new List<Engine>();
        List<Engine> enginesOn = new List<Engine>();
        List<Engine> enginesOff = new List<Engine>();
        public List<Engine> Engines
        {
            get { return engines; }
        }

        public List<Engine> EnginesOn
        {
            get { return enginesOn; }
        }

        public List<Engine> EnginesOff
        {
            get { return enginesOff; }
        }

        public int MaxEngines
        {
            get { return engines.Count; }
        }

        int currentEnginesOn = 0;

        public int CurrentEnginesOn
        {
            get { return currentEnginesOn; }
        }

        public bool AllEnginesOn
        {
            get 
            {
                return currentEnginesOn == MaxEngines;
            }
        }

        public EngineManager(List<Engine> engines)
        {
            this.engines = engines;
            
            foreach (Engine engine in engines)
            {
                if (engine.IsEnegryFull)
                {
                    currentEnginesOn++;
                    enginesOn.Add(engine);
                }
                else
                {
                    enginesOff.Add(engine);
                }
            }
        }

        #region Public Methods

        public void Update(GameTime gameTime)
        {
            foreach (Engine engine in engines)
            {
                engine.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (Engine engine in engines)
            {
                engine.Draw(spriteBatch, gameTime);
            }
        }

        public void Intersect(Rectangle rect)
        {
            foreach (Engine engine in engines)
            {
                if (!engine.IsEnegryFull)
                {
                    if (engine.CentralCollisionArea.Intersects(rect))
                    {
                        engine.IncreaseEnergy(1);
                        if (engine.IsEnegryFull)
                        {
                            currentEnginesOn++;
                            enginesOn.Add(engine);
                            enginesOff.Remove(engine);
                            if (GameplayScreen.isAddEngineObjectives)
                            {
                                GameplayScreen.objFinder.RemoveObjectives(engine);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
