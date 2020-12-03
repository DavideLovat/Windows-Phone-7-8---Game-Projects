#region File Description
//-----------------------------------------------------------------------------
// AnimatingSprite.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace ZoneGame
{
    /// <summary>
    /// A sprite sheet with flipbook-style animations.
    /// </summary>
    public class AnimatingSprite
    {
        #region Graphics Data


        /// <summary>
        /// The content path and name of the texture for this spell animation.
        /// </summary>
        private string textureName;

        /// <summary>
        /// The content path and name of the texture for this spell animation.
        /// </summary>
        public string TextureName
        {
            get { return textureName; }
            set { textureName = value; }
        }


        /// <summary>
        /// The texture for this spell animation.
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// The texture for this spell animation.
        /// </summary>
        [ContentSerializerIgnore]
        public Texture2D Texture
        {
            get { return texture; }
            set 
            { 
                texture = value;
                textureData = ExtractCollisionData(texture);
            }
        }


        private Color[] textureData;

        public Color[] TextureData
        {
            get { return textureData; }
            private set { textureData = value; }
        }

        public Color[] currentSpriteTextureData
        {
            get 
            {
                Color[] newTextureData =
                    new Color[frameDimensions.X * frameDimensions.Y];

                int column = 0;
                int frame = 0;
                if (currentFrame > currentAnimation.EndingFrame)
                {
                    frame = (currentFrame - 1) - 1;
                    column = frame / framesPerRow;                    
                }
                else
                {
                    frame = currentFrame - 1;
                    column = frame / framesPerRow;
                }
   
                Rectangle rectangle = new Rectangle(
                    (frame - (column * framesPerRow)) * frameDimensions.X,
                    column * frameDimensions.Y,
                    frameDimensions.X, frameDimensions.Y);
                
                texture.GetData<Color>(
                    0,
                    rectangle,
                    newTextureData,
                    0,
                    newTextureData.Length);

                return newTextureData;
             
            }
        }

        #endregion


        #region Frame Data


        /// <summary>
        /// The dimensions of a single frame of animation.
        /// </summary>
        private Point frameDimensions;

        /// <summary>
        /// The width of a single frame of animation.
        /// </summary>
        public Point FrameDimensions
        {
            get { return frameDimensions; }
            set 
            { 
                frameDimensions = value;
                frameOrigin.X = frameDimensions.X / 2;
                frameOrigin.Y = frameDimensions.Y / 2;
            }
        }


        /// <summary>
        /// The origin of the sprite, within a frame.
        /// </summary>
        private Point frameOrigin;


        /// <summary>
        /// The number of frames in a row in this sprite.
        /// </summary>
        private int framesPerRow;

        /// <summary>
        /// The number of frames in a row in this sprite.
        /// </summary>
        public int FramesPerRow
        {
            get { return framesPerRow; }
            set { framesPerRow = value; }
        }


        /// <summary>
        /// The offset of this sprite from the position it's drawn at.
        /// </summary>
        private Vector2 sourceOffset;

        /// <summary>
        /// The offset of this sprite from the position it's drawn at.
        /// </summary>
        [ContentSerializer(Optional=true)]
        public Vector2 SourceOffset
        {
            get { return sourceOffset; }
            set { sourceOffset = value; }
        }


        #endregion


        #region Animation Data


        /// <summary>
        /// The animations defined for this sprite.
        /// </summary>
        private List<Animation> animations = new List<Animation>();

        /// <summary>
        /// The animations defined for this sprite.
        /// </summary>
        public List<Animation> Animations
        {
            get { return animations; }
            set { animations = value; }
        }

        /// <summary>
        /// Enumerate the animations on this animated sprite.
        /// </summary>
        /// <param name="animationName">The name of the animation.</param>
        /// <returns>The animation if found; null otherwise.</returns>
        public Animation this[string animationName]
        {
            get
            {
                if (String.IsNullOrEmpty(animationName))
                {
                    return null;
                }
                foreach (Animation animation in animations)
                {
                    if (String.Compare(animation.Name, animationName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return animation;
                    }
                }
                return null;
            }
        }


        /// <summary>
        /// Add the animation to the list, checking for name collisions.
        /// </summary>
        /// <returns>True if the animation was added to the list.</returns>
        public bool AddAnimation(Animation animation)
        {
            if ((animation != null) && (this[animation.Name] == null))
            {
                animations.Add(animation);
                return true;
            }

            return false;
        }


        #endregion


        #region Playback


        /// <summary>
        /// The animation currently playing back on this sprite.
        /// </summary>
        private Animation currentAnimation = null;

        /// <summary>
        /// The current frame in the current animation.
        /// </summary>
        public int CurrentFrame
        {
            get { return currentFrame; }
        }
        private int currentFrame;

        public int StartingFrame
        {
            get
            {
                if (currentAnimation != null)
                    return currentAnimation.StartingFrame;
                else
                    return 0;
            }
        }

        public int EndingFrame
        {
            get 
            {
                if (currentAnimation != null)
                    return currentAnimation.EndingFrame;
                else
                    return 0;
            }
        }

        public int CurrentRowFrame
        {
            get 
            {
                if (currentAnimation != null)
                {
                    return framesPerRow - (EndingFrame - CurrentFrame);
                }
                else
                    return 0;
            }
        }

        /// <summary>
        /// The elapsed time since the last frame switch.
        /// </summary>
        private float elapsedTime;


        /// <summary>
        /// The source rectangle of the current frame of animation.
        /// </summary>
        private Rectangle sourceRectangle;

        /// <summary>
        /// The source rectangle of the current frame of animation.
        /// </summary>
        public Rectangle SourceRectangle
        {
            get { return sourceRectangle; }
        }


        /// <summary>
        /// Play the given animation on the sprite.
        /// </summary>
        /// <remarks>The given animation may be null, to clear any animation.</remarks>
        public void PlayAnimation(Animation animation, bool reset)
        {
            // start the new animation, ignoring redundant Plays
            if (animation != currentAnimation)
            {
                Animation previousAnimation = currentAnimation;
                currentAnimation = animation;
                if (!reset && previousAnimation != null)
                {
                    currentFrame =
                            currentAnimation.StartingFrame +
                            Math.Abs(currentFrame - previousAnimation.StartingFrame);  
                }
                else
                {
                    ResetAnimation();
                }
            }
        }

        
        /// <summary>
        /// Play an animation given by index.
        /// </summary>
        public void PlayAnimation(int index, bool reset)
        {
            // check the parameter
            if ((index < 0) || (index >= animations.Count))
            {
                throw new ArgumentOutOfRangeException("index");
            }

            PlayAnimation(this.animations[index], reset);
        }


        /// <summary>
        /// Play an animation given by name.
        /// </summary>
        public void PlayAnimation(string name, bool reset)
        {
            // check the parameter
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            PlayAnimation(this[name], reset);
        }


        /// <summary>
        /// Play a given animation name, with the given direction suffix.
        /// </summary>
        /// <example>
        /// For example, passing "Walk" and Direction.South will play the animation
        /// named "WalkSouth".
        /// </example>
        public void PlayAnimation(string name, CardinalDirection direction, bool reset)
        {
            // check the parameter
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            PlayAnimation(name + direction.ToString(), reset);
        }


        /// <summary>
        /// Reset the animation back to its starting position.
        /// </summary>
        public void ResetAnimation()
        {
            elapsedTime = 0f;
            if (currentAnimation != null)
            {
                currentFrame = currentAnimation.StartingFrame;
                // calculate the source rectangle by updating the animation
                UpdateAnimation(0f);
            }
        }


        /// <summary>
        /// Advance the current animation to the final sprite.
        /// </summary>
        public void AdvanceToEnd()
        {
            if (currentAnimation != null)
            {
                currentFrame = currentAnimation.EndingFrame;
                // calculate the source rectangle by updating the animation
                UpdateAnimation(0f);
            }
        }


        /// <summary>
        /// Stop any animation playing on the sprite.
        /// </summary>
        public void StopAnimation()
        {
            currentAnimation = null;
        }


        /// <summary>
        /// Returns true if playback on the current animation is complete, or if
        /// there is no animation at all.
        /// </summary>
        public bool IsPlaybackComplete
        {
            get
            {
                return ((currentAnimation == null) ||
                    (!currentAnimation.IsLoop &&
                    (currentFrame > currentAnimation.EndingFrame)));
            }
        }


        #endregion


        #region Updating


        /// <summary>
        /// Update the current animation.
        /// </summary>
        public void UpdateAnimation(float elapsedSeconds)
        {
            if (IsPlaybackComplete)
            {
                return;
            }

            // loop the animation if needed
            if (currentAnimation.IsLoop && (currentFrame > currentAnimation.EndingFrame))
            {
                currentFrame = currentAnimation.StartingFrame;
            }

            // update the source rectangle
            int column = (currentFrame - 1) / framesPerRow;
            sourceRectangle = new Rectangle(
                (currentFrame - 1 - (column * framesPerRow)) * frameDimensions.X,
                column * frameDimensions.Y,
                frameDimensions.X, frameDimensions.Y);

            // update the elapsed time
            elapsedTime += elapsedSeconds;

            // advance to the next frame if ready
            while (elapsedTime * 1000f > (float)currentAnimation.Interval)
            {
                currentFrame++;
                elapsedTime -= (float)currentAnimation.Interval / 1000f;
            }
        }


        #endregion


        #region Drawing


        /// <summary>
        /// Draw the sprite at the given position.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch object used to draw.</param>
        /// <param name="position">The position of the sprite on-screen.</param>
        /// <param name="layerDepth">The depth at which the sprite is drawn.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, float layerDepth)
        {
            Draw(spriteBatch, position, layerDepth, SpriteEffects.None);
        }


        /// <summary>
        /// Draw the sprite at the given position.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch object used to draw.</param>
        /// <param name="position">The position of the sprite on-screen.</param>
        /// <param name="layerDepth">The depth at which the sprite is drawn.</param>
        /// <param name="spriteEffect">The sprite-effect applied.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, float layerDepth, 
            SpriteEffects spriteEffect)
        {
            // check the parameters
            if (spriteBatch == null)
            {
                throw new ArgumentNullException("spriteBatch");
            }

            if (texture != null)
            {
                spriteBatch.Draw(texture, position, sourceRectangle, Color.White, 0f,
                    sourceOffset, 1f, spriteEffect, 
                    MathHelper.Clamp(layerDepth, 0f, 1f));
            }
        }


        #endregion

        #region Texture Data

        protected virtual Color[] ExtractCollisionData(Texture2D texture)
        {
            // Extract collision data
            Color [] newTextureData =
                new Color[texture.Width * texture.Height];
            texture.GetData(newTextureData);
            
            return newTextureData;
        }

        #endregion

        #region ICloneable Members

        public AnimatingSprite DeepCopy()
        {
            AnimatingSprite animatingSprite = new AnimatingSprite();

            animatingSprite.animations.AddRange(animations);
            animatingSprite.currentAnimation = currentAnimation;
            animatingSprite.currentFrame = currentFrame;
            animatingSprite.elapsedTime = elapsedTime;
            animatingSprite.frameDimensions = frameDimensions;
            animatingSprite.frameOrigin = frameOrigin;
            animatingSprite.framesPerRow = framesPerRow;
            animatingSprite.sourceOffset = sourceOffset;
            animatingSprite.sourceRectangle = sourceRectangle;
            animatingSprite.texture = texture;
            animatingSprite.textureData = textureData;
            animatingSprite.textureName = textureName;

            return animatingSprite;
        }

        #endregion

        /*
        #region Content Type Reader


        /// <summary>
        /// Read an AnimatingSprite object from the content pipeline.
        /// </summary>
        public class AnimatingSpriteReader : ContentTypeReader<AnimatingSprite>
        {
            /// <summary>
            /// Read an AnimatingSprite object from the content pipeline.
            /// </summary>
            protected override AnimatingSprite Read(ContentReader input, 
                AnimatingSprite existingInstance)
            {
                AnimatingSprite animatingSprite = existingInstance;
                if (animatingSprite == null)
                {
                    animatingSprite = new AnimatingSprite();
                }

                animatingSprite.AssetName = input.AssetName;

                animatingSprite.TextureName = input.ReadString();
                animatingSprite.Texture =
                    input.ContentManager.Load<Texture2D>(
                        System.IO.Path.Combine(@"Textures",
                        animatingSprite.TextureName));
                animatingSprite.FrameDimensions = input.ReadObject<Point>();
                animatingSprite.FramesPerRow = input.ReadInt32();
                animatingSprite.SourceOffset = input.ReadObject<Vector2>();
                animatingSprite.Animations.AddRange(
                    input.ReadObject<List<Animation>>());

                return animatingSprite;
            }
        }


        #endregion
        

        #region ICloneable Members


        /// <summary>
        /// Creates a clone of this object.
        /// </summary>
        public object Clone()
        {
            AnimatingSprite animatingSprite = new AnimatingSprite();

            animatingSprite.animations.AddRange(animations);
            animatingSprite.currentAnimation = currentAnimation;
            animatingSprite.currentFrame = currentFrame;
            animatingSprite.elapsedTime = elapsedTime;
            animatingSprite.frameDimensions = frameDimensions;
            animatingSprite.frameOrigin = frameOrigin;
            animatingSprite.framesPerRow = framesPerRow;
            animatingSprite.sourceOffset = sourceOffset;
            animatingSprite.sourceRectangle = sourceRectangle;
            animatingSprite.texture = texture;
            animatingSprite.textureName = textureName;

            return animatingSprite;
        }


        #endregion
         * */
    }
}
