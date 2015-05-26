﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Agentes;
using Game.Juego;
using Microsoft.Xna.Framework;

namespace Game
{
    public abstract class Agent : SpriteComponent
    {
        public Agent(Microsoft.Xna.Framework.Game game, Vector2 tamano, Vector2 posicion, String nombreImagen)
            : base(game, tamano, posicion)
        {
            NombreImagen = nombreImagen;
            ColorImagen = Color.White;
            Direccion = "left";
            LoadContent();
        }

        public override void Colision(SpriteComponent otro, Vector2 desplazamiento) 
        {
            if (otro is Muro)
            {
                Mover(desplazamiento);
                if (desplazamiento.Y != 0)
                {
                    velocidad.Y = 0;
                    isOnGround = true;
                }
            }
            if (otro is Jugador)
            {
                Mover(desplazamiento);
                if (desplazamiento.Y != 0)
                {
                    velocidad.Y = 0;
                }
            }
        }

        public estados Percepciones(SpriteComponent sprite, SpriteComponent agente)
        {
            // lee las entradas (percepciones) y las compara para luego retornar un estado
            // 'jugador' son las percepciones del objetivo: Jugador.
            // 'agente' son las percepciones del objetivo: Agente.
            estados nuevoEstado = new estados(); // inicializa los estados.
            nuevoEstado.estado = new List<string>(); // inicializa el conjunto de estados.
            // verifica si es un jugador u otro objeto
            if (sprite is Jugador)
            {
                // calcula la distancia entre jugador y agente.
                Vector2 distancia = new Vector2(agente.Posicion.X - sprite.Posicion.X, agente.Posicion.Y - sprite.Posicion.Y);
                // verifica si el jugador esta cerca o no.
                if ((distancia.X > -64) && (distancia.X < 64)) nuevoEstado.estado.Add("player_is_near");
                else nuevoEstado.estado.Add("player_no_near");
                // verifica si el jugador esta en el aire o no.
                if (sprite.isOnGround) nuevoEstado.estado.Add("player_no_onAir");
                else nuevoEstado.estado.Add("player_onAir");
            }
            if (sprite is Muro)
            {
                // calcula la distancia entre el muro y agente.
                // tamano del muro
                //Vector2 distancia = new Vector2(agente.Posicion.X - (sprite.Posicion.X + sprite.Tamano.X), agente.Posicion.Y - (sprite.Posicion.Y ));
                // calcular distancia entre objetos
                float distanciaX;
                if (Direccion.Equals("left"))
                {
                    distanciaX = agente.Posicion.X - (sprite.Posicion.X + sprite.Tamano.X);
                } else distanciaX = agente.Posicion.X - sprite.Posicion.X;
                
                // verifica si el jugador esta cerca o no.
                if ((sprite.Posicion.Y >= agente.Posicion.Y) && (sprite.Posicion.Y < (agente.Posicion.Y + agente.Tamano.Y)))
                {
                    if ((distanciaX < agente.Tamano.X) && (distanciaX > -(agente.Tamano.X*2))) nuevoEstado.estado.Add("block_is_near");
                    else nuevoEstado.estado.Add("block_no_near");
	            }
                //if (((sprite.Posicion.Y + sprite.Tamano.Y ) > agente.Posicion.Y) && (sprite.Posicion.Y < (agente.Posicion.Y - agente.Tamano.Y)))
                //{
                //    if ((distanciaX < 32) && (distanciaX > 0)) nuevoEstado.estado.Add("muro_is_near");
                //    else nuevoEstado.estado.Add("muro_no_near");
                //}    
                if ( ( sprite.Posicion.Y  < ( agente.Posicion.Y - agente.Tamano.Y ) ) && ( ( sprite.Posicion.Y + sprite.Tamano.Y ) > agente.Posicion.Y))
                {
                    if ((distanciaX < (agente.Tamano.X) + 1) && (distanciaX > -(agente.Tamano.X) - 1)) nuevoEstado.estado.Add("muro_is_near");
                    else nuevoEstado.estado.Add("muro_no_near");
                }
            }
            
            // retorna los estados agregados.
            return nuevoEstado;
        }

        public acciones Regla(estados e, reglas r)
        {
            // deberia leer las reglas desde un archivo
            acciones nuevasAcciones = new acciones();
            nuevasAcciones.accion = new List<string>();
            int cond = 0;
            
            foreach (var rule in r.regla)
            {
                foreach (var state in e.estado)
                {
                    //Console.Out.WriteLine(rule.accion+state);
                    foreach (var item in rule.condicion)
                    {
                        if (item == state) cond++;
                        if (cond == rule.condicion.Count) { nuevasAcciones.accion.Add(rule.accion); cond = 0; }
                    }
                }
            }
            
            return nuevasAcciones;
        }

        public void Sensor(SpriteComponent sprite) 
        {
            //asigna los estados
            estados e = new estados();
            e = Percepciones(sprite, this);
            // acciones necesitan reglas
            reglas r = new reglas();
            //r.regla = new List<condiciones>();
            //// condiciones
            //condiciones cond1 = new condiciones(); // tecnicamente esto es una regla
            //cond1.accion = "perseguir";
            //cond1.condicion = new List<string>();
            //cond1.condicion.Add("player_near"); // y esto son las condiciones de dicha regla
            //condiciones cond2 = new condiciones();
            //cond2.accion = "saltar";
            //cond2.condicion = new List<string>();
            //cond2.condicion.Add("block_is_near");
            //condiciones cond3 = new condiciones();
            //cond3.accion = "avanzar";
            //cond3.condicion = new List<string>();
            ////cond3.condicion.Add("player_no_near");
            //cond3.condicion.Add("muro_no_near");
            //condiciones cond4 = new condiciones();
            //cond4.accion = "cambiar_direccion";
            //cond4.condicion = new List<string>();
            //cond4.condicion.Add("muro_is_near");
            ////
            //r.regla.Add(cond1);
            //r.regla.Add(cond2);
            //r.regla.Add(cond4);
            //r.regla.Add(cond3);
            ////
            //////XML.Serialize(r,"reglas.dat");
            r = XML.Deserialize<reglas>("reglas.dat");
            acciones action = new acciones();
            action = Regla(e, r);
            //
            Comportamiento(action);
        }

        public abstract void Comportamiento(acciones a);

        public void perseguir()
        {
            // calcula el mejor camino.. aun por definir.
            if (isOnGround)
            {
                velocidad.Y = -Salto;
                isOnGround = false;
            } 
        }

        public void saltar() 
        {
            if (isOnGround)
            {
                velocidad.Y = -Salto;

                isOnGround = false;
            }    
        }
        
        public void avanzar()
        {
            if (Direccion.Equals("left"))
            {
                velocidad.X = -100;
            }
            else if (Direccion.Equals("right"))
            {
                velocidad.X = 100;
            }
            
        }

        public void avanzarDireccion(String Direccion)
        {
            if (Direccion.Equals("left"))
            {
                velocidad.X = -100;
            }
            else if (Direccion.Equals("right"))
            {
                velocidad.X = 100;
            }

        }

        public void cambiarDireccion() 
        {
            if (Direccion.Equals("left")) Direccion = "right";
            else Direccion = "left";
            avanzar(); // hace una llamada a la funcion avanzar para no entrar en un loop.
        }



    }
}
