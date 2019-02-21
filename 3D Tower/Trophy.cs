using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Thivierge_Midterm
{
    class Trophy : BasicModel
    {
        Matrix scale = Matrix.Identity;
        Matrix rotationAdjust = Matrix.Identity;
        Matrix rotation = Matrix.Identity;
        Matrix translation = Matrix.Identity;

        //Scale
        Vector3 scaler = new Vector3(0.5f, 0.5f, 0.5f);

        //Position
        Vector3 position;

        public Trophy(Model m, Vector3 position)
            : base(m)
        {
            this.position = position;
        }

        public override void Update()
        {
            scale = Matrix.CreateScale(scaler);
            rotationAdjust = Matrix.CreateRotationX(-(MathHelper.Pi / 2));
            rotation *= Matrix.CreateRotationZ(MathHelper.Pi / 180);
            translation = Matrix.CreateTranslation(position);
        }

        public override Matrix GetWorld()
        {
            return world * scale * rotation * rotationAdjust * translation;
        }
    }
}
