using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Thivierge_Midterm
{
    class Platform
    {
        private Matrix translation = Matrix.Identity;
        private Matrix world = Matrix.Identity;
        public Model model { get; protected set; }
        public BoundingSphere sphere;
        public BoundingBox box;

        // Position
        Vector3 position;

        public Platform(Model model, Vector3 position)
        {
            this.model = model;
            this.position = position;
        }

        public void Update()
        {
            translation = Matrix.CreateTranslation(position);

            sphere = new BoundingSphere(new Vector3(position.X, position.Y, position.Z), 4.0f);

            float radius = sphere.Radius;

            Vector3 min = new Vector3(sphere.Center.X - radius, sphere.Center.Y - radius + 3.0f, sphere.Center.Z - radius + 2.0f);
            Vector3 max = new Vector3(sphere.Center.X + radius, sphere.Center.Y + radius + 3.0f, sphere.Center.Z + radius + 5.0f);
            box = new BoundingBox(min, max);
        }

        public void Draw(Camera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    be.Projection = camera.projection;
                    be.View = camera.view;
                    be.World = GetWorld() * mesh.ParentBone.Transform;
                }

                mesh.Draw();
            }
        }

        public Matrix GetWorld()
        {
            return world * translation;
        }

        public bool CollidesWith(BoundingBox platformBox, BoundingSphere otherSphere)
        {
            if (platformBox.Intersects(otherSphere))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
