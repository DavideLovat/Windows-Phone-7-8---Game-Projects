using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZoneGame
{
    public class BulletManager : ISampleComponent
    {
        Texture2D bulletTexture;
        // the color data for the images; used for pixel collision
        Color[] bulletTextureData;
        Vector2 worldSize;
        List<Bullet> bullets = new List<Bullet>();

        public List<Bullet> Bullets
        {
            get { return bullets; }
        }

        public BulletManager(Texture2D bulletTexture, Vector2 worldSize)
        {
            this.bulletTexture = bulletTexture;
            bulletTextureData = ExtractCollisionData(bulletTexture);
            this.worldSize = worldSize;
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (Bullet bullet in bullets)
                bullet.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach(Bullet bullet in bullets)
                bullet.Draw(spriteBatch, gameTime);
        }

        public void GenerateBullet(Vector2 position, Vector2 velocity, Color color)
        {
            Bullet b = new Bullet(bulletTexture, worldSize, position, velocity, color);
            b.Origin = new Vector2(b.Width()/ 2f, b.Height() / 2f);
            // we use the Atan2 method to compute the rotation of the bullet
            // base on its velocity.
            b.Rotation = (float)Math.Atan2(velocity.Y, velocity.X);
            b.TextureData = bulletTextureData;
            bullets.Add(b);            
        }

        protected virtual Color[] ExtractCollisionData(Texture2D texture)
        {
            // Extract collision data
            Color [] newTextureData =
                new Color[texture.Width * texture.Height];
            texture.GetData(newTextureData);
            
            return newTextureData;

        }
    }
}
