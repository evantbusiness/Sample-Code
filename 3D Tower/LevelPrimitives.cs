using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Thivierge_Midterm
{
    public class LevelPrimitives : DrawableGameComponent
    {
        // Camera
        Camera camera;

        // Vertex data
        VertexPositionTexture[] strip1;
        VertexPositionTexture[] strip2;
        VertexPositionTexture[] strip3;
        VertexPositionTexture[] strip4;
        VertexPositionTexture[] strip5;
        VertexPositionTexture[] strip6;
        VertexBuffer[] stripBuffer;

        // Effect
        BasicEffect effect;

        //Transformation Matricies
        Matrix worldTranslation = Matrix.Identity;
        Matrix worldRotation = Matrix.Identity;
        Matrix worldScaling = Matrix.Identity;

        // Texture control state
        SamplerState clampTextureAddressMode;

        // Texture info
        Texture2D[] texture;

        public LevelPrimitives(Game game, Camera camera)
            : base (game)
        {
            this.camera = camera;
        }

        public override void Initialize()
        {

            //Set cull mode to None (off)
            // UNREMARK TO REMOVE CULLING:
            //RasterizerState rs = new RasterizerState();
            //rs.CullMode = CullMode.None;
            //GraphicsDevice.RasterizerState = rs;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Strip verts for triangles
            strip1 = new VertexPositionTexture[4];

            //strip 1 - back
            strip1[0] = new VertexPositionTexture(
                new Vector3(20, 0, 20), new Vector2(1, 1));
            strip1[1] = new VertexPositionTexture(
                new Vector3(20, 200, 20), new Vector2(1, 0));
            strip1[2] = new VertexPositionTexture(
                new Vector3(-20, 0, 20), new Vector2(0, 1));
            strip1[3] = new VertexPositionTexture(
                new Vector3(-20, 200, 20), new Vector2(0, 0));

            strip2 = new VertexPositionTexture[4];

            //strip 2 - right
            strip2[0] = new VertexPositionTexture(
                new Vector3(20, 0, -20), new Vector2(1, 1));
            strip2[1] = new VertexPositionTexture(
                new Vector3(20, 200, -20), new Vector2(1, 0));
            strip2[2] = new VertexPositionTexture(
                new Vector3(20, 0, 20), new Vector2(0, 1));
            strip2[3] = new VertexPositionTexture(
                new Vector3(20, 200, 20), new Vector2(0, 0));

            strip3 = new VertexPositionTexture[4];

            //strip 3 - left
            strip3[0] = new VertexPositionTexture(
                new Vector3(-20, 200, -20), new Vector2(1, 1));
            strip3[1] = new VertexPositionTexture(
                new Vector3(-20, 0, -20), new Vector2(1, 0));
            strip3[2] = new VertexPositionTexture(
                new Vector3(-20, 200, 20), new Vector2(0, 1));
            strip3[3] = new VertexPositionTexture(
                new Vector3(-20, 0, 20), new Vector2(0, 0));

            strip4 = new VertexPositionTexture[4];

            //strip 4 - top
            strip4[0] = new VertexPositionTexture(
                new Vector3(20, 200, 20), new Vector2(1, 1));
            strip4[1] = new VertexPositionTexture(
                new Vector3(20, 200, -20), new Vector2(1, 0));
            strip4[2] = new VertexPositionTexture(
                new Vector3(-20, 200, 20), new Vector2(0, 1));
            strip4[3] = new VertexPositionTexture(
                new Vector3(-20, 200, -20), new Vector2(0, 0));

            strip5 = new VertexPositionTexture[4];

            //strip 5 - front
            strip5[0] = new VertexPositionTexture(
                new Vector3(20, 200, -20), new Vector2(1, 1));
            strip5[1] = new VertexPositionTexture(
                new Vector3(20, 0, -20), new Vector2(1, 0));
            strip5[2] = new VertexPositionTexture(
                new Vector3(-20, 200, -20), new Vector2(0, 1));
            strip5[3] = new VertexPositionTexture(
                new Vector3(-20, 0, -20), new Vector2(0, 0));

            strip6 = new VertexPositionTexture[4];

            //strip 6 - terrain
            strip6[0] = new VertexPositionTexture(
                new Vector3(20, 0, 20), new Vector2(1, 1));
            strip6[1] = new VertexPositionTexture(
                new Vector3(-20, 0, 20), new Vector2(1, 0));
            strip6[2] = new VertexPositionTexture(
                new Vector3(20, 0, -20), new Vector2(0, 1));
            strip6[3] = new VertexPositionTexture(
                new Vector3(-20, 0, -20), new Vector2(0, 0));


            //Set vertex data in vertex buffer 
            stripBuffer = new VertexBuffer[6];
            stripBuffer[0] = new VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), strip1.Length, BufferUsage.None);
            stripBuffer[0].SetData(strip1);

            stripBuffer[1] = new VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), strip2.Length, BufferUsage.None);
            stripBuffer[1].SetData(strip2);

            stripBuffer[2] = new VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), strip3.Length, BufferUsage.None);
            stripBuffer[2].SetData(strip3);

            stripBuffer[3] = new VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), strip4.Length, BufferUsage.None);
            stripBuffer[3].SetData(strip4);

            stripBuffer[4] = new VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), strip5.Length, BufferUsage.None);
            stripBuffer[4].SetData(strip5);

            stripBuffer[5] = new VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), strip6.Length, BufferUsage.None);
            stripBuffer[5].SetData(strip6);

            //Initialize BasicEffect
            effect = new BasicEffect(GraphicsDevice);

            //Handle Textures
            clampTextureAddressMode = new SamplerState
            {
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp
            };


            //Load texture
            texture = new Texture2D[6];

            texture[0] = Game.Content.Load<Texture2D>(@"Textures\wall");
            texture[1] = Game.Content.Load<Texture2D>(@"Textures\wall");
            texture[2] = Game.Content.Load<Texture2D>(@"Textures\wall2");
            texture[3] = Game.Content.Load<Texture2D>(@"Textures\ceiling");
            texture[4] = Game.Content.Load<Texture2D>(@"Textures\wall2");
            texture[5] = Game.Content.Load<Texture2D>(@"Textures\floor");

            worldScaling *= Matrix.CreateScale(1.5f);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {


            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //Adjust graphics device to draw from within texture boundaries using clamp
            GraphicsDevice.SamplerStates[0] = clampTextureAddressMode;

            //Set object and camera info
            effect.World = worldScaling * worldRotation * worldTranslation;

            effect.View = camera.view;              //Set effect to camera's view
            effect.Projection = camera.projection;  // Same for camera's projection
            effect.TextureEnabled = true;           // Texturing on, allows texturing...else white primitive
            effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);

            //Begin effect and draw for each pass
            //Loop through each pass with current effect

            GraphicsDevice.SetVertexBuffer(stripBuffer[0]); // Vertex Buffer
            effect.Texture = texture[0]; // Texture
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {

                pass.Apply();

                //Draw primitives
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>
                    (PrimitiveType.TriangleStrip, strip1, 0, 2);
            }

            GraphicsDevice.SetVertexBuffer(stripBuffer[1]); // Vertex Buffer
            effect.Texture = texture[1]; // Texture
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {

                pass.Apply();

                //Draw primitives
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>
                    (PrimitiveType.TriangleStrip, strip2, 0, 2);
            }

            GraphicsDevice.SetVertexBuffer(stripBuffer[2]); // Vertex Buffer
            effect.Texture = texture[2]; // Texture
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {

                pass.Apply();

                //Draw primitives
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>
                    (PrimitiveType.TriangleStrip, strip3, 0, 2);
            }

            GraphicsDevice.SetVertexBuffer(stripBuffer[3]); // Vertex Buffer
            effect.Texture = texture[3]; // Texture
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {

                pass.Apply();

                //Draw primitives
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>
                    (PrimitiveType.TriangleStrip, strip4, 0, 2);
            }

            GraphicsDevice.SetVertexBuffer(stripBuffer[4]); // Vertex Buffer
            effect.Texture = texture[4]; // Texture
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {

                pass.Apply();

                //Draw primitives
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>
                    (PrimitiveType.TriangleStrip, strip5, 0, 2);
            }

            GraphicsDevice.SetVertexBuffer(stripBuffer[5]); // Vertex Buffer
            effect.Texture = texture[5]; // Texture
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {

                pass.Apply();

                //Draw primitives
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>
                    (PrimitiveType.TriangleStrip, strip6, 0, 2);
            }

            base.Draw(gameTime);
        }
    }
}
