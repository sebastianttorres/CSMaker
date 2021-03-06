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


namespace CSMaker
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Objeto : SpriteComponent
    {
        public Objeto(Microsoft.Xna.Framework.Game game, Vector2 tamano, Vector2 posicion, String img)
            : base(game, tamano, posicion)
        {
            Peso = 0.0f;
            NombreImagen = img;
            ColorImagen = Color.White;
            LoadContent();
        }

        public override void Colision(SpriteComponent otro, Vector2 desplazamiento)
        {
            // no se hace nada
        }
    }
}
